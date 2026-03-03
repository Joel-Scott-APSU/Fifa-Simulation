using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Fifa_Simulation.Teams
{
    public class Team
    {
        public string name {  get; set; }
        public int elo { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        public int Points { get; set; }

        public int Seed { get; set; }
        public double PerformanceBias { get; set; }
        public int MatchCounter { get; set; }

        public int SourceGroup { get; set; }

        public Team(string name, int elo, double PerformanceBias)
        {
            this.name = name;
            this.elo = elo;
            this.PerformanceBias = PerformanceBias;
            Points = 0;
            Seed = 0;
            Wins = 0;
            Losses = 0;
            MatchCounter = 0;
        }

        public void resetRecord()
        {
            Wins = 0;
            Losses = 0;
            MatchCounter = 0;
        }
    }
}
