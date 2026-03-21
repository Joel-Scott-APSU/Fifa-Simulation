using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fifa_Simulation.Tournaments
{
    public class SingleElimination
    {
        private List<Team> teams;
        private readonly List<Team> finalRoundTeams = new();
        private readonly List<Team> allEliminatedTeams = new();

        public SingleElimination(List<Team> teams, bool reseedBeforeBracket = true)
        {
            if (teams == null)
                throw new ArgumentNullException(nameof(teams));

            if (teams.Count == 0)
                throw new ArgumentException("teams cannot be empty", nameof(teams));

            if (!IsPowerOfTwo(teams.Count))
                throw new ArgumentException("Single elimination requires a power-of-two team count.", nameof(teams));

            if (reseedBeforeBracket || teams.Any(t => t.Seed <= 0))
            {
                // Reseed using current tournament performance / rating
                this.teams = teams
                    .OrderByDescending(t => t.Wins)
                    .ThenBy(t => t.Losses)
                    .ThenByDescending(t => t.elo)
                    .ToList();

                for (int i = 0; i < this.teams.Count; i++)
                {
                    this.teams[i].Seed = i + 1;
                }
            }
            else
            {
                // Respect incoming seeds
                this.teams = teams
                    .OrderBy(t => t.Seed)
                    .ToList();
            }
        }

        public Team Run(StreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            // Build bracket order ONCE. After that, preserve bracket path.
            teams = OrderForBracket(teams);

            while (teams.Count > 1)
            {
                int teamsThisRound = teams.Count;

                WriteRoundHeader(writer, teamsThisRound);

                if (teamsThisRound == 2)
                {
                    finalRoundTeams.Clear();
                    finalRoundTeams.AddRange(teams);
                }

                List<Team> roundEliminated = new();
                teams = PlayRound(teams, writer, roundEliminated);

                foreach (Team team in roundEliminated)
                {
                    // Placement band based on round lost:
                    // lose in 16-team round => Seed = 16
                    // lose in 8-team round  => Seed = 8
                    // lose in 4-team round  => Seed = 4
                    // lose in 2-team round  => Seed = 2
                    team.Seed = teamsThisRound;
                }

                allEliminatedTeams.AddRange(roundEliminated);
            }

            if (teams.Count == 1)
            {
                teams[0].Seed = 1;
                writer.WriteLine($"\nWINNER: {teams[0].name}");
                return teams[0];
            }

            writer.WriteLine("No teams remaining to select a winner!");
            return null;
        }

        public List<Team> GetFinalists()
        {
            return new List<Team>(finalRoundTeams);
        }

        public List<Team> GetEliminated()
        {
            return new List<Team>(allEliminatedTeams);
        }

        public List<Team> GetOrderedFinish()
        {
            var ordered = new List<Team>();

            if (teams.Count == 1)
                ordered.Add(teams[0]); // Winner

            // Final loser should already have Seed = 2, SF losers = 4, etc.
            ordered.AddRange(allEliminatedTeams
                .OrderBy(t => t.Seed)
                .ThenByDescending(t => t.elo));

            return ordered;
        }

        private static List<Team> PlayRound(List<Team> roundTeams, StreamWriter writer, List<Team> roundEliminated)
        {
            var winners = new List<Team>();

            for (int i = 0; i < roundTeams.Count; i += 2)
            {
                Team a = roundTeams[i];
                Team b = roundTeams[i + 1];

                Team winner = new Helpers.Match(a, b).Play();
                Team loser = winner == a ? b : a;

                writer.WriteLine($"{a.name} vs {b.name} --- Winner: {winner.name}");

                winners.Add(winner);
                roundEliminated.Add(loser);
            }

            return winners;
        }

        private static List<Team> OrderForBracket(List<Team> roundTeams)
        {
            var bySeed = roundTeams
                .OrderBy(t => t.Seed)
                .ToList();

            int count = bySeed.Count;
            int[] positions = BuildBracketSeedOrder(count);

            var ordered = new List<Team>(count);

            foreach (int seedNumber in positions)
            {
                ordered.Add(bySeed[seedNumber - 1]);
            }

            return ordered;
        }

        /// <summary>
        /// Builds standard bracket seed order for any power-of-two size.
        /// Example:
        /// 2  => [1,2]
        /// 4  => [1,4,2,3]
        /// 8  => [1,8,4,5,2,7,3,6]
        /// 16 => [1,16,8,9,4,13,5,12,2,15,7,10,3,14,6,11]
        /// </summary>
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

        private static void WriteRoundHeader(StreamWriter writer, int teamCount)
        {
            string title = teamCount switch
            {
                64 => "Round of 64",
                32 => "Round of 32",
                16 => "Round of 16",
                8 => "Quarterfinals",
                4 => "Semifinals",
                2 => "Final",
                _ => $"Round with {teamCount} teams"
            };

            writer.WriteLine($"\n{title}\n----------------------------------");
        }

        private static bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }


    }
}