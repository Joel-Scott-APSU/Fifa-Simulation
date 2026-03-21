using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fifa_Simulation.Helpers
{
    public class TeamStats
    {
        public string TeamName { get; set; }

        //Major 1
        public int Major1Wins { get; set; }
        public int Major1Top4s { get; set; }

        //Major 2
        public int Major2Wins { get; set; }
        public int Major2Top4s { get; set; }

        //Major 3
        public int Major3Wins { get; set; }
        public int Major3Top4s { get; set; }

        //Worlds
        public int WorldsWins { get; set; }
        public int WorldsTop4s { get; set; }
    }
}
