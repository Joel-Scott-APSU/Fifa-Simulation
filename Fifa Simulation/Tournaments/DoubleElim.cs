using Fifa_Simulation.Helpers;
using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fifa_Simulation.Tournaments
{
    public class DoubleElim
    {
        private List<Team> upperBracket;
        private readonly List<Team> allEliminatedTeams = new();
        private readonly List<Team> grandFinalists = new();

        public DoubleElim(List<Team> teams, bool reseedBeforeBracket = true)
        {
            if (teams == null)
                throw new ArgumentNullException(nameof(teams));

            if (teams.Count != 32)
                throw new ArgumentException("This DoubleElim class is written for a 32-team bracket.", nameof(teams));

            List<Team> seededTeams;

            if (reseedBeforeBracket || teams.Any(t => t.Seed <= 0))
            {
                seededTeams = teams
                    .OrderByDescending(t => t.Wins)
                    .ThenBy(t => t.Losses)
                    .ThenByDescending(t => t.elo)
                    .ToList();

                for (int i = 0; i < seededTeams.Count; i++)
                    seededTeams[i].Seed = i + 1;
            }
            else
            {
                seededTeams = teams
                    .OrderBy(t => t.Seed)
                    .ToList();
            }

            upperBracket = OrderForBracket(seededTeams);
        }

        public Team Run(StreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            // UPPER ROUND 1 (32 -> 16)
            writer.WriteLine("\nUPPER BRACKET - ROUND OF 32\n---------------------------------");
            var ubR1 = PlayAdjacentRound(upperBracket, writer, out var ubR1Losers);

            // LOWER ROUND 1
            // Eliminated = 25th-32nd -> Seed 32
            writer.WriteLine("\nLOWER BRACKET - ROUND 1\n---------------------------------");
            var lbR1 = PlayAdjacentRound(ubR1Losers, writer, out var lbR1Losers);
            AssignPlacement(lbR1Losers, 32);

            // UPPER ROUND 2 (16 -> 8)
            writer.WriteLine("\nUPPER BRACKET - ROUND OF 16\n---------------------------------");
            var ubR2 = PlayAdjacentRound(ubR1, writer, out var ubR2Losers);

            // LOWER ROUND 2
            // LB1 winners vs UB R2 losers
            // Eliminated = 17th-24th -> Seed 24
            writer.WriteLine("\nLOWER BRACKET - ROUND 2\n---------------------------------");
            var lbR2 = PlayCrossRound(lbR1, ubR2Losers, writer, out var lbR2Losers);
            AssignPlacement(lbR2Losers, 24);

            // UPPER ROUND 3 (8 -> 4)
            writer.WriteLine("\nUPPER BRACKET - QUARTERFINALS\n---------------------------------");
            var ubR3 = PlayAdjacentRound(ubR2, writer, out var ubR3Losers);

            // LOWER ROUND 3
            // LB2 winners play each other
            // Eliminated = 13th-16th -> Seed 16
            writer.WriteLine("\nLOWER BRACKET - ROUND 3\n---------------------------------");
            var lbR3 = PlayAdjacentRound(lbR2, writer, out var lbR3Losers);
            AssignPlacement(lbR3Losers, 16);

            // LOWER ROUND 4
            // LB3 winners vs UB R3 losers
            // Eliminated = 9th-12th -> Seed 12
            writer.WriteLine("\nLOWER BRACKET - ROUND 4\n---------------------------------");
            var lbR4 = PlayCrossRound(lbR3, ubR3Losers, writer, out var lbR4Losers);
            AssignPlacement(lbR4Losers, 12);

            // UPPER ROUND 4 (4 -> 2)
            writer.WriteLine("\nUPPER BRACKET - SEMIFINALS\n---------------------------------");
            var ubR4 = PlayAdjacentRound(ubR3, writer, out var ubR4Losers);

            // LOWER ROUND 5
            // LB4 winners play each other
            // Eliminated = 7th-8th -> Seed 8
            writer.WriteLine("\nLOWER BRACKET - ROUND 5\n---------------------------------");
            var lbR5 = PlayAdjacentRound(lbR4, writer, out var lbR5Losers);
            AssignPlacement(lbR5Losers, 8);

            // LOWER ROUND 6
            // LB5 winners vs UB R4 losers
            // Eliminated = 5th-6th -> Seed 6
            writer.WriteLine("\nLOWER BRACKET - ROUND 6\n---------------------------------");
            var lbR6 = PlayCrossRound(lbR5, ubR4Losers, writer, out var lbR6Losers);
            AssignPlacement(lbR6Losers, 6);

            // UPPER FINAL (2 -> 1)
            writer.WriteLine("\nUPPER BRACKET - FINAL\n---------------------------------");
            var ubFinalWinners = PlayAdjacentRound(ubR4, writer, out var ubFinalLosers);
            Team ubChampion = ubFinalWinners[0];
            Team ubFinalLoser = ubFinalLosers[0];

            // LOWER ROUND 7
            // LB6 winners play each other
            // Eliminated = 4th -> Seed 4
            writer.WriteLine("\nLOWER BRACKET - ROUND 7\n---------------------------------");
            var lbR7 = PlayAdjacentRound(lbR6, writer, out var lbR7Losers);
            AssignPlacement(lbR7Losers, 4);

            // LOWER FINAL
            // LB7 winner vs UB Final loser
            // Eliminated = 3rd -> Seed 3
            writer.WriteLine("\nLOWER BRACKET - FINAL\n---------------------------------");
            var lbFinal = PlayCrossRound(lbR7, new List<Team> { ubFinalLoser }, writer, out var lbFinalLosers);
            AssignPlacement(lbFinalLosers, 3);

            Team lbChampion = lbFinal[0];

            // GRAND FINAL
            grandFinalists.Clear();
            grandFinalists.Add(ubChampion);
            grandFinalists.Add(lbChampion);

            return PlayGrandFinal(ubChampion, lbChampion, writer);
        }

        public List<Team> GetGrandFinalists()
        {
            return new List<Team>(grandFinalists);
        }

        public List<Team> GetEliminated()
        {
            return new List<Team>(allEliminatedTeams);
        }

        private List<Team> PlayAdjacentRound(List<Team> teams, StreamWriter writer, out List<Team> losers)
        {
            if (teams == null)
                throw new ArgumentNullException(nameof(teams));

            if (teams.Count % 2 != 0)
                throw new InvalidOperationException("Adjacent round requires an even number of teams.");

            var winners = new List<Team>();
            losers = new List<Team>();

            for (int i = 0; i < teams.Count; i += 2)
            {
                Team a = teams[i];
                Team b = teams[i + 1];

                Team winner = new Match(a, b).Play();
                Team loser = winner == a ? b : a;

                writer.WriteLine($"{a.name} vs {b.name} --- Winner: {winner.name}");

                winners.Add(winner);
                losers.Add(loser);
            }

            return winners;
        }

        private List<Team> PlayCrossRound(List<Team> leftSide, List<Team> rightSide, StreamWriter writer, out List<Team> losers)
        {
            if (leftSide == null)
                throw new ArgumentNullException(nameof(leftSide));

            if (rightSide == null)
                throw new ArgumentNullException(nameof(rightSide));

            if (leftSide.Count != rightSide.Count)
                throw new InvalidOperationException("Cross round requires matching team counts.");

            var winners = new List<Team>();
            losers = new List<Team>();

            for (int i = 0; i < leftSide.Count; i++)
            {
                Team a = leftSide[i];
                Team b = rightSide[i];

                Team winner = new Match(a, b).Play();
                Team loser = winner == a ? b : a;

                writer.WriteLine($"{a.name} vs {b.name} --- Winner: {winner.name}");

                winners.Add(winner);
                losers.Add(loser);
            }

            return winners;
        }

        private Team PlayGrandFinal(Team ubChampion, Team lbChampion, StreamWriter writer)
        {
            writer.WriteLine("\nGRAND FINAL\n---------------------------------");

            Team firstWinner = new Match(ubChampion, lbChampion).Play();
            writer.WriteLine($"{ubChampion.name} (UB) vs {lbChampion.name} (LB) --- Winner: {firstWinner.name}");

            Team champion = firstWinner;

            // LB champion must beat UB champion twice
            if (firstWinner == lbChampion)
            {
                writer.WriteLine("\nBRACKET RESET\n---------------------------------");

                Team resetWinner = new Match(ubChampion, lbChampion).Play();
                writer.WriteLine($"{ubChampion.name} vs {lbChampion.name} --- Winner: {resetWinner.name}");

                champion = resetWinner;
            }

            Team runnerUp = champion == ubChampion ? lbChampion : ubChampion;

            champion.Seed = 1;
            runnerUp.Seed = 2;
            allEliminatedTeams.Add(runnerUp);

            writer.WriteLine($"\nCHAMPION: {champion.name}");
            return champion;
        }

        private void AssignPlacement(List<Team> eliminatedThisRound, int placementSeed)
        {
            foreach (Team team in eliminatedThisRound)
            {
                team.Seed = placementSeed;
                allEliminatedTeams.Add(team);
            }
        }

        private static List<Team> OrderForBracket(List<Team> seededTeams)
        {
            var bySeed = seededTeams
                .OrderBy(t => t.Seed)
                .ToList();

            int[] positions = BuildBracketSeedOrder(bySeed.Count);

            var ordered = new List<Team>(bySeed.Count);
            foreach (int seedNumber in positions)
            {
                ordered.Add(bySeed[seedNumber - 1]);
            }

            return ordered;
        }

        private static int[] BuildBracketSeedOrder(int size)
        {
            if (!IsPowerOfTwo(size))
                throw new ArgumentException("Bracket size must be a power of two.", nameof(size));

            List<int> seeds = new() { 1, 2 };

            while (seeds.Count < size)
            {
                int nextSize = seeds.Count * 2 + 1;
                var expanded = new List<int>(seeds.Count * 2);

                foreach (int seed in seeds)
                {
                    expanded.Add(seed);
                    expanded.Add(nextSize - seed);
                }

                seeds = expanded;
            }

            return seeds.ToArray();
        }

        private static bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }
    }
}