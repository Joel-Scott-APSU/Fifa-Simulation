using Fifa_Simulation.Teams;
using System;

namespace Fifa_Simulation.Helpers
{
    public class Match
    {
        private static readonly Random rng = new Random();

        private readonly Team teamA;
        private readonly Team teamB;

        // Maximum amount a team can swing above/below baseline Elo
        private const int MaxBiasEloSwing = 120;

        // 2% fatigue added per match played
        private const double FatiguePerMatch = 0.02;

        // Optional fatigue cap (20 matches = 40%)
        private const double MaxFatigue = 0.40;

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
        /// Converts baseline Elo into a match-specific Elo using:
        /// - 50/50 above or below baseline
        /// - random swing size
        /// - fatigue that lowers ceiling and worsens floor
        /// </summary>
        private static double GetEffectiveElo(Team team)
        {
            // 50/50 chance of performing above or below baseline
            bool aboveBaseline = rng.NextDouble() < 0.5;

            // Random swing amount from 0 to MaxBiasEloSwing
            double swing = rng.NextDouble() * MaxBiasEloSwing;

            // Fatigue builds per match and only affects the swing
            double fatigue = team.MatchCounter * FatiguePerMatch;
            fatigue = Math.Min(fatigue, MaxFatigue);

            if (aboveBaseline)
            {
                // Fatigue reduces peak performance
                swing *= (1.0 - fatigue);
            }
            else
            {
                // Fatigue worsens poor performances
                swing *= -(1.0 + fatigue);
            }

            // Base Elo stays intact
            return team.elo + swing;
        }

        private static double GetWinProbability(double eloA, double eloB)
        {
            return 1.0 / (1.0 + Math.Pow(10, (eloB - eloA) / 400.0));
        }
    }
}