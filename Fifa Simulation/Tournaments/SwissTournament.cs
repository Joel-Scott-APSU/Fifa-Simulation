using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Fifa_Simulation.Helpers;

namespace Fifa_Simulation.Tournaments
{
    public class SwissTournament
    {
        public List<Team> ActiveTeams { get; private set; }
        public List<Team> AdvancedTeams { get; } = new();
        public List<Team> EliminatedTeams { get; } = new();

        private readonly List<string> matchLog = new();
        private readonly HeadToHead h2h = new();

        // (optional) keep if you still want to detect/avoid rematches later
        private readonly HashSet<MatchKey> played = new();

        // Snapshot seeds coming INTO swiss (used as a fallback tiebreak)
        private readonly Dictionary<Team, int> swissIncomingSeed = new();

        public SwissTournament(List<Team> seededTeams)
        {
            ActiveTeams = new List<Team>(seededTeams);

            // capture "incoming swiss seed" once
            foreach (var t in ActiveTeams)
                swissIncomingSeed[t] = t.Seed;
        }

        /// <summary>
        /// Runs swiss and returns the TOP 8 reseeded (sorted) for single elimination.
        /// Sorting:
        ///  1) Swiss record (Wins desc, Losses asc)
        ///  2) SOS = Sum over opponents of (opp.Wins - opp.Losses) desc
        ///  3) H2H between tied teams (if they played) desc
        ///  4) Opponent avg incoming seed (lower is harder schedule) asc
        ///  5) Incoming swiss seed (lower better) asc
        ///  6) Elo desc
        /// </summary>
        public List<Team> Run()
        {
            for (int round = 1; round <= 5 && AdvancedTeams.Count < 8; round++)
            {
                var recordGroups = ActiveTeams
                    .GroupBy(t => (t.Wins, t.Losses))
                    .Select(g => g.OrderBy(t => t.Seed).ToList())
                    .ToList();

                foreach (var group in recordGroups)
                {
                    if (group.Count >= 2)
                        RunRound(PairBySeed(group), round);
                }
            }

            // Return reseeded top8 for single elim
            return GetReseededTop8();
        }

        private List<Team> GetReseededTop8()
        {
            // If you always stop when AdvancedTeams.Count == 8, this is safe.
            // If not, still take best 8 by the same rules.
            var candidates = AdvancedTeams.Count >= 8
                ? AdvancedTeams.ToList()
                : AdvancedTeams.Concat(ActiveTeams).ToList(); // fallback if loop exits early

            // Precompute metrics
            var sos = candidates.ToDictionary(t => t, ComputeSoS);
            var oppAvgSeed = candidates.ToDictionary(t => t, ComputeOpponentAvgIncomingSeed);

            var ordered = candidates
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenByDescending(t => sos[t])
                // H2H is pairwise, so we apply it in a custom tie-break pass below.
                .ThenBy(t => oppAvgSeed[t]) // lower avg opp seed = tougher schedule
                .ThenBy(t => swissIncomingSeed.TryGetValue(t, out var s) ? s : int.MaxValue)
                .ThenByDescending(t => t.elo)
                .ToList();

            // Apply a simple H2H bubble for adjacent ties (same record + same SOS)
            // (Good enough for 8 teams; avoids complex multi-way tie logic)
            for (int i = 0; i < ordered.Count - 1; i++)
            {
                var a = ordered[i];
                var b = ordered[i + 1];

                bool tied =
                    a.Wins == b.Wins &&
                    a.Losses == b.Losses &&
                    sos[a] == sos[b];

                if (!tied) continue;

                int aOverB = h2h.GetWins(a, b);
                int bOverA = h2h.GetWins(b, a);

                // If b beat a head-to-head, swap them
                if (bOverA > aOverB)
                {
                    ordered[i] = b;
                    ordered[i + 1] = a;

                    // step back one to stabilize ordering a bit
                    if (i > 0) i -= 2;
                }
            }

            // Take top 8 and assign swiss seeds 1..8 for your single elim bracket
            var top8 = ordered.Take(8).ToList();
            for (int i = 0; i < top8.Count; i++)
                top8[i].Seed = i + 1;

            return top8;
        }

