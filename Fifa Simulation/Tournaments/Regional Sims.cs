using Fifa_Simulation.Helpers;
using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fifa_Simulation.Tournaments
{
    public class Regional_Sims
    {
        public Regional_Sims() { }

        public List<List<Team>> CreateRandomGroups(List<Team> teams, int groupCount)
        {
            int groupSize = teams.Count / groupCount;

            var shuffled = teams
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            var groups = new List<List<Team>>();

            for (int i = 0; i < groupCount; i++)
            {
                var group = shuffled
                    .Skip(i * groupSize)
                    .Take(groupSize)
                    .ToList();

                for (int j = 0; j < group.Count; j++)
                    group[j].SourceGroup = i;

                groups.Add(group);
            }

            return groups;
        }

        public List<List<Team>> CreateSeededGroup(List<Team> teams, int groupCount)
        {
            var orderedTeams = teams
                .OrderByDescending(t => t.RegionPoints)
                .ThenByDescending(t => t.elo)
                .ToList();

            var groups = new List<List<Team>>();

            for (int i = 0; i < groupCount; i++)
                groups.Add(new List<Team>());

            int direction = 1;
            int currentGroup = 0;

            foreach (var team in orderedTeams)
            {
                groups[currentGroup].Add(team);
                team.SourceGroup = currentGroup;

                if (direction == 1)
                {
                    if (currentGroup == groups.Count - 1)
                        direction = -1;
                    else
                        currentGroup++;
                }
                else
                {
                    if (currentGroup == 0)
                        direction = 1;
                    else
                        currentGroup--;
                }
            }

            return groups;
        }

        public void Simulate64TeamRegion(List<Team> teams, int simNumber, string regionName, StreamWriter writer)
        {
            SimulateGroupedRegion(
                teams,
                simNumber,
                groupCount: 8,
                advancePerGroup: 2,
                placementFromGroupFinish: finish =>
                {
                    if (finish <= 2) return 0;   // advances
                    if (finish <= 4) return 48;  // 33-48
                    return 64;                   // 49-64
                },
                awardPoints: (team, sim) => PointsAwarder.Award64TeamRegionPoints(team, sim),
                regionName: regionName,
                writer: writer
            );
        }

        public void Simulate32TeamRegion(List<Team> teams, int simNumber, string regionName, StreamWriter writer)
        {
            SimulateGroupedRegion(
                teams,
                simNumber,
                groupCount: 4,
                advancePerGroup: 2,
                placementFromGroupFinish: finish =>
                {
                    if (finish <= 2) return 0;   // advances
                    if (finish <= 4) return 24;  // 17-24
                    return 32;                   // 25-32
                },
                awardPoints: (team, sim) => PointsAwarder.Award32TeamRegionPoints(team, sim),
                regionName: regionName,
                writer: writer
            );
        }

        public void Simulate16TeamRegion(List<Team> teams, int simNumber, string regionName, StreamWriter writer)
        {
            SimulateGroupedRegion(
                teams,
                simNumber,
                groupCount: 4,
                advancePerGroup: 2,
                placementFromGroupFinish: finish =>
                {
                    if (finish <= 2) return 0;   // advances
                    if (finish == 3) return 12;  // 9-12
                    return 16;                   // 13-16
                },
                awardPoints: (team, sim) => PointsAwarder.Award16TeamRegionPoints(team, sim),
                regionName: regionName,
                writer: writer
            );
        }

        public void Simulate8TeamRegion(List<Team> teams, int simNumber, string regionName, StreamWriter writer)
        {
            SimulateGroupedRegion(
                teams,
                simNumber,
                groupCount: 2,
                advancePerGroup: 2,
                placementFromGroupFinish: finish =>
                {
                    if (finish <= 2) return 0; // advances
                    if (finish == 3) return 6; // 5-6
                    return 8;                  // 7-8
                },
                awardPoints: (team, sim) => PointsAwarder.Award8TeamRegionPoints(team, sim),
                regionName: regionName,
                writer: writer
            );
        }

        public void Simulate4TeamRegion(List<Team> teams, int simNumber, string regionName, StreamWriter writer)
        {
            if (teams == null) throw new ArgumentNullException(nameof(teams));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (teams.Count != 4) throw new ArgumentException("4-team region requires exactly 4 teams.");

            ResetEventStats(teams);

            writer.WriteLine($"\n==================================================");
            writer.WriteLine($"{regionName.ToUpper()} - REGIONAL EVENT {simNumber + 1}");
            writer.WriteLine("==================================================");

            var h2h = new HeadToHead();

            // Round robin group
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    PlayMatch(teams[i], teams[j], h2h);
                }
            }

            var standings = GetStandingsWithH2H(teams, h2h);

            PrintGroupStandings(standings, 1, writer);

            // 3rd and 4th locked from group standings
            standings[2].Seed = 3;
            standings[3].Seed = 4;

            // Final between top 2
            writer.WriteLine($"\n=== {regionName.ToUpper()} FINAL ===");

            Team finalistA = standings[0];
            Team finalistB = standings[1];

            Team winner = new Match(finalistA, finalistB).Play();
            Team loser = winner == finalistA ? finalistB : finalistA;

            winner.Seed = 1;
            loser.Seed = 2;

            writer.WriteLine($"{finalistA.name} vs {finalistB.name} --- Winner: {winner.name}");

            foreach (var team in teams)
            {
                PointsAwarder.Award4TeamRegionPoints(team, simNumber);

                if (team.RegionalFinishes != null && simNumber < team.RegionalFinishes.Length)
                    team.RegionalFinishes[simNumber] = team.Seed;
            }

            PrintRegionalPoints(teams, regionName, writer);
        }

        public List<Team> RunRegionThreeTimes(
            List<Team> teams,
            StreamWriter writer,
            bool resetPoints = true)
        {
            if (teams == null) throw new ArgumentNullException(nameof(teams));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (teams.Count == 0) throw new ArgumentException("Team list cannot be empty.", nameof(teams));

            string regionName;
            int majorSpots;

            string firstTeam = teams[0].name;

            switch (teams.Count)
            {
                case 64:
                    regionName = "Western Europe";
                    majorSpots = 10;
                    break;

                case 32:
                    regionName = "Central Europe";
                    majorSpots = 6;
                    break;

                case 16:
                    if (firstTeam == "Inter Milan")
                    {
                        regionName = "Italy";
                        majorSpots = 4;
                    }
                    else if (firstTeam == "Shakhtar Donetsk")
                    {
                        regionName = "Eastern Europe";
                        majorSpots = 3;
                    }
                    else if (firstTeam == "Palmeiras")
                    {
                        regionName = "South America";
                        majorSpots = 2;
                    }
                    else
                    {
                        throw new ArgumentException("Unknown 16-team region list.");
                    }
                    break;

                case 8:
                    if (firstTeam == "Club America")
                    {
                        regionName = "Americas";
                        majorSpots = 2;
                    }
                    else if (firstTeam == "Kawasaki Frontale")
                    {
                        regionName = "East Asia";
                        majorSpots = 1;
                    }
                    else if (firstTeam == "Adidas All Stars")
                    {
                        regionName = "Wildcard";
                        majorSpots = 2;
                    }
                    else
                    {
                        throw new ArgumentException("Unknown 8-team region list.");
                    }
                    break;

                case 4:
                    if (firstTeam == "Al Hilal")
                    {
                        regionName = "Middle East";
                        majorSpots = 1;
                    }
                    else if (firstTeam == "Al Ahly")
                    {
                        regionName = "Africa";
                        majorSpots = 1;
                    }
                    else
                    {
                        throw new ArgumentException("Unknown 4-team region list.");
                    }
                    break;

                default:
                    throw new ArgumentException($"Unsupported region size: {teams.Count}");
            }

            if (resetPoints)
                ResetRegionPoints(teams);

            for (int sim = 0; sim < 3; sim++)
            {
                switch (teams.Count)
                {
                    case 64:
                        Simulate64TeamRegion(teams, sim, regionName, writer);
                        break;

                    case 32:
                        Simulate32TeamRegion(teams, sim, regionName, writer);
                        break;

                    case 16:
                        Simulate16TeamRegion(teams, sim, regionName, writer);
                        break;

                    case 8:
                        Simulate8TeamRegion(teams, sim, regionName, writer);
                        break;

                    case 4:
                        Simulate4TeamRegion(teams, sim, regionName, writer);
                        break;
                }
            }

            var qualifiers = majorSpots > 0
                ? GetQualifiedTeams(teams, majorSpots)
                : new List<Team>();

            PrintFinalRegionalStandings(teams, regionName, majorSpots, writer);

            return qualifiers;
        }

        private void SimulateGroupedRegion(
            List<Team> teams,
            int simNumber,
            int groupCount,
            int advancePerGroup,
            Func<int, int> placementFromGroupFinish,
            Action<Team, int> awardPoints,
            string regionName,
            StreamWriter writer)
        {
            if (teams == null) throw new ArgumentNullException(nameof(teams));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (teams.Count % groupCount != 0) throw new ArgumentException("Team count must divide evenly into groups.");

            ResetEventStats(teams);

            writer.WriteLine($"\n==================================================");
            writer.WriteLine($"{regionName.ToUpper()} - REGIONAL EVENT {simNumber + 1}");
            writer.WriteLine("==================================================");

            List<List<Team>> groups = simNumber == 0
                ? CreateRandomGroups(teams, groupCount)
                : CreateSeededGroup(teams, groupCount);

            var advancingTeams = new List<Team>();
            var allTeams = new List<Team>();

            for (int g = 0; g < groups.Count; g++)
            {
                var group = groups[g];
                var groupH2H = new HeadToHead();

                for (int i = 0; i < group.Count; i++)
                {
                    for (int j = i + 1; j < group.Count; j++)
                    {
                        PlayMatch(group[i], group[j], groupH2H);
                    }
                }

                var standings = GetStandingsWithH2H(group, groupH2H);

                for (int i = 0; i < standings.Count; i++)
                {
                    int finish = i + 1;

                    if (finish <= advancePerGroup)
                    {
                        advancingTeams.Add(standings[i]);
                    }
                    else
                    {
                        standings[i].Seed = placementFromGroupFinish(finish);
                    }
                }

                PrintGroupStandings(standings, g + 1, writer);
                allTeams.AddRange(standings);
            }

            if (advancingTeams.Count > 1)
            {
                writer.WriteLine($"\n=== {regionName.ToUpper()} PLAYOFF BRACKET ===");

                var elim = new SingleElimination(advancingTeams, reseedBeforeBracket: true);
                elim.Run(writer);
            }

            foreach (var team in allTeams)
            {
                awardPoints(team, simNumber);

                if (team.RegionalFinishes != null && simNumber < team.RegionalFinishes.Length)
                    team.RegionalFinishes[simNumber] = team.Seed;
            }

            if (simNumber != 3)
                PrintRegionalPoints(teams, regionName, writer);
        }

        private static void ResetEventStats(List<Team> teams)
        {
            foreach (var team in teams)
                team.resetRecord();
        }

        private static void ResetRegionPoints(List<Team> teams)
        {
            foreach (var team in teams)
            {
                team.RegionPoints = 0;
                team.Seed = 0;
                team.SourceGroup = -1;
                team.resetRecord();
                team.SwissOpponents.Clear();

                if (team.RegionalFinishes != null)
                    Array.Clear(team.RegionalFinishes, 0, team.RegionalFinishes.Length);
            }
        }

        private static void PlayMatch(Team a, Team b, HeadToHead groupH2H)
        {
            Team winner = new Match(a, b).Play();
            Team loser = winner == a ? b : a;

            groupH2H.RecordWin(winner, loser);
        }

        private static List<Team> GetStandingsWithH2H(List<Team> teams, HeadToHead h2h)
        {
            var standings = teams.ToList();

            standings.Sort((a, b) =>
            {
                int cmp = b.Wins.CompareTo(a.Wins);
                if (cmp != 0) return cmp;

                int aOverB = h2h.GetWins(a, b);
                int bOverA = h2h.GetWins(b, a);
                if (aOverB != bOverA)
                    return bOverA.CompareTo(aOverB);

                return b.elo.CompareTo(a.elo);
            });

            return standings;
        }

        private static List<Team> GetQualifiedTeams(List<Team> teams, int majorSpots)
        {
            return teams
                .OrderBy(t => t, Comparer<Team>.Create(CompareRegionalRanking))
                .Take(majorSpots)
                .ToList();
        }

        private static int CompareRegionalRanking(Team a, Team b)
        {
            int cmp = b.RegionPoints.CompareTo(a.RegionPoints);
            if (cmp != 0) return cmp;

            int[] aFinishes = GetSortedFinishes(a);
            int[] bFinishes = GetSortedFinishes(b);

            for (int i = 0; i < Math.Min(aFinishes.Length, bFinishes.Length); i++)
            {
                cmp = aFinishes[i].CompareTo(bFinishes[i]); // lower finish is better
                if (cmp != 0) return cmp;
            }

            cmp = b.TotalPoints.CompareTo(a.TotalPoints);
            if (cmp != 0) return cmp;

            cmp = b.elo.CompareTo(a.elo);
            if (cmp != 0) return cmp;

            return string.Compare(a.name, b.name, StringComparison.Ordinal);
        }

        private static int[] GetSortedFinishes(Team team)
        {
            if (team.RegionalFinishes == null || team.RegionalFinishes.Length == 0)
                return Array.Empty<int>();

            return team.RegionalFinishes
                .Select(f => f == 0 ? int.MaxValue : f)
                .OrderBy(f => f)
                .ToArray();
        }

        private void PrintGroupStandings(List<Team> standings, int groupNumber, TextWriter writer)
        {
            writer.WriteLine($"\n--- GROUP {groupNumber} FINAL STANDINGS ---");

            for (int i = 0; i < standings.Count; i++)
            {
                var t = standings[i];
                writer.WriteLine($"{i + 1}. {t.name}  W:{t.Wins} L:{t.Losses}");
            }
        }

        private void PrintRegionalPoints(List<Team> teams, string regionName, TextWriter writer)
        { 
            writer.WriteLine($"\n=== {regionName.ToUpper()} REGIONAL POINTS ===");

            var ordered = teams
                .OrderBy(t => t, Comparer<Team>.Create(CompareRegionalRanking))
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                var t = ordered[i];
                string finishes = t.RegionalFinishes == null
                    ? "-"
                    : string.Join(", ", t.RegionalFinishes.Select(f => f == 0 ? "-" : f.ToString()));

                writer.WriteLine(
                    $"{i + 1}. {t.name}  RegionPts:{t.RegionPoints}  TotalPts:{t.TotalPoints}  Finishes:[{finishes}]"
                );
            }
        }

        private void PrintFinalRegionalStandings(List<Team> teams, string regionName, int majorSpots, TextWriter writer)
        {
            writer.WriteLine($"\n==================================================");
            writer.WriteLine($"{regionName.ToUpper()} - FINAL REGIONAL STANDINGS");
            writer.WriteLine("==================================================");

            var ordered = teams
                .OrderBy(t => t, Comparer<Team>.Create(CompareRegionalRanking))
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                var t = ordered[i];
                string marker = i < majorSpots ? "[QUALIFIED]" : "           ";
                string finishes = t.RegionalFinishes == null
                    ? "-"
                    : string.Join(", ", t.RegionalFinishes.Select(f => f == 0 ? "-" : f.ToString()));

                writer.WriteLine(
                    $"{i + 1}. {marker} {t.name}  RegionPts:{t.RegionPoints}  TotalPts:{t.TotalPoints}  Finishes:[{finishes}]"
                );
            }
        }
    }
}