using System;

namespace Fifa_Simulation
{
    public class Match
    {
        private static readonly Random rng = new Random();

        private readonly Team teamA;
        private readonly Team teamB;

        // How much bias can swing Elo (tunable)
        private const int MaxBiasEloSwing = 120;

        public Match(Team teamA, Team teamB)
        {
            this.teamA = teamA;
            this.teamB = teamB;
        }

        public Team Play()
        {
            double effectiveEloA = GetEffectiveElo(teamA);
            double effectiveEloB = GetEffectiveElo(teamB);

            double probabilityA = GetWinProbability(effectiveEloA, effectiveEloB);

            Team winner = rng.NextDouble() < probabilityA ? teamA : teamB;
            Team loser = winner == teamA ? teamB : teamA;

            winner.Wins++;
            loser.Losses++;

            return winner;
        }

        /// <summary>
        /// Converts Elo + performance bias into a match-specific Elo
        /// </summary>
        private static double GetEffectiveElo(Team team)
        {
            // Bias is [0.0, 1.0] → map to [-1, +1]
            double biasCentered = (team.PerformanceBias - 0.5) * 2.0;

            // Small random variance so same teams don't play identically
            double noise = rng.NextDouble() * 0.15 - 0.075; // ±7.5%

            double biasMultiplier = biasCentered + noise;

            return team.elo + (biasMultiplier * MaxBiasEloSwing);
        }

        private static double GetWinProbability(double eloA, double eloB)
        {
            return 1.0 / (1.0 + Math.Pow(10, (eloB - eloA) / 400.0));
        }
    }
}
