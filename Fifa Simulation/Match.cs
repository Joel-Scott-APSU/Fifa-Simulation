using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Fifa_Simulation
{
    public class Match
    {
        private static readonly Random rng = new Random();

        private readonly Team teamA;
        private readonly Team teamB;

        public Match(Team teamA, Team teamB)
        {
            this.teamA = teamA;
            this.teamB = teamB;
        }

        public Team Play()
        {
            double probabilityA = GetWinProbability(teamA.elo, teamB.elo);

            Team winner = rng.NextDouble() < probabilityA ? teamA : teamB;
            Team loser = winner == teamA ? teamB : teamA;

            winner.Wins++;
            loser.Losses++;

            return winner;
        }

        private static double GetWinProbability(int eloA, int eloB)
        {
            return 1.0 / (1.0 + Math.Pow(10, (double)(eloB - eloA) / 400.0));
        }
    }
}
