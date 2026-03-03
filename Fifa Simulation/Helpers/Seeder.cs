using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fifa_Simulation.Groups;

namespace Fifa_Simulation.Helpers
{
    public static class Seeder
    {
        public static List<Team> SeedTopTeams(List<InitialRoundRobin> groups)
        {
            var qualified = groups
                .SelectMany(g => g.GetStandings().Take(4))
                .OrderByDescending(t => t.Wins)
                .ThenByDescending(t => t.elo)
                .ToList();

            for (int i = 0; i < qualified.Count; i++)
                qualified[i].Seed = i + 1;

            return qualified;
        }
    }
}
