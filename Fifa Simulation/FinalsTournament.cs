using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fifa_Simulation
{
    public class FinalsTournament
    {
        private readonly List<Team> top16;

        public FinalsTournament(List<Team> allTeams)
        {
            // Select top 16 based on points, breaking ties by Elo
            top16 = allTeams
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.elo)
                .Take(16)
                .ToList();
        }

        public void Run()
        {
            Console.WriteLine("\n========== FINALS ==========");

            // Split top16 into 4 groups for the round-robin stage
            var groups = new List<FinalsGroup>
            {
                new FinalsGroup("A", new() { top16[0], top16[15], top16[8], top16[7] }),
                new FinalsGroup("B", new() { top16[1], top16[14], top16[9], top16[6] }),
                new FinalsGroup("C", new() { top16[2], top16[13], top16[10], top16[5] }),
                new FinalsGroup("D", new() { top16[3], top16[12], top16[11], top16[4] })
            };

            // Run each group
            foreach (var g in groups)
                g.Run();

            // Collect top 2 from each group
            List<Team> upperBracket = new(); // 1st place teams
            List<Team> lowerBracket = new(); // 2nd place teams
            foreach (var g in groups)
            {
                upperBracket.Add(g.Teams[0]);
                lowerBracket.Add(g.Teams[1]);
            }

            Console.WriteLine("\n--- UPPER/LOWER BRACKET MATCHUPS ---");

            // Upper bracket step 1: top seeds play
            var upperWinners = new List<Team>();
            var upperLosers = new List<Team>();
            for (int i = 0; i < upperBracket.Count; i += 2)
            {
                Team a = upperBracket[i];
                Team b = upperBracket[i + 1];
                Team winner = new Match(a, b).Play();
                Team loser = winner == a ? b : a;

                upperWinners.Add(winner);
                upperLosers.Add(loser);

                Console.WriteLine($"Upper Bracket: {a.name} vs {b.name} --- Winner: {winner.name}");
            }

            // Lower bracket step 2: initial single elimination
            var lowerWinners = new List<Team>();
            for (int i = 0; i < lowerBracket.Count; i += 2)
            {
                Team a = lowerBracket[i];
                Team b = lowerBracket[i + 1];
                Team winner = new Match(a, b).Play();
                lowerWinners.Add(winner);

                Console.WriteLine($"Lower Bracket: {a.name} vs {b.name} --- Winner: {winner.name}");
            }

            // Step 3: lower winners vs upper losers (quarterfinals)
            var quarterFinalists = new List<Team>();
            for (int i = 0; i < upperLosers.Count; i++)
            {
                Team a = upperLosers[i];
                Team b = lowerWinners[i];
                Team winner = new Match(a, b).Play();
                quarterFinalists.Add(winner);

                Console.WriteLine($"Quarterfinal: {a.name} vs {b.name} --- Winner: {winner.name}");
            }

            // Step 4: semifinals
            Console.WriteLine("\n--- SEMIFINALS ---");
            List<Team> semiFinalWinners = new();
            for(int i = 0; i < 2; i++)
            {
                Team a = upperWinners[i];
                Team b = quarterFinalists[i];

                Team winner = new Match(a, b).Play();
                semiFinalWinners.Add(winner);

                Console.WriteLine($"Semifinal: {a.name} vs {b.name} --- Winner: {winner.name}");
            }

            // Step 5: final
            Console.WriteLine("\n--- FINALS ---");
            Team champion = new Match(semiFinalWinners[0], semiFinalWinners[1]).Play();
            Console.WriteLine($"{semiFinalWinners[0].name} vs {semiFinalWinners[1].name}");
            Console.WriteLine($"\nCHAMPION: {champion.name}");
        }
    }
}
