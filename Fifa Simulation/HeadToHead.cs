using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fifa_Simulation
{
    public class HeadToHead
    {
        private readonly Dictionary<(Team, Team), int> wins = new();

        public void RecordWin(Team Winner, Team Loser)
        {
            var key = (Winner, Loser);
            wins[key] = wins.GetValueOrDefault(key);
        }

        public int GetWins(Team a, Team b)
        {
            return wins.GetValueOrDefault((a, b));
        }
    }
}
