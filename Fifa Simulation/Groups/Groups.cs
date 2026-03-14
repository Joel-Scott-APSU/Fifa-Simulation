using Fifa_Simulation.Helpers;
using Fifa_Simulation.Teams;
using Fifa_Simulation.Tournaments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fifa_Simulation.Groups
{
    public class Groups32
    {
        private List<Team> qualifiedTeams;
        private readonly List<List<Team>> groupsOf4;

        public Groups32(List<InitialRoundRobin> initialGroups)
        {
            if (initialGroups == null)
                throw new ArgumentNullException(nameof(initialGroups));

            qualifiedTeams = initialGroups
                .SelectMany(g => g.GetStandings().Take(4))
                .ToList();

            if (qualifiedTeams.Count != 32)
                throw new InvalidOperationException($"Expected 32 qualified teams, got {qualifiedTeams.Count}.");

            qualifiedTeams = qualifiedTeams
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenByDescending(t => t.Points)
                .ThenByDescending(t => t.elo)
                .ToList();

            for (int i = 0; i < qualifiedTeams.Count; i++)
                qualifiedTeams[i].Seed = i + 1;

            groupsOf4 = BuildSeededGroupsWithPots(qualifiedTeams);
        }

        public void RunAndFinish(StreamWriter writer, int sim)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var advancingTeams = new List<Team>();
            var all32 = new List<Team>();

            var groupWinners = new Team[8];
            var groupRunners = new Team[8];
            var incomingSeed = new Dictionary<Team, int>();

            writer.WriteLine("\n================ 32-TEAM MAJOR GROUP STAGE ================");

            for (int gi = 0; gi < groupsOf4.Count; gi++)
            {
                var group = groupsOf4[gi];
                string groupName = ((char)('A' + gi)).ToString();
                var h2h = new HeadToHead();

                foreach (var t in group)
                {
                    if (!incomingSeed.ContainsKey(t))
                        incomingSeed[t] = t.Seed;

                    t.resetRecord();
                }

                writer.WriteLine($"\n=== GROUP {groupName} ===");
                PrintGroupRoster(group, writer);

                for (int i = 0; i < group.Count; i++)
                {
                    for (int j = i + 1; j < group.Count; j++)
                    {
                        PlaySingleMatch(group[i], group[j], writer, h2h);
                    }
                }

                var standings = GetStandingsWithH2H(group, h2h);
                PrintStandings(groupName, standings, writer);

                groupWinners[gi] = standings[0];
                groupRunners[gi] = standings[1];

                advancingTeams.Add(standings[0]);
                advancingTeams.Add(standings[1]);

                // Group-stage non-qualifiers
                standings[2].Seed = 24; // 17-24
                standings[3].Seed = 32; // 25-32

                all32.AddRange(standings);
            }

            var rankedWinners = groupWinners
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenBy(t => incomingSeed[t])
                .ToList();

            var rankedRunners = groupRunners
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenBy(t => incomingSeed[t])
                .ToList();

            writer.WriteLine("\n================ GROUP WINNERS RANKING ================");
            for (int i = 0; i < rankedWinners.Count; i++)
                writer.WriteLine($"{i + 1}. {rankedWinners[i].name} (Incoming Seed {incomingSeed[rankedWinners[i]]})");

            writer.WriteLine("\n================ GROUP RUNNERS-UP RANKING ================");
            for (int i = 0; i < rankedRunners.Count; i++)
                writer.WriteLine($"{i + 1}. {rankedRunners[i].name} (Incoming Seed {incomingSeed[rankedRunners[i]]})");

            foreach (var t in advancingTeams)
                t.resetRecord();

            writer.WriteLine("\n================ 16-TEAM SINGLE ELIM STAGE ================");

            // Fixed Round of 16 mapping:
            // A1 vs H2, B1 vs G2, C1 vs F2, D1 vs E2, E1 vs D2, F1 vs C2, G1 vs B2, H1 vs A2
            var r16Slots = new List<Team>
            {
                groupWinners[0], groupRunners[7],
                groupWinners[1], groupRunners[6],
                groupWinners[2], groupRunners[5],
                groupWinners[3], groupRunners[4],
                groupWinners[4], groupRunners[3],
                groupWinners[5], groupRunners[2],
                groupWinners[6], groupRunners[1],
                groupWinners[7], groupRunners[0]
            };

            // Lock bracket path
            for (int i = 0; i < r16Slots.Count; i++)
                r16Slots[i].Seed = i + 1;

            var singleElim = new SingleElimination(r16Slots, reseedBeforeBracket: false);
            Team champ = singleElim.Run(writer);

            if (champ != null)
                champ.Seed = 1;

            foreach (var t in all32)
                PointsAwarder.Award32TeamRegionPoints(t, sim);
        }

        private static List<List<Team>> BuildSeededGroupsWithPots(List<Team> rankedTop32, bool shuffleWithinPots = false)
        {
            var ordered = rankedTop32
                .OrderBy(t => t.Seed)
                .ToList();

            var pots = new List<List<Team>>
            {
                ordered.Take(8).ToList(),
                ordered.Skip(8).Take(8).ToList(),
                ordered.Skip(16).Take(8).ToList(),
                ordered.Skip(24).Take(8).ToList()
            };

            if (shuffleWithinPots)
            {
                var rng = new Random();
                for (int i = 0; i < pots.Count; i++)
                    pots[i] = pots[i].OrderBy(_ => rng.Next()).ToList();
            }

            var groups = new List<List<Team>>();
            for (int i = 0; i < 8; i++)
                groups.Add(new List<Team>(4));

            // Pot 1: A B C D E F G H
            for (int i = 0; i < 8; i++)
                groups[i].Add(pots[0][i]);

            // Pot 2: H G F E D C B A
            for (int i = 0; i < 8; i++)
                groups[7 - i].Add(pots[1][i]);

            // Pot 3: A B C D E F G H
            for (int i = 0; i < 8; i++)
                groups[i].Add(pots[2][i]);

            // Pot 4: H G F E D C B A
            for (int i = 0; i < 8; i++)
                groups[7 - i].Add(pots[3][i]);

            return groups;
        }

        private static void PlaySingleMatch(Team a, Team b, TextWriter writer, HeadToHead h2h)
        {
            Team winner = new Match(a, b).Play();
            Team loser = winner == a ? b : a;

            h2h.RecordWin(winner, loser);
            writer.WriteLine($"{a.name} vs {b.name} --- {winner.name}");
        }

        private static List<Team> GetStandingsWithH2H(List<Team> group, HeadToHead h2h)
        {
            var list = group.ToList();

            list.Sort((a, b) =>
            {
                int cmp = b.Wins.CompareTo(a.Wins);
                if (cmp != 0) return cmp;

                int aOverB = h2h.GetWins(a, b);
                int bOverA = h2h.GetWins(b, a);
                if (aOverB != bOverA)
                    return bOverA.CompareTo(aOverB);

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

        public List<List<Team>> GetGroups()
        {
            return groupsOf4.Select(g => g.ToList()).ToList();
        }
    }
}