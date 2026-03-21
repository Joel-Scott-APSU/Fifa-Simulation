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
        private readonly List<Team> qualifiedTeams;
        private readonly List<List<Team>> groupsOf4;

        public Groups32(List<Team> seededTeams)
        {
            if (seededTeams == null)
                throw new ArgumentNullException(nameof(seededTeams));

            if (seededTeams.Count != 32)
                throw new InvalidOperationException($"Expected 32 seeded teams, got {seededTeams.Count}.");

            qualifiedTeams = seededTeams
                .OrderBy(t => t.Seed)
                .ToList();

            groupsOf4 = BuildSeededGroupsWithPots(qualifiedTeams);
        }

        public List<Team> RunAndFinish(StreamWriter writer, int sim)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var advancingTeams = new List<Team>();
            var thirdPlaceTeams = new List<Team>();
            var fourthPlaceTeams = new List<Team>();

            var groupWinners = new Team[8];
            var groupRunners = new Team[8];
            var incomingSeed = new Dictionary<Team, int>();

            writer.WriteLine("\n================ 32-TEAM MAJOR GROUP STAGE ================");

            for (int gi = 0; gi < groupsOf4.Count; gi++)
            {
                var group = groupsOf4[gi];
                string groupName = ((char)('A' + gi)).ToString();
                var h2h = new HeadToHead();

                foreach (var team in group)
                {
                    if (!incomingSeed.ContainsKey(team))
                        incomingSeed[team] = team.Seed;

                    team.resetRecord();
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
                thirdPlaceTeams.Add(standings[2]);
                fourthPlaceTeams.Add(standings[3]);
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

            foreach (var team in advancingTeams)
                team.resetRecord();

            writer.WriteLine("\n================ 16-TEAM SINGLE ELIM STAGE ================");

            // Locked bracket path:
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

            for (int i = 0; i < r16Slots.Count; i++)
                r16Slots[i].Seed = i + 1;

            var singleElim = new SingleElimination(r16Slots, reseedBeforeBracket: false);
            singleElim.Run(writer);

            var playoffOrder = singleElim.GetOrderedFinish();

            var orderedThirds = thirdPlaceTeams
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenBy(t => incomingSeed[t])
                .ToList();

            var orderedFourths = fourthPlaceTeams
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenBy(t => incomingSeed[t])
                .ToList();

            var finalOrder = new List<Team>();
            finalOrder.AddRange(playoffOrder);
            finalOrder.AddRange(orderedThirds);
            finalOrder.AddRange(orderedFourths);

            for (int i = 0; i < finalOrder.Count; i++)
                finalOrder[i].Seed = i + 1;

            foreach (var team in finalOrder)
                PointsAwarder.Award32TeamRegionPoints(team, sim);

            return finalOrder;
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
            foreach (var team in group.OrderBy(t => t.Seed))
                writer.WriteLine($" - Seed {team.Seed}: {team.name} (Region:{team.Region}, Elo:{team.elo})");

            writer.WriteLine();
        }

        private static void PrintStandings(string groupName, List<Team> standings, TextWriter writer)
        {
            writer.WriteLine($"\n--- GROUP {groupName} STANDINGS (W -> H2H -> Seed) ---");

            for (int i = 0; i < standings.Count; i++)
            {
                var team = standings[i];
                writer.WriteLine($"{i + 1}. {team.name,-25}  W:{team.Wins}  L:{team.Losses}  Seed:{team.Seed}");
            }
        }

        public List<List<Team>> GetGroups()
        {
            return groupsOf4.Select(g => g.ToList()).ToList();
        }
    }
}