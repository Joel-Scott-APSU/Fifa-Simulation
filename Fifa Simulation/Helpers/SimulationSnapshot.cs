using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fifa_Simulation.Teams;

namespace Fifa_Simulation.Helpers
{
    public class SimulationSnapshot
    {
        public int SimulationNumber { get; set; }
        public List<Team> Major1Top4 { get; set; } = new();
        public List<Team> Major2Top4 { get; set; } = new();
        public List<Team> Major3Top4 { get; set; } = new();
        public List<Team> WorldsTop4 { get; set; } = new();
    }
}
