using Fifa_Simulation.Helpers;
using Fifa_Simulation.Teams;
using Fifa_Simulation.Tournaments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fifa_Simulation.Groups
{
    public class Groups
    {
        private readonly HeadToHead h2h = new();

        // The reseeded top16
        private readonly List<Team> top16;

        // 4 new groups of 4 teams each
        private readonly List<List<Team>> groupsOf4;

        public Groups(List<InitialRoundRobin> initialGroups)
        {
            if (initialGroups == null) throw new ArgumentNullException(nameof(initialGroups));

            // 1) Build top16: top 4 from each initial group
            top16 = initialGroups
                .SelectMany(g => g.GetStandings().Take(4))
                .ToList();

            if (top16.Count != 16)
                throw new InvalidOperationException($"Expected 16 qualified teams, got {top16.Count}. Check your group sizes / Take(4).");

            // 2) Rank top16 for new seeding (record then Elo; add more tiebreakers if you want)
            top16 = top16
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenByDescending(t => t.elo)
                .ToList();

            // 3) Assign seeds 1..16 for reseeding
            for (int i = 0; i < top16.Count; i++)
                top16[i].Seed = i + 1;

            // 4) Create balanced groups of 4
            groupsOf4 = BuildSeededGroupsWithPots(top16);
        }


        public void RunAndFinish(StreamWriter writer, int sim)
        {
            var top8 = new List<Team>();
            var all16 = new List<Team>();

            var groupWinners = new Team[4];
            var groupRunners = new Team[4];

            // Snapshot the "seed coming in" before any elimination/points seed overwrites.
            // (This is important because later you set Seed=9/16 for points.)
            var incomingSeed = new Dictionary<Team, int>();

            writer.WriteLine("\n================ RESEEDED GROUP STAGE ================");

            for (int gi = 0; gi < groupsOf4.Count; gi++)
            {
                var group = groupsOf4[gi];
                string groupName = ((char)('A' + gi)).ToString();

                // record incoming seeds once (these are the reseed 1..16 values)
                foreach (var t in group)
                    if (!incomingSeed.ContainsKey(t))
                        incomingSeed[t] = t.Seed;

                // reset for this stage
                foreach (var t in group)
                    t.resetRecord();

                writer.WriteLine($"\n=== GROUP {groupName} ===");

                // single round robin
                for (int i = 0; i < group.Count; i++)
                {
                    for (int j = i + 1; j < group.Count; j++)
                        PlaySingleMatch(group[i], group[j], writer);
                }

                var standings = GetStandingsWithH2H(group);
                PrintStandings(groupName, standings, writer);

                // Save winner/runner for fixed QF mapping later
                groupWinners[gi] = standings[0];
                groupRunners[gi] = standings[1];

                // Top 2 advance
                top8.Add(standings[0]);
                top8.Add(standings[1]);

                // Group placements for points:
                standings[2].Seed = 9;   // 3rd -> Top9 points
                standings[3].Seed = 16;  // 4th -> Top16 points

                all16.AddRange(standings);
            }

            // Rank group winners 1..4 by (group record) then (incoming seed)
            // This ranking isn't required for your fixed mapping, but you asked for it.
            var rankedWinners = groupWinners
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenBy(t => incomingSeed[t])  // seed coming in
                .ToList();

            for (int i = 0; i < rankedWinners.Count; i++)
            {
                var t = rankedWinners[i];
            }

            // Reset records for the knockout stage (so bracket isn't affected by group W/L)
            foreach (var t in top8)
                t.resetRecord();

            // Quarterfinals with your fixed cross-group mapping:
            // A1 vs D2, B1 vs C2, C1 vs B2, D1 vs A2
            writer.WriteLine("\n================ TOP 8 SINGLE ELIM ================");

            var qfPairings = new List<(Team A, Team B)>
    {
        (groupWinners[0], groupRunners[3]), // A1 vs D2
        (groupWinners[1], groupRunners[2]), // B1 vs C2
        (groupWinners[2], groupRunners[1]), // C1 vs B2
        (groupWinners[3], groupRunners[0])  // D1 vs A2
    };

            var qfWinners = new List<Team>();

            foreach (var (a, b) in qfPairings)
            {
                var winner = new Match(a, b).Play();
                var loser = (winner == a) ? b : a;

                writer.WriteLine($"{a.name} vs {b.name} --- Winner {winner.name}");

                // QF losers are Top8
                loser.Seed = 8;

                qfWinners.Add(winner);
            }

            // Now finish with your existing SingleElimination on the 4 winners (semis/finals)
            foreach (var t in qfWinners)
                t.resetRecord();

            var elim = new SingleElimination(qfWinners);
            Team champ = elim.Run(writer);

            if (champ != null)
                champ.Seed = 1; // ensure winner gets winner points

            // Award points to everyone in this stage
            foreach (var t in all16)
                PointsAwarder.AwardPoints(t, sim);
        }

        /// <summary>
        /// Your linear "representation" builder: each new group tries to include unique SourceGroup values.
        /// Assumes teams are already seeded 1..16 and SourceGroup is set.
        /// </summary>
        private static List<List<Team>> BuildSeededGroupsWithPots(List<Team> rankedTop16, bool shuffleWithinPots = false)
        {
            var ordered = rankedTop16.OrderBy(t => t.Seed).ToList(); // Seed 1..16

            var pot1 = ordered.Take(4).ToList();          // 1-4
            var pot2 = ordered.Skip(4).Take(4).ToList();  // 5-8
            var pot3 = ordered.Skip(8).Take(4).ToList();  // 9-12
            var pot4 = ordered.Skip(12).Take(4).ToList(); // 13-16

            if (shuffleWithinPots)
            {
                var rng = new Random();
                pot1 = pot1.OrderBy(_ => rng.Next()).ToList();
                pot2 = pot2.OrderBy(_ => rng.Next()).ToList();
                pot3 = pot3.OrderBy(_ => rng.Next()).ToList();
                pot4 = pot4.OrderBy(_ => rng.Next()).ToList();
            }

            var groupA = new List<Team>(4);
            var groupB = new List<Team>(4);
            var groupC = new List<Team>(4);
            var groupD = new List<Team>(4);

            // Snake distribution:
            // Pot1: A B C D
            groupA.Add(pot1[0]); groupB.Add(pot1[1]); groupC.Add(pot1[2]); groupD.Add(pot1[3]);

            // Pot2: D C B A
            groupD.Add(pot2[0]); groupC.Add(pot2[1]); groupB.Add(pot2[2]); groupA.Add(pot2[3]);

            // Pot3: A B C D
            groupA.Add(pot3[0]); groupB.Add(pot3[1]); groupC.Add(pot3[2]); groupD.Add(pot3[3]);

            // Pot4: D C B A
            groupD.Add(pot4[0]); groupC.Add(pot4[1]); groupB.Add(pot4[2]); groupA.Add(pot4[3]);

            return new List<List<Team>> { groupA, groupB, groupC, groupD };
        }

        private void PlaySingleMatch(Team a, Team b, TextWriter writer)
        {
            Team winner = new Match(a, b).Play();
            Team loser = winner == a ? b : a;

            h2h.RecordWin(winner, loser);
            writer.WriteLine($"{a.name} vs {b.name} --- {winner.name}");
        }

        /// <summary>
        /// Standings tie-break: Wins desc -> H2H -> Seed asc
        /// Note: H2H is best for 2-team ties; for 3+ way ties it’s a simple pairwise tie-break.
        /// </summary>
        private List<Team> GetStandingsWithH2H(List<Team> group)
        {
            var list = group.ToList();

            list.Sort((a, b) =>
            {
                // 1) Wins
                int cmp = b.Wins.CompareTo(a.Wins);
                if (cmp != 0) return cmp;

                // 2) Head-to-head (a vs b)
                int aOverB = h2h.GetWins(a, b);
                int bOverA = h2h.GetWins(b, a);
                if (aOverB != bOverA)
                    return bOverA.CompareTo(aOverB); // more H2H wins ranks higher

                // 3) Seed coming in (lower is better)
                return a.Seed.CompareTo(b.Seed);
            });

            return list;
        }

        private static void PrintGroupRoster(List<Team> group, TextWriter writer)
        {
            writer.WriteLine("Teams:");
            foreach (var t in group.OrderBy(t => t.Seed))
                writer.WriteLine($" - Seed {t.Seed}: {t.name} (Src:{t.SourceGroup}, Elo:{t.elo})");
            writer.WriteLine();
        }

        private static void PrintStandings(string groupName, List<Team> standings, TextWriter writer)
        {
            writer.WriteLine($"\n--- GROUP {groupName} STANDINGS (W -> H2H -> Seed) ---");

            for (int i = 0; i < standings.Count; i++)
            {
                var t = standings[i];
                writer.WriteLine($"{i + 1}. {t.name,-25}  W:{t.Wins}  L:{t.Losses}  Seed:{t.Seed}");
            }
        }

        // Optional: expose groups if you want to use them later
        public List<List<Team>> GetGroups() => groupsOf4.Select(g => g.ToList()).ToList();
    }
}