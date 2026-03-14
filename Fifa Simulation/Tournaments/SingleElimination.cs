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
        private readonly List<Team> eliminatedTeams = new();

        public SingleElimination(List<Team> teams, bool reseedBeforeBracket = true)
        {
            if (teams == null) throw new ArgumentNullException(nameof(teams));
            if (teams.Count == 0) throw new ArgumentException("teams cannot be empty", nameof(teams));

            // If you want Swiss to control seeding, pass reseedBeforeBracket=false
            // and ensure teams already have Seed 1..N assigned.
            if (reseedBeforeBracket || teams.Any(t => t.Seed <= 0))
            {
                // Your original reseed logic (Swiss can also set Seed beforehand if you disable this)
                this.teams = teams
                    .OrderByDescending(t => t.Wins)
                    .ThenBy(t => t.Losses)
                    .ThenByDescending(t => t.elo)
                    .ToList();

                for (int i = 0; i < this.teams.Count; i++)
                    this.teams[i].Seed = i + 1;
            }
            else
            {
                this.teams = teams.ToList();
            }
        }

        public Team Run(StreamWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            int round = 1;

            while (teams.Count > 1)
            {
                WriteRoundHeader(writer, round, teams.Count);

                // IMPORTANT: keep bracket structure; do NOT sort by wins/elo each round.
                var bracketOrdered = OrderForBracket(teams);

                teams = PlayRound(bracketOrdered, writer);

                // Assign placement-based "Seed" for points AFTER the round completes
                foreach (Team team in eliminatedTeams)
                {
                    if (round == 1) team.Seed = 8;      // QF losers
                    else if (round == 2) team.Seed = 4; // SF losers
                    else team.Seed = 2;                 // Final loser
                }

                eliminatedTeams.Clear();
                round++;
            }

            if (teams.Count == 1)
            {
                teams[0].Seed = 1; // Champion points
                writer.WriteLine($"\n  WINNER: {teams[0].name}");
                return teams[0];
            }

            writer.WriteLine("No teams remaining to select a winner!");
            return null;
        }

        public List<Team> GetFinalists()
        {
            return finalRoundTeams.Count >= 2
                ? finalRoundTeams.Take(2).ToList()
                : new List<Team>(teams);
        }

        private List<Team> PlayRound(List<Team> bracketOrdered, StreamWriter writer)
        {
            var winners = new List<Team>();

            // Pair adjacent in bracket order: (0,1), (2,3), ...
            for (int i = 0; i < bracketOrdered.Count; i += 2)
            {
                Team a = bracketOrdered[i];
                Team b = bracketOrdered[i + 1];

                Team winner = new Helpers.Match(a, b).Play();
                Team loser = (winner == a) ? b : a;

                writer.WriteLine($"{a.name} vs {b.name} --- Winner {winner.name}");

                winners.Add(winner);
                eliminatedTeams.Add(loser);
            }

            // Track teams that reached the final round (last round that has <=2 winners)
            if (winners.Count <= 2)
            {
                finalRoundTeams.Clear();
                finalRoundTeams.AddRange(winners);
            }

            // Winners stay in bracket order (no reseed)
            return winners;
        }

        /// <summary>
        /// For 8 teams: forces standard bracket so 1 & 2 are on opposite sides:
        /// QF: 1v8, 4v5, 3v6, 2v7
        /// After that, bracket order is preserved.
        /// </summary>
        private static List<Team> OrderForBracket(List<Team> roundTeams)
        {
            // Always use current Seed values for ordering
            var bySeed = roundTeams.OrderBy(t => t.Seed).ToList();

            if (bySeed.Count == 8)
            {
                // [1,8,4,5,3,6,2,7] -> QF: (1,8)(4,5)(3,6)(2,7)
                return new List<Team>
                {
                    bySeed[0], bySeed[7],
                    bySeed[3], bySeed[4],
                    bySeed[2], bySeed[5],
                    bySeed[1], bySeed[6]
                };
            }

            // Generic fallback: 1vN, 2v(N-1), 3v(N-2)...
            // (This still keeps top seeds apart reasonably, but it's not the exact "true bracket" layout.)
            var ordered = new List<Team>();
            int l = 0, r = bySeed.Count - 1;
            while (l < r)
            {
                ordered.Add(bySeed[l++]);
                ordered.Add(bySeed[r--]);
            }

            if (l == r)
                ordered.Add(bySeed[l]); // should not happen for your 8-team case

            return ordered;
        }

        private static void WriteRoundHeader(StreamWriter writer, int round, int teamCount)
        {
            // For 8 teams: round 1=QF, 2=SF, 3=F
            if (teamCount == 8) writer.WriteLine("\nQuarterfinals\n----------------------------------");
            else if (teamCount == 4) writer.WriteLine("\nSemifinals\n----------------------------------");
            else if (teamCount == 2) writer.WriteLine("\nFinals\n----------------------------------");
            else writer.WriteLine($"\nRound {round}\n----------------------------------");
        }

        public List<Team> GetEliminated() => eliminatedTeams;
    }
}