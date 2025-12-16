using System;
using System.Collections.Generic;
using System.Linq;

namespace Fifa_Simulation
{
    public class SingleElimination
    {
        private List<Team> teams;
        private List<Team> finalRoundTeams = new();
        private List<Team> eliminatedTeams = new();

        public SingleElimination(List<Team> teams)
        {
            // Initially, sort teams by wins descending, losses ascending, then Elo descending
            this.teams = teams
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenByDescending(t => t.elo)
                .ToList();

            // Assign seeds based on this order
            for (int i = 0; i < this.teams.Count; i++)
                this.teams[i].Seed = i + 1;
        }

        public Team Run()
        {
            int round = 1;

            Console.WriteLine($"\n--- ELIMINATION ROUND ---");

            while (teams.Count > 1)
            {
                if (round == 1)
                    Console.WriteLine("\nQuarterfinals");
                else if (round == 2)
                    Console.WriteLine("\nSemifinals");
                else
                    Console.WriteLine("\nFinals");

                teams = PlayRound(teams);
                round++;
            }

            if (teams.Count > 0)
            {
                Console.WriteLine($"\n🏆 WINNER: {teams[0].name} (Seed {teams[0].Seed})");
                return teams[0];
            }
            else
            {
                Console.WriteLine("No teams remaining to select a winner!");
                return null;
            }
        }

        public List<Team> GetFinalists()
        {
            // Return the two teams that were in the final round
            return finalRoundTeams.Count >= 2 ? finalRoundTeams.Take(2).ToList() : new List<Team>(teams);
        }

        private List<Team> PlayRound(List<Team> roundTeams)
        {
            List<Team> winners = new();
            int l = 0, r = roundTeams.Count - 1;

            while (l < r)
            {
                Team a = roundTeams[l];
                Team b = roundTeams[r];
                new Match(a, b).Play();

                Team winner = a.Wins > b.Wins ? a : b;
                Team loser = winner == a ? b : a;

                Console.WriteLine($"{a.name} vs {b.name} --- Winner {winner.name}");

                winners.Add(winner);
                eliminatedTeams.Add(loser); // Track losers

                l++;
                r--;
            }

            // If odd number of teams, automatically advance the middle team
            if (l == r)
                winners.Add(roundTeams[l]);

            // Keep track of teams that reached the final round
            if (winners.Count <= 2)
                finalRoundTeams = new List<Team>(winners);

            return winners;
        }


        public void PrintRemainingTeams(string message = "Current Remaining Teams")
        {
            Console.WriteLine($"\n--- {message} ---");
            if (teams.Count == 0)
            {
                Console.WriteLine("No teams remaining!");
                return;
            }

            for (int i = 0; i < teams.Count; i++)
            {
                var t = teams[i];
                Console.WriteLine(
                    $"Index {i}: Seed {t.Seed}, {t.name}, W:{t.Wins}, L:{t.Losses}, Points:{t.Points}, Elo:{t.elo}"
                );
            }
        }

        public List<Team> GetEliminated()
        {
            return eliminatedTeams;
        }

    }
}
