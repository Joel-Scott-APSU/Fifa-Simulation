using Fifa_Simulation;
using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fifa_Simulation.Helpers;

namespace Fifa_Simulation.Tournaments
{
    public class DoubleElim
    {
        private List<Team> teams;
        private List<Team> upperBracket = new();
        private List<Team> lowerBracket = new();
        private List<Team> eliminatedTeams = new();
        private int LowerRound = 1;
        private int UpperRound = 1;

        public DoubleElim(List<Team> teams)
        {
            // Initially, sort teams by wins descending, losses ascending, then Elo descending
            this.teams = teams
                .OrderByDescending(t => t.Wins)
                .ThenBy(t => t.Losses)
                .ThenByDescending(t => t.elo)
                .ToList();
            // Assign seeds based on this order
            for (int i = 0; i < this.teams.Count; i++)
                this.teams[i].Seed = i + 1;
            upperBracket.AddRange(this.teams);
        }

        public void Run(StreamWriter w)
        {
            // UB1
            upperBracket = PlayUpperBracket(upperBracket, w);

            // LB1 (uses UB1 drops already in lowerBracket)
            lowerBracket = PlayLowerBracket(lowerBracket, w);
            LowerRound++;

            // UB2
            upperBracket = PlayUpperBracket(upperBracket, w);

            // LB2 (uses UB2 drops)
            lowerBracket = PlayLowerBracket(lowerBracket, w);
            LowerRound++;

            // LB3 (no UB happened in between, so no new drops)
            lowerBracket = PlayLowerBracket(lowerBracket, w);
            LowerRound++;

            // UB3
            upperBracket = PlayUpperBracket(upperBracket, w);

            // LB4
            lowerBracket = PlayLowerBracket(lowerBracket, w);
            LowerRound++;

            // LB5
            lowerBracket = PlayLowerBracket(lowerBracket, w);
            LowerRound++;

            // UB Final
            w.WriteLine("Upper Finals\n---------------------------------");
            upperBracket = PlayUpperBracket(upperBracket, w);

            // LB Final
            w.WriteLine("Lower Finals\n---------------------------------");
            lowerBracket = PlayLowerBracket(lowerBracket, w);
            LowerRound++;


            // Grand Finals
            DoubleElimFinals(upperBracket[0], lowerBracket[0], w);
        }

        private List<Team> PlayUpperBracket(List<Team> currentTeams, StreamWriter w)
        {
            if(UpperRound != 4)
            {
                w.WriteLine($"Upper Round {UpperRound}\n---------------------------------");
            }
            var winners = new List<Team>();

            foreach (var (a, b) in PairBySeed(currentTeams))
            {
                var winner = new Match(a, b).Play();
                var loser = (winner == a) ? b : a;

                w.WriteLine($"{a.name} vs {b.name} --- Winner {winner.name}\n");

                winners.Add(winner);
                lowerBracket.Add(loser);
            }

            UpperRound++;
            // bye handling (only needed if odd count ever happens)
            if (currentTeams.Count % 2 == 1)
                winners.Add(currentTeams.OrderBy(t => t.Seed).ElementAt(currentTeams.Count / 2));

            return winners;
        }

        private List<Team> PlayLowerBracket(List<Team> currentTeams, StreamWriter streamWriter)
        {
            if(LowerRound != 6)
            {
                streamWriter.WriteLine($"Lower Round {LowerRound}\n---------------------------------");
            }

            var winners = new List<Team>();
            var eliminatedThisRound = new List<Team>();

            foreach (var (a, b) in PairBySeed(currentTeams))
            {
                Team winner = new Match(a, b).Play();
                Team loser = (winner == a) ? b : a;

                streamWriter.WriteLine($"{a.name} vs {b.name} --- Winner {winner.name}\n");

                winners.Add(winner);
                eliminatedThisRound.Add(loser);
            }

            // Bye handling (won’t happen in your fixed 16-team DE flow, but safe)
            if (currentTeams.Count % 2 == 1)
            {
                var byeTeam = currentTeams.OrderBy(t => t.Seed)
                                          .ElementAt(currentTeams.Count / 2);

                winners.Add(byeTeam);
                streamWriter.WriteLine($"{byeTeam.name} advances with a bye (Lower).");
            }

            foreach (Team t in eliminatedThisRound)
            {
                AssignLowerBracketSeed(t);
                eliminatedTeams.Add(t);
                streamWriter.WriteLine($"{t.name} has been eliminated.\n");
            }

            
            return winners;
        }

        private void DoubleElimFinals(Team ubChamp, Team lbChamp, StreamWriter w)
        {
            // Grand Finals 1
            Team firstWinner = new Match(ubChamp, lbChamp).Play();
            w.WriteLine($"Grand Finals (Match 1): {ubChamp.name} (UB) vs {lbChamp.name} (LB) --- Winner: {firstWinner.name}\n");

            Team finalWinner = firstWinner;

            // If LB champ wins Match 1, bracket resets -> play Match 2
            if (firstWinner == lbChamp)
            {
                finalWinner = new Match(ubChamp, lbChamp).Play();
                w.WriteLine($"Grand Finals (Reset Match): {ubChamp.name} vs {lbChamp.name} --- Winner: {finalWinner.name}\n");
            }

            Team runnerUp = (finalWinner == ubChamp) ? lbChamp : ubChamp;

            finalWinner.Seed = 1;
            runnerUp.Seed = 2;
        }

        private void AssignLowerBracketSeed(Team team)
        {
            if (LowerRound == 1)
                team.Seed = 16;
            else if (LowerRound == 2)
                team.Seed = 9;
            else if (LowerRound == 3)
                team.Seed = 8;
            else if (LowerRound == 4)
                team.Seed = 6;
            else if (LowerRound == 5)
                team.Seed = 4;
            else if (LowerRound == 6)
                team.Seed = 3;
            else
                team.Seed = 0; // For any further rounds, though in a standard double elim there shouldn't be more than 4 LB rounds
        }

        private static List<(Team, Team)> PairBySeed(List<Team> teams)
        {
            var ordered = teams.OrderBy(t => t.Seed).ToList();
            var pairings = new List<(Team, Team)>();

            int l = 0, r = ordered.Count - 1;
            while (l < r)
            {
                pairings.Add((ordered[l], ordered[r]));
                l++;
                r--;
            }

            return pairings;
        }
    }
}
