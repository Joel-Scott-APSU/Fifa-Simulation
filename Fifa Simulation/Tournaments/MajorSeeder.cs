using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fifa_Simulation.Tournaments
{
    public static class MajorSeeder
    {
        public static List<Team> SeedMajorOne(
            List<Team> westernEurope,
            List<Team> centralEurope,
            List<Team> italy,
            List<Team> easternEurope,
            List<Team> southAmerica,
            List<Team> americas,
            List<Team> eastAsia,
            List<Team> middleEast,
            List<Team> africa,
            List<Team> wildcard)
        {
            if (westernEurope == null) throw new ArgumentNullException(nameof(westernEurope));
            if (centralEurope == null) throw new ArgumentNullException(nameof(centralEurope));
            if (italy == null) throw new ArgumentNullException(nameof(italy));
            if (easternEurope == null) throw new ArgumentNullException(nameof(easternEurope));
            if (southAmerica == null) throw new ArgumentNullException(nameof(southAmerica));
            if (americas == null) throw new ArgumentNullException(nameof(americas));
            if (eastAsia == null) throw new ArgumentNullException(nameof(eastAsia));
            if (middleEast == null) throw new ArgumentNullException(nameof(middleEast));
            if (africa == null) throw new ArgumentNullException(nameof(africa));
            if (wildcard == null) throw new ArgumentNullException(nameof(wildcard));

            if (westernEurope.Count < 10) throw new ArgumentException("Western Europe must have at least 10 qualified teams.");
            if (centralEurope.Count < 6) throw new ArgumentException("Central Europe must have at least 6 qualified teams.");
            if (italy.Count < 4) throw new ArgumentException("Italy must have at least 4 qualified teams.");
            if (easternEurope.Count < 3) throw new ArgumentException("Eastern Europe must have at least 3 qualified teams.");
            if (southAmerica.Count < 2) throw new ArgumentException("South America must have at least 2 qualified teams.");
            if (americas.Count < 2) throw new ArgumentException("Americas must have at least 2 qualified teams.");
            if (eastAsia.Count < 1) throw new ArgumentException("East Asia must have at least 1 qualified team.");
            if (middleEast.Count < 1) throw new ArgumentException("Middle East must have at least 1 qualified team.");
            if (africa.Count < 1) throw new ArgumentException("Africa must have at least 1 qualified team.");
            if (wildcard.Count < 2) throw new ArgumentException("Wildcard must have at least 2 qualified teams.");

            var seeds = new List<Team>
            {
                westernEurope[0],
                italy[0],
                southAmerica[0],
                westernEurope[1],
                americas[0],
                centralEurope[0],
                westernEurope[2],
                easternEurope[0],

                italy[1],
                southAmerica[1],
                westernEurope[3],
                centralEurope[1],
                americas[1],
                westernEurope[4],
                easternEurope[1],
                wildcard[0],

                italy[2],
                westernEurope[5],
                centralEurope[2],
                easternEurope[2],
                westernEurope[6],
                italy[3],
                eastAsia[0],
                wildcard[1],

                westernEurope[7],
                centralEurope[3],
                middleEast[0],
                westernEurope[8],
                africa[0],
                centralEurope[4],
                westernEurope[9],
                centralEurope[5]
            };

            for (int i = 0; i < seeds.Count; i++)
                seeds[i].Seed = i + 1;

            return seeds;
        }

        public static List<Team> SeedMajorFromPreviousResults(
            List<Team> previousMajorFinalOrder,
            List<Team> westernEurope,
            List<Team> centralEurope,
            List<Team> italy,
            List<Team> easternEurope,
            List<Team> southAmerica,
            List<Team> americas,
            List<Team> eastAsia,
            List<Team> middleEast,
            List<Team> africa,
            List<Team> wildcard)
        {
            if (previousMajorFinalOrder == null) throw new ArgumentNullException(nameof(previousMajorFinalOrder));
            if (previousMajorFinalOrder.Count != 32)
                throw new ArgumentException("Previous major final order must contain exactly 32 teams.", nameof(previousMajorFinalOrder));

            var qualifiersByRegion = new Dictionary<string, List<Team>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Western Europe"] = westernEurope ?? throw new ArgumentNullException(nameof(westernEurope)),
                ["Central Europe"] = centralEurope ?? throw new ArgumentNullException(nameof(centralEurope)),
                ["Italy"] = italy ?? throw new ArgumentNullException(nameof(italy)),
                ["Eastern Europe"] = easternEurope ?? throw new ArgumentNullException(nameof(easternEurope)),
                ["South America"] = southAmerica ?? throw new ArgumentNullException(nameof(southAmerica)),
                ["Americas"] = americas ?? throw new ArgumentNullException(nameof(americas)),
                ["East Asia"] = eastAsia ?? throw new ArgumentNullException(nameof(eastAsia)),
                ["Middle East"] = middleEast ?? throw new ArgumentNullException(nameof(middleEast)),
                ["Africa"] = africa ?? throw new ArgumentNullException(nameof(africa)),
                ["Wildcard"] = wildcard ?? throw new ArgumentNullException(nameof(wildcard))
            };

            var regionUsage = qualifiersByRegion.Keys.ToDictionary(k => k, _ => 0, StringComparer.OrdinalIgnoreCase);
            var seeded = new List<Team>(32);

            foreach (var priorTeam in previousMajorFinalOrder)
            {
                if (string.IsNullOrWhiteSpace(priorTeam.Region))
                    throw new ArgumentException($"Team {priorTeam.name} is missing Region data in previous major results.");

                if (!qualifiersByRegion.TryGetValue(priorTeam.Region, out var regionList))
                    throw new ArgumentException($"Unknown region in previous major results: {priorTeam.Region}");

                int index = regionUsage[priorTeam.Region];

                if (index >= regionList.Count)
                    throw new ArgumentException(
                        $"Region {priorTeam.Region} does not have enough current qualifiers to fill the slots earned from the previous major.");

                var nextTeam = regionList[index];
                seeded.Add(nextTeam);
                regionUsage[priorTeam.Region]++;
            }

            for (int i = 0; i < seeded.Count; i++)
                seeded[i].Seed = i + 1;

            return seeded;
        }
    }
}