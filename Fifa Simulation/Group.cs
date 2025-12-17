using System;
using System.Collections.Generic;
using System.Linq;

namespace Fifa_Simulation
{
    public class Group
    {
        public string name { get; }
        public List<Team> Teams { get; }

        public Group(string name, List<Team> teams)
        {
            this.name = name;
            Teams = teams;
        }

        // Double round-robin: each team plays every other team twice
        public void PlayMatches()
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                for (int j = i + 1; j < Teams.Count; j++)
                {
                    // Match 1: i home, j away
                    new Match(Teams[i], Teams[j]).Play();

                    // Match 2: j home, i away
                    new Match(Teams[j], Teams[i]).Play();
                }
            }
        }

        // Return standings sorted by wins then Elo
        public List<Team> GetStandings()
        {
            return Teams
                .OrderByDescending(t => t.Wins)
                .ThenByDescending(t => t.elo)
                .ToList();
        }

        // Display final standings after group matches
        public void DisplayFinalStandings()
        {
            Console.WriteLine($"\n========== {name} FINAL STANDINGS ==========");

            int rank = 1;
            foreach (var team in GetStandings())
            {
                int totalGames = team.Wins + team.Losses;
                double winPercent = totalGames > 0 ? (double)team.Wins / totalGames * 100 : 0;

                Console.WriteLine(
                    $"{rank,2}. {team.name,-25}  W:{team.Wins}  L:{team.Losses}  Elo:{team.elo} Win Percent:{Math.Round(winPercent, 2)}"
                );
                rank++;
                team.resetRecord();
            }

        }
    }
}
