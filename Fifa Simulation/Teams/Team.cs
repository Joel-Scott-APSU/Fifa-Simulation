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
        public HashSet<Team> SwissOpponents { get; set; } = new HashSet<Team>();
        public int elo { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        public int TotalPoints { get; set; }
        public int RegionPoints { get; set; }

        public int Seed { get; set; }
        public int MatchCounter { get; set; }

        public int SourceGroup { get; set; }
        public int[] RegionalFinishes { get; set; }
        public int PreviousMajorFinish { get; set; } = 999;

        public string Region { get; set; }
        public List<int> majorFinishes = new List<int> { 33, 33, 33};
        public Team(string name, int elo, string region)
        {
            this.name = name;
            this.elo = elo;
            TotalPoints = 0;
            RegionPoints = 0;
            Seed = 0;
            Wins = 0;
            Losses = 0;
            MatchCounter = 0;
            RegionalFinishes = new int[3];
            Region = region;
        }

        public void resetRecord()
        {
            Wins = 0;
            Losses = 0;
            MatchCounter = 0;
        }
    }
}
