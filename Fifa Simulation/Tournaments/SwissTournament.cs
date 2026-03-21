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
        private readonly HashSet<MatchKey> played = new();
        private readonly Dictionary<Team, int> swissIncomingSeed = new();

        public SwissTournament(List<Team> seededTeams)
        {
            if (seededTeams == null)
                throw new ArgumentNullException(nameof(seededTeams));

            if (seededTeams.Count != 32)
                throw new ArgumentException("SwissTournament expects 32 teams for Major 1.", nameof(seededTeams));

            ActiveTeams = new List<Team>(seededTeams);

            foreach (var t in ActiveTeams)
            {
                swissIncomingSeed[t] = t.Seed;
                t.SwissOpponents.Clear();
                t.Wins = 0;
                t.Losses = 0;
            }
        }

        /// <summary>
        /// Runs 32-team Swiss:
        /// - 3 wins = advance
        /// - 3 losses = eliminate
        /// Returns top 16 reseeded for single elimination.
        /// </summary>
        public List<Team> Run()
        {
            for (int round = 1; round <= 5 && AdvancedTeams.Count < 16; round++)
            {
                var recordGroups = ActiveTeams
                    .GroupBy(t => (t.Wins, t.Losses))
                    .OrderByDescending(g => g.Key.Wins)
                    .ThenBy(g => g.Key.Losses)
                    .Select(g => g.OrderBy(t => t.Seed).ToList())
                    .ToList();

                foreach (var group in recordGroups)
                {
                    if (group.Count < 2)
                        continue;

                    var pairings = PairBySeedAvoidRematches(group);
                    RunRound(pairings, round);
                }
            }

            ResolveTeams();
            SwissPlacement();
            return GetReseededTop16();
        }

        private List<Team> GetReseededTop16()
        {
            var candidates = AdvancedTeams.Count >= 16
                ? AdvancedTeams.ToList()
                : AdvancedTeams.Concat(ActiveTeams).ToList();

            var sos = candidates.ToDictionary(t => t, ComputeSoS);
            var oppAvgSeed = candidates.ToDictionary(t => t, ComputeOpponentAvgIncomingSeed);

            var ordered = candidates
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenByDescending(t => sos[t])
                .ThenBy(t => oppAvgSeed[t])
                .ThenBy(t => swissIncomingSeed.TryGetValue(t, out var s) ? s : int.MaxValue)
                .ThenByDescending(t => t.elo)
                .ToList();

            for (int i = 0; i < ordered.Count - 1; i++)
            {
                var a = ordered[i];
                var b = ordered[i + 1];

                bool tied =
                    a.Wins == b.Wins &&
                    a.Losses == b.Losses &&
                    sos[a] == sos[b];

                if (!tied)
                    continue;

                int aOverB = h2h.GetWins(a, b);
                int bOverA = h2h.GetWins(b, a);

                if (bOverA > aOverB)
                {
                    ordered[i] = b;
                    ordered[i + 1] = a;

                    if (i > 0)
                        i -= 2;
                }
            }

            var top16 = ordered.Take(16).ToList();

            for (int i = 0; i < top16.Count; i++)
                top16[i].Seed = i + 1;

            return top16;
        }

        private int ComputeSoS(Team team)
        {
            int total = 0;
            foreach (var opp in team.SwissOpponents)
                total += (opp.Wins - opp.Losses);

            return total;
        }

        private double ComputeOpponentAvgIncomingSeed(Team team)
        {
            if (team.SwissOpponents.Count == 0)
                return double.PositiveInfinity;

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
                matchLog.Add($"R{round}: Seed {a.Seed} ({a.Wins}-{a.Losses}) vs Seed {b.Seed} ({b.Wins}-{b.Losses})");

                played.Add(new MatchKey(a, b));

                Team winner = new Match(a, b).Play();
                Team loser = winner == a ? b : a;

                a.SwissOpponents.Add(b);
                b.SwissOpponents.Add(a);

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
                    if (!AdvancedTeams.Contains(team))
                        AdvancedTeams.Add(team);

                    ActiveTeams.Remove(team);
                }
                else if (team.Losses == 3)
                {
                    if (!EliminatedTeams.Contains(team))
                        EliminatedTeams.Add(team);

                    ActiveTeams.Remove(team);
                }
            }
        }

        private List<(Team, Team)> PairBySeedAvoidRematches(List<Team> teams)
        {
            var ordered = teams.OrderBy(t => t.Seed).ToList();
            var pairings = new List<(Team, Team)>();
            var used = new HashSet<Team>();

            for (int i = 0; i < ordered.Count; i++)
            {
                var a = ordered[i];
                if (used.Contains(a))
                    continue;

                Team opponent = null;

                for (int j = ordered.Count - 1; j > i; j--)
                {
                    var b = ordered[j];
                    if (used.Contains(b))
                        continue;

                    if (!played.Contains(new MatchKey(a, b)))
                    {
                        opponent = b;
                        break;
                    }
                }

                if (opponent == null)
                {
                    for (int j = ordered.Count - 1; j > i; j--)
                    {
                        var b = ordered[j];
                        if (!used.Contains(b))
                        {
                            opponent = b;
                            break;
                        }
                    }
                }

                if (opponent != null)
                {
                    used.Add(a);
                    used.Add(opponent);
                    pairings.Add((a, opponent));
                }
            }

            return pairings;
        }

        public void DisplaySwissResults(StreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteLine("\n==================================================");
            writer.WriteLine("SWISS RESULTS");
            writer.WriteLine("==================================================");

            writer.WriteLine("\n--- ADVANCED FROM SWISS ---");
            foreach (var t in AdvancedTeams
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenBy(t => t.Seed))
            {
                writer.WriteLine($"{t.name} ({t.Wins}-{t.Losses})");
            }

            writer.WriteLine("\n--- ELIMINATED IN SWISS ---");
            foreach (var t in EliminatedTeams
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenBy(t => t.Seed))
            {
                writer.WriteLine($"{t.name} ({t.Wins}-{t.Losses})");
            }

            if (ActiveTeams.Count > 0)
            {
                writer.WriteLine("\n--- STILL ACTIVE / UNRESOLVED ---");
                foreach (var t in ActiveTeams
                    .OrderByDescending(t => t.Wins)
                    .ThenBy(t => t.Losses)
                    .ThenBy(t => t.Seed))
                {
                    writer.WriteLine($"{t.name} ({t.Wins}-{t.Losses})");
                }
            }
        }

        public void DisplaySwissMatchLog(StreamWriter writer)
        {
            writer.WriteLine("\n--- SWISS MATCH LOG ---");
            foreach (var entry in matchLog)
                writer.WriteLine(entry);
        }

        /// <summary>
        /// Placement bands for 32-team major swiss:
        /// 0-3 => 32
        /// 1-3 => 24
        /// 2-3 => 16
        /// </summary>
        public void SwissPlacement()
        {
            foreach (Team team in EliminatedTeams)
            {
                if (team.Wins == 0)
                    team.Seed = 32;
                else if (team.Wins == 1)
                    team.Seed = 24;
                else
                    team.Seed = 16;
            }
        }

        private readonly struct MatchKey : IEquatable<MatchKey>
        {
            private readonly Team A;
            private readonly Team B;

            public MatchKey(Team t1, Team t2)
            {
                if (string.CompareOrdinal(t1.name, t2.name) <= 0)
                {
                    A = t1;
                    B = t2;
                }
                else
                {
                    A = t2;
                    B = t1;
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