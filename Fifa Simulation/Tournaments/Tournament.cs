using Fifa_Simulation.Groups;
using Fifa_Simulation.Helpers;
using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fifa_Simulation.Tournaments
{
    public class Tournament
    {
        private List<Team> Major1Top4 = new();
        private List<Team> Major2Top4 = new();
        private List<Team> Major3Top4 = new();
        private List<Team> WorldsTop4 = new();
        private List<Team> averageMajorFinish = new();
        public Tournament() { }

        public SimulationSnapshot RunThreeSimulations(StreamWriter writer, int simulationNumber)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            List<Team> previousMajorFinalOrder = null;
            List<Team> lastMajorTeams = null;

            for (int sim = 0; sim < 3; sim++)
            {
                writer.WriteLine($"\n================ SIMULATION {sim + 1} ================");

                var regionalQualifiers = RunRegionalSims(writer);

                List<Team> majorTeams = sim == 0
                    ? MajorSeeder.SeedMajorOne(
                        regionalQualifiers.WesternEurope,
                        regionalQualifiers.CentralEurope,
                        regionalQualifiers.Italy,
                        regionalQualifiers.EasternEurope,
                        regionalQualifiers.SouthAmerica,
                        regionalQualifiers.Americas,
                        regionalQualifiers.EastAsia,
                        regionalQualifiers.MiddleEast,
                        regionalQualifiers.Africa,
                        regionalQualifiers.Wildcard)
                    : MajorSeeder.SeedMajorFromPreviousResults(
                        previousMajorFinalOrder,
                        regionalQualifiers.WesternEurope,
                        regionalQualifiers.CentralEurope,
                        regionalQualifiers.Italy,
                        regionalQualifiers.EasternEurope,
                        regionalQualifiers.SouthAmerica,
                        regionalQualifiers.Americas,
                        regionalQualifiers.EastAsia,
                        regionalQualifiers.MiddleEast,
                        regionalQualifiers.Africa,
                        regionalQualifiers.Wildcard);

                PrintMajorSeeds(majorTeams, writer, sim + 1);

                if (sim == 0)
                {
                    previousMajorFinalOrder = RunSwissSingleElimMajor(majorTeams, writer, sim);
                    for (int i = 0; i < previousMajorFinalOrder.Count; i++)
                    {
                        previousMajorFinalOrder[i].majorFinishes[0] = i + 1;
                    }

                    for(int i = 0; i < 4; i++)
                    {
                        Major1Top4.Add(previousMajorFinalOrder[i]);
                    }
                }
                else if (sim == 1)
                {
                    previousMajorFinalOrder = RunDoubleElimMajor(majorTeams, writer, sim);
                    for(int i = 0; i < previousMajorFinalOrder.Count; i++)
                    {
                        previousMajorFinalOrder[i].majorFinishes[1] = i + 1;
                    }
                    for(int i = 0; i < 4; i++)
                    {
                        Major2Top4.Add(previousMajorFinalOrder[i]);
                    }
                }
                else
                {
                    previousMajorFinalOrder = RunGroupSingleElimMajor(majorTeams, writer, sim);
                    for (int i = 0; i < previousMajorFinalOrder.Count; i++)
                    {
                        previousMajorFinalOrder[i].majorFinishes[2] = i + 1;
                    }
                    for(int i = 0; i < 4; i++)
                    {
                        Major3Top4.Add(previousMajorFinalOrder[i]);
                    }
                }

                PrintFinalMajorOrder(previousMajorFinalOrder, writer, sim + 1);

                for (int i = 0; i < previousMajorFinalOrder.Count; i++)
                {
                    previousMajorFinalOrder[i].PreviousMajorFinish = i + 1;
                    previousMajorFinalOrder[i].Seed = i + 1;
                    previousMajorFinalOrder[i].resetRecord();
                }

                lastMajorTeams = previousMajorFinalOrder;
        }

            var worldsTeams = GetWorldsTeams(writer);

            FinalsTournament finalsTournament = new FinalsTournament(worldsTeams);
            WorldsTop4 = finalsTournament.Run(writer);
            writer.Flush();

            return new SimulationSnapshot
            {
                SimulationNumber = simulationNumber,
                Major1Top4 = Major1Top4,
                Major2Top4 = Major2Top4,
                Major3Top4 = Major3Top4,
                WorldsTop4 = WorldsTop4
            };
        }

        private RegionalQualifierSet RunRegionalSims(StreamWriter writer)
        {
            Regional_Sims sims = new Regional_Sims();

            var westernEuropeTeams = TeamFactory.GetWesternEuropeTeams();
            var centralEuropeTeams = TeamFactory.GetCentralEuropeTeams();
            var italyTeams = TeamFactory.GetItalyTeams();
            var easternEuropeTeams = TeamFactory.GetEasternEuropeTeams();
            var southAmericaTeams = TeamFactory.GetSouthAmericaTeams();
            var americasTeams = TeamFactory.GetAmericasTeams();
            var eastAsiaTeams = TeamFactory.GetEastAsiaTeams();
            var middleEastTeams = TeamFactory.GetMiddleEastTeams();
            var africaTeams = TeamFactory.GetAfricaTeams();
            var wildcardTeams = TeamFactory.GetWildcardTeams();

            return new RegionalQualifierSet
            {
                WesternEurope = sims.RunRegionThreeTimes(westernEuropeTeams, writer, true),
                CentralEurope = sims.RunRegionThreeTimes(centralEuropeTeams, writer, true),
                Italy = sims.RunRegionThreeTimes(italyTeams, writer, true),
                EasternEurope = sims.RunRegionThreeTimes(easternEuropeTeams, writer, true),
                SouthAmerica = sims.RunRegionThreeTimes(southAmericaTeams, writer, true),
                Americas = sims.RunRegionThreeTimes(americasTeams, writer, true),
                EastAsia = sims.RunRegionThreeTimes(eastAsiaTeams, writer, true),
                MiddleEast = sims.RunRegionThreeTimes(middleEastTeams, writer, true),
                Africa = sims.RunRegionThreeTimes(africaTeams, writer, true),
                Wildcard = sims.RunRegionThreeTimes(wildcardTeams, writer, true)
            };
        }

        private List<Team> RunSwissSingleElimMajor(List<Team> majorTeams, StreamWriter writer, int majorNumber)
        {
            writer.WriteLine("\n==================================================");
            writer.WriteLine($"MAJOR {majorNumber + 1} - SWISS + SINGLE ELIM");
            writer.WriteLine("==================================================");

            SwissTournament swiss = new SwissTournament(majorTeams);
            var top16 = swiss.Run();
            swiss.DisplaySwissResults(writer);

            SingleElimination singleElimination = new SingleElimination(top16, reseedBeforeBracket: false);
            singleElimination.Run(writer);

            return GetFinalMajorOrder(singleElimination, swiss);
        }

        private List<Team> RunDoubleElimMajor(List<Team> majorTeams, StreamWriter writer, int majorNumber)
        {
            writer.WriteLine("\n==================================================");
            writer.WriteLine($"MAJOR {majorNumber + 1} - DOUBLE ELIMINATION");
            writer.WriteLine("==================================================");

            DoubleElim doubleElim = new DoubleElim(majorTeams, reseedBeforeBracket: false);
            doubleElim.Run(writer);

            return GetFinalMajorOrder(doubleElim, majorTeams);
        }

        private List<Team> RunGroupSingleElimMajor(List<Team> majorTeams, StreamWriter writer, int majorNumber)
        {
            writer.WriteLine("\n==================================================");
            writer.WriteLine($"MAJOR {majorNumber + 1} - GROUPS + SINGLE ELIM");
            writer.WriteLine("==================================================");

            ResetMajorEventStats(majorTeams);

            var groups32 = new Groups32(majorTeams);
            return groups32.RunAndFinish(writer, majorNumber);
        }

        private void ResetMajorEventStats(List<Team> teams)
        {
            foreach (var team in teams)
            {
                team.resetRecord();
                team.SwissOpponents.Clear();
            }
        }

        private List<Team> GetFinalMajorOrder(SingleElimination playoffs, SwissTournament swiss)
        {
            if (playoffs == null)
                throw new ArgumentNullException(nameof(playoffs));

            if (swiss == null)
                throw new ArgumentNullException(nameof(swiss));

            var finalOrder = new List<Team>();

            var playoffOrder = playoffs.GetOrderedFinish();
            finalOrder.AddRange(playoffOrder);

            var swissElims = swiss.EliminatedTeams
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenByDescending(t => t.elo)
                .ToList();

            finalOrder.AddRange(swissElims);

            for (int i = 0; i < finalOrder.Count; i++)
                finalOrder[i].Seed = i + 1;

            return finalOrder;
        }

        private List<Team> GetFinalMajorOrder(DoubleElim doubleElim, List<Team> majorTeams)
        {
            if (doubleElim == null)
                throw new ArgumentNullException(nameof(doubleElim));

            if (majorTeams == null)
                throw new ArgumentNullException(nameof(majorTeams));

            var finalOrder = majorTeams
                .OrderBy(t => t.Seed)
                .ThenByDescending(t => t.elo)
                .ToList();

            for (int i = 0; i < finalOrder.Count; i++)
                finalOrder[i].Seed = i + 1;

            return finalOrder;
        }

        private void PrintMajorSeeds(List<Team> seeds, StreamWriter writer, int majorNumber)
        {
            writer.WriteLine("\n==================================================");
            writer.WriteLine($"MAJOR {majorNumber} SEEDS");
            writer.WriteLine("==================================================");

            for (int i = 0; i < seeds.Count; i++)
                writer.WriteLine($"{i + 1}. {seeds[i].name} [{seeds[i].Region} {seeds[i].RegionPoints}]");
        }

        private void PrintFinalMajorOrder(List<Team> finalOrder, StreamWriter writer, int majorNumber)
        {
            writer.WriteLine("\n==================================================");
            writer.WriteLine($"MAJOR {majorNumber} FINAL ORDER");
            writer.WriteLine("==================================================");

            for (int i = 0; i < finalOrder.Count; i++)
                writer.WriteLine($"{i + 1}. {finalOrder[i].name} [{finalOrder[i].Region}]");
        }

        public List<Team> PrintGlobalStandings(List<Team> allTeams, StreamWriter writer)
        {
            if (allTeams == null)
                throw new ArgumentNullException(nameof(allTeams));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var ordered = allTeams
                .OrderByDescending(t => t.TotalPoints)
                .ThenByDescending(t => t.elo)
                .Take(32)
                .ToList();

            var worldsTeams = allTeams
                .OrderByDescending(t => t.TotalPoints)
                .ThenByDescending(t => t.elo)
                .Take(16)
                .ToList();

            writer.WriteLine("\n==================================================");
            writer.WriteLine("GLOBAL STANDINGS (TOP 32 BY TOTAL POINTS)");
            writer.WriteLine("Top 16 qualify for WORLDS");
            writer.WriteLine("==================================================");

            for (int i = 0; i < ordered.Count; i++)
            {
                var team = ordered[i];
                string qualifier = i < 16 ? "  <-- WORLDS" : "";

                writer.WriteLine(
                    $"{i + 1,2}. {team.name,-25}  PTS:{team.TotalPoints,5}  Elo:{team.elo,4} Region{team.Region}{qualifier}");
            }

            writer.Flush();

            return worldsTeams;
        }

        public List<Team> GetWorldsTeams(StreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var allTeams = TeamFactory.GetAllTeams();
            var worldsTeams = new List<Team>();
            var qualifiedNames = new HashSet<string>();

            var guaranteedSlots = new Dictionary<string, int>
    {
        { "Western Europe", 4 },
        { "Central Europe", 2 },
        { "Italy", 2 },
        { "Eastern Europe", 1 },
        { "South America", 1 }
    };

            // Step 1: guaranteed regional qualifiers
            foreach (var slot in guaranteedSlots)
            {
                string region = slot.Key;
                int count = slot.Value;

                var regionalTeams = allTeams
                    .Where(t => NormalizeRegion(t.Region) == region)
                    .OrderByDescending(t => t.TotalPoints)
                    .ThenByDescending(t => t.elo)
                    .Take(count)
                    .ToList();

                foreach (var team in regionalTeams)
                {
                    if (qualifiedNames.Add(team.name))
                        worldsTeams.Add(team);
                }
         
            }



            // Step 2: fill last 6 at-large spots by TotalPoints
            var atLargeTeams = allTeams
                .Where(t => !qualifiedNames.Contains(t.name))
                .OrderBy(t => GetAverageMajorFinish(t))
                .ThenByDescending(t => t.TotalPoints)
                .ThenByDescending(t => t.elo)
                .Take(6)
                .ToList();

            foreach (var team in atLargeTeams)
            {
                if (qualifiedNames.Add(team.name))
                    worldsTeams.Add(team);
            }

            if (worldsTeams.Count != 16)
                throw new InvalidOperationException($"Expected 16 Worlds teams, got {worldsTeams.Count}.");

            // Step 3: seed by average major finish (lower is better)
            worldsTeams = worldsTeams
                .OrderBy(t => GetAverageMajorFinish(t))
                .ThenByDescending(t => t.TotalPoints)
                .ThenByDescending(t => t.elo)
                .ToList();

            for (int i = 0; i < worldsTeams.Count; i++)
                worldsTeams[i].Seed = i + 1;

            writer.WriteLine("\n==================================================");
            writer.WriteLine("WORLDS QUALIFIERS + SEEDS");
            writer.WriteLine("Guaranteed: WE 4 | CE 2 | IT 2 | EE 1 | SA 1");
            writer.WriteLine("At-Large: 6 by TotalPoints");
            writer.WriteLine("Seeds: Average Major Finish -> TotalPoints -> Elo");
            writer.WriteLine("==================================================");

            for (int i = 0; i < worldsTeams.Count; i++)
            {
                var team = worldsTeams[i];
                double avgFinish = GetAverageMajorFinish(team);

                writer.WriteLine(
                    $"{team.Seed,2}. {team.name,-25}  AvgFinish:{avgFinish,5:F2}  PTS:{team.TotalPoints,5}  Elo:{team.elo,4}  Region:{team.Region}");
            }

            writer.Flush();

            return worldsTeams;
        }

        private double GetAverageMajorFinish(Team team)
        {
            if (team == null)
                throw new ArgumentNullException(nameof(team));

            if (team.majorFinishes == null || team.majorFinishes.Count == 0)
                return double.MaxValue;

            return team.majorFinishes.Average();
        }
        

        private string NormalizeRegion(string region)
        {
            return region?.Replace("Region", "").Trim();
        }



        private class RegionalQualifierSet
        {
            public List<Team> WesternEurope { get; set; }
            public List<Team> CentralEurope { get; set; }
            public List<Team> Italy { get; set; }
            public List<Team> EasternEurope { get; set; }
            public List<Team> SouthAmerica { get; set; }
            public List<Team> Americas { get; set; }
            public List<Team> EastAsia { get; set; }
            public List<Team> MiddleEast { get; set; }
            public List<Team> Africa { get; set; }
            public List<Team> Wildcard { get; set; }
        }
    }
}