using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using Fifa_Simulation.Helpers;

namespace Fifa_Simulation.Tournaments
{
    public class FinalsTournament
    {
        List<FinalsGroup> groups = new();
        public FinalsTournament(List<Team> top16)
        {
            int seed = 1;

            foreach (var t in top16)
            {
                t.Seed = seed;
                seed++;
            }

            groups = new List<FinalsGroup>
            {
                new FinalsGroup("A", new() { top16[0], top16[15], top16[8], top16[7] }),
                new FinalsGroup("B", new() { top16[1], top16[14], top16[9], top16[6] }),
                new FinalsGroup("C", new() { top16[2], top16[13], top16[10], top16[5] }),
                new FinalsGroup("D", new() { top16[3], top16[12], top16[11], top16[4] })
            };
        }

        public void Run(StreamWriter writer)
        {
            writer.WriteLine("\n========== FINALS ==========");

            foreach (var g in groups)
                g.Run(writer);
        

            List<Team> upperBracket = new();
            List<Team> lowerBracket = new();

            foreach (var g in groups)
            {
                upperBracket.Add(g.Teams[0]);
                lowerBracket.Add(g.Teams[1]);
            }

            writer.WriteLine("\n--- UPPER BRACKET MATCHUPS ---");

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

                writer.WriteLine($"Upper Bracket: {a.name} vs {b.name} --- Winner: {winner.name}");
            }

            writer.WriteLine("\n--- LOWER BRACKET MATCHUPS ---");

            var lowerWinners = new List<Team>();
            for (int i = 0; i < lowerBracket.Count; i += 2)
            {
                Team a = lowerBracket[i];
                Team b = lowerBracket[i + 1];

                Team winner = new Match(a, b).Play();
                lowerWinners.Add(winner);

                writer.WriteLine($"Lower Bracket: {a.name} vs {b.name} --- Winner: {winner.name}");
            }

            writer.WriteLine("\n--- QUARTERFINAL BRACKET MATCHUPS ---");

            var quarterFinalists = new List<Team>();
            for (int i = 1; i > -1; i--)
            {
                Team a = upperLosers[i];
                Team b = lowerWinners[i];

                Team winner = PlayBestOf(a, b, 3, writer);
                quarterFinalists.Add(winner);

                writer.WriteLine($"Quarterfinal Winner: {winner.name}\n");
            }

            // 🔥 SEMIFINALS — BEST OF 3
            writer.WriteLine("\n--- SEMIFINALS (BEST OF 3) ---\n----------------------------------------");

            List<Team> semiFinalWinners = new();
            for (int i = 0; i < 2; i++)
            {
                Team a = upperWinners[i];
                Team b = quarterFinalists[i];

                Team winner = PlayBestOf(a, b, 3, writer);
                semiFinalWinners.Add(winner);

                writer.WriteLine($"Semifinal Winner: {winner.name}\n");
            }

            // 🏆 FINALS — BEST OF 5
            writer.WriteLine("\n--- FINALS (BEST OF 5) ---\n-------------------------------------------");

            Team champion = PlayBestOf(semiFinalWinners[0], semiFinalWinners[1], 5, writer);
            writer.WriteLine($"\nCHAMPION: {champion.name}");
        }

        // =========================
        // BEST OF SERIES HELPER
        // =========================
        private Team PlayBestOf(Team a, Team b, int games, StreamWriter writer)
        {
            int winsA = 0;
            int winsB = 0;
            int required = games / 2 + 1;

            writer.WriteLine($"{a.name} vs {b.name} (Best of {games})");

            while (winsA < required && winsB < required)
            {
                Team winner = new Helpers.Match(a, b).Play();
                if (winner == a) winsA++;
                else winsB++;

                writer.WriteLine($"Game Result: {winner.name} | Series {winsA}-{winsB}");
            }

            return winsA > winsB ? a : b;
        }
    }
}
