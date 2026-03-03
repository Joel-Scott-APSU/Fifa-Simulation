using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fifa_Simulation.Teams;

namespace Fifa_Simulation.Tournaments
{
    public static class StandingsPrinter
    {
        public static void PrintFinalPointStandings(IEnumerable<Team> teams, StreamWriter writer)
        {
            writer.WriteLine("\n================ FINAL POINT STANDINGS ================");

            var top16 = teams
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.elo)
                .Take(16)
                .ToList();

            int rank = 1;
            foreach (var team in top16)
            {
                writer.WriteLine(
                    $"{rank}. {team.name} Points:{team.Points}"
                );
                rank++;
            }
        }
    }
}
