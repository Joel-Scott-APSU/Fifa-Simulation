using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fifa_Simulation.Teams;

namespace Fifa_Simulation.Tournaments
{
    public static class RecordResetter
    {
        public static void ResetWinsAndLosses(IEnumerable<Team> teams)
        {
            foreach (var team in teams)
            {
                team.resetRecord();
            }
        }

        public static void ResetTournamentRecord(IEnumerable<Team> teams)
        {
            foreach (var team in teams)
            {
                team.resetRecord();
            }
        }
    }
}