        /// <summary>
        /// SOS = sum(opp.Wins - opp.Losses) across all opponents faced in swiss.
        /// Uses final swiss W/L values at the end of swiss (after everyone finishes).
        /// </summary>
        private int ComputeSoS(Team team)
        {
            int total = 0;
            foreach (var opp in team.SwissOpponents)
                total += (opp.Wins - opp.Losses);
            return total;
        }

        /// <summary>
        /// Lower = harder schedule (you faced better seeds).
        /// </summary>
        private double ComputeOpponentAvgIncomingSeed(Team team)
        {
            if (team.SwissOpponents.Count == 0) return double.PositiveInfinity;

            double sum = 0;
            int n = 0;

            foreach (var opp in team.SwissOpponents)
            {
                n++;
                sum += swissIncomingSeed.TryGetValue(opp, out var s) ? s : 9999;
            }

            return sum / n;
        }

        private void RunRound(List<(Team, Team)> pairings, int round)
        {
            foreach (var (a, b) in pairings)
            {
                matchLog.Add(
                    $"R{round}: Seed {a.Seed} ({a.Wins}-{a.Losses}) vs Seed {b.Seed} ({b.Wins}-{b.Losses})"
                );

                // Track matchup (no printing)
                played.Add(new MatchKey(a, b));

                Team winner = new Match(a, b).Play();
                Team loser = (winner == a) ? b : a;

                // Track "who you played"
                a.SwissOpponents.Add(b);
                b.SwissOpponents.Add(a);

                // Track who beat who
                h2h.RecordWin(winner, loser);
            }

            ResolveTeams();
        }

        private void ResolveTeams()
        {
            foreach (var team in ActiveTeams.ToList())
            {
                if (team.Wins == 3)
                {
                    AdvancedTeams.Add(team);
                    ActiveTeams.Remove(team);
                }
                else if (team.Losses == 3)
                {
                    EliminatedTeams.Add(team);
                    ActiveTeams.Remove(team);
                }
            }
        }

        private static List<(Team, Team)> PairBySeed(List<Team> teams)
        {
            var ordered = teams.OrderBy(t => t.Seed).ToList();
            var pairings = new List<(Team, Team)>();

            int l = 0, r = ordered.Count - 1;
            while (l < r)
            {
                pairings.Add((ordered[l], ordered[r]));
                l++;
                r--;
            }

            return pairings;
        }

        public void DisplaySwissResults(StreamWriter writer, List<Team> teams)
        {
            writer.WriteLine("\n--- ADVANCED FROM SWISS ---");
            foreach (var t in teams.OrderBy(t => t.Wins).ThenBy(t => t.Losses))
                writer.WriteLine($"{t.name} ({t.Wins}-{t.Losses})");

            writer.WriteLine("\n--- ELIMINATED IN SWISS ---");
            foreach (var t in EliminatedTeams.OrderBy(t => t.Wins).ThenBy(t => t.Losses))
                writer.WriteLine($"{t.name} ({t.Wins}-{t.Losses})");
        }

        public void DisplaySwissMatchLog(StreamWriter writer)
        {
            writer.WriteLine("\n--- SWISS MATCH LOG ---");
            foreach (var entry in matchLog)
                writer.WriteLine(entry);
        }

        public void SwissPlacement()
        {
            foreach (Team team in EliminatedTeams)
            {
                if (team.Wins == 0) team.Seed = 15;
                else if (team.Wins == 1) team.Seed = 12;
                else team.Seed = 9;
            }
        }

        // =========================
        // MatchKey for rematch detection
        // =========================
        private readonly struct MatchKey : IEquatable<MatchKey>
        {
            private readonly Team A;
            private readonly Team B;

            public MatchKey(Team t1, Team t2)
            {
                if (string.CompareOrdinal(t1.name, t2.name) <= 0)
                {
                    A = t1; B = t2;
                }
                else
                {
                    A = t2; B = t1;
                }
            }

            public bool Equals(MatchKey other) =>
                ReferenceEquals(A, other.A) && ReferenceEquals(B, other.B);

            public override bool Equals(object obj) =>
                obj is MatchKey other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    int h1 = A?.GetHashCode() ?? 0;
                    int h2 = B?.GetHashCode() ?? 0;
                    return (h1 * 397) ^ h2;
                }
            }
        }
    }
}