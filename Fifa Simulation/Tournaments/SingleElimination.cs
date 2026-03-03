using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fifa_Simulation.Tournaments
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

        public Team Run(StreamWriter writer)
        {
            int round = 1;

            writer.WriteLine($"\n--- ELIMINATION ROUND ---");

            while (teams.Count > 1)
            {
                if (round == 1)
                {
                    writer.WriteLine("\nQuarterfinals\n----------------------------------");
                }
                else if (round == 2)
                    writer.WriteLine("\nSemifinals\n----------------------------------");
                else
                    writer.WriteLine("\nFinals\n----------------------------------");

                teams = PlayRound(teams, writer);
                foreach(Team team in eliminatedTeams)
                {
                    if(round == 1)
                    {
                        team.Seed = 8;
                    }
                    else if(round == 2)
                    {
                        team.Seed = 4;
                    }
                    else
                    {
                        team.Seed = 2;
                    }
                }

                eliminatedTeams.Clear();
                round++;
            }

            if (teams.Count > 0)
            {
                writer.WriteLine($"\n  WINNER: {teams[0].name}");
                return teams[0];
            }
            else
            {
                writer.WriteLine("No teams remaining to select a winner!");
                return null;
            }
        }

        public List<Team> GetFinalists()
        {
            // Return the two teams that were in the final round
            return finalRoundTeams.Count >= 2 ? finalRoundTeams.Take(2).ToList() : new List<Team>(teams);
        }

        private List<Team> PlayRound(List<Team> roundTeams, StreamWriter writer)
        {
            List<Team> winners = new();
            int l = 0, r = roundTeams.Count - 1;

            while (l < r)
            {
                Team a = roundTeams[l];
                Team b = roundTeams[r];
                new Helpers.Match(a, b).Play();

                Team winner = a.Wins > b.Wins ? a : b;
                Team loser = winner == a ? b : a;

                writer.WriteLine($"{a.name} vs {b.name} --- Winner {winner.name}");

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




        public List<Team> GetEliminated()
        {
            return eliminatedTeams;
        }

    }
}
