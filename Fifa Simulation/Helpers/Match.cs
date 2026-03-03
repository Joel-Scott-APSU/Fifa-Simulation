using Fifa_Simulation.Teams;
using System;

namespace Fifa_Simulation.Helpers
{
    public class Match
    {
        private static readonly Random rng = new Random();

        private readonly Team teamA;
        private readonly Team teamB;

        // How much bias can swing Elo (tunable)
        private const int MaxBiasEloSwing = 120;
        private const double FatiguePerMatch = 0.02; // Each match adds 2% fatigue
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

            teamA.MatchCounter++;
            teamB.MatchCounter++;

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

            // ±7.5% noise
            double noise = rng.NextDouble() * 0.15 - 0.075;

            double biasMultiplier = biasCentered + noise;

            // Fatigue: -2% per match, but only applied to the swing
            double fatigue = 1.0 - (team.MatchCounter * FatiguePerMatch);

            // Clamp so it never goes negative / silly
            fatigue = Math.Max(0.4, fatigue);

            double swing = (biasMultiplier * MaxBiasEloSwing) * fatigue;

            // Base Elo stays intact
            return team.elo + swing;
        }


        private static double GetWinProbability(double eloA, double eloB)
        {
            return 1.0 / (1.0 + Math.Pow(10, (eloB - eloA) / 400.0));
        }
    }
}
