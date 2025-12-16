using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Fifa_Simulation
{
    public class Team
    {
        public string name {  get; set; }
        public int elo { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        public int Points { get; set; }

        public int Seed { get; set; }

        public Team(string name, int elo)
        {
            this.name = name;
            this.elo = elo;
            Points = 0;
            Seed = 0;
            Wins = 0;
            Losses = 0;
        }

        public void resetRecord()
        {
            Wins = 0;
            Losses = 0;
        }
    }
}
