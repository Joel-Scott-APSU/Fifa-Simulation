using System;
using System.Collections.Generic;
using System.Linq;

namespace Fifa_Simulation
{
    public class Tournament
    {
        private readonly List<Group> groups;
        private readonly List<Team> allTeams;

        private static readonly int[][] SwissPoints =
        {
            new[] { 3, 4, 5 }, // 0 wins
            new[] { 4, 5, 6 }, // 1 win
            new[] { 5, 6, 7 }  // 2 wins
        };

        private static readonly int[] Top8Points = { 8, 10, 12 };
        private static readonly int[] Top4Points = { 12, 16, 20 };
        private static readonly int[] SecondPoints = { 16, 24, 32 };
        private static readonly int[] WinnerPoints = { 20, 30, 40 };

        public Tournament(List<Group> groups)
        {
            this.groups = groups;
            allTeams = groups.SelectMany(g => g.Teams).ToList();
        }

        public void RunThreeSimulations()
        {
            for (int sim = 0; sim < 3; sim++)
            {
                Console.WriteLine($"\n================ SIMULATION {sim + 1} ================");
                ResetWinsAndLosses();

                RunGroupStage();
                List<Team> seeded = SeedTopTeams();

                SwissTournament swiss = new(seeded);
                swiss.Run();
                swiss.DisplaySwissResults();
                AwardSwissPoints(swiss, sim);

                SingleElimination elim = new(swiss.AdvancedTeams);
                Team winner = elim.Run();

                AwardEliminationPoints(elim, sim);
            }

            DisplayFinalPointStandings();
        }

        private void ResetWinsAndLosses()
        {
            foreach (var team in allTeams)
            {
                team.Wins = 0;
                team.Losses = 0;
            }
        }

        private void RunGroupStage()
        {
            foreach (var group in groups)
            {
                group.PlayMatches();
                group.DisplayFinalStandings();
            }
        }

        private List<Team> SeedTopTeams()
        {
            var qualified = groups
                .SelectMany(g => g.GetStandings().Take(4))
                .OrderByDescending(t => t.Wins)
                .ThenByDescending(t => t.elo)
                .ToList();

            for (int i = 0; i < qualified.Count; i++)
                qualified[i].Seed = i + 1;

            return qualified;
        }

        private void AwardSwissPoints(SwissTournament swiss, int sim)
        {
            foreach (var team in swiss.EliminatedTeams)
            {
                int wins = team.Wins;
                team.Points += SwissPoints[wins][sim];
            }

            foreach (var team in swiss.AdvancedTeams)
            {
                team.Points += Top8Points[sim];
            }
        }

        private void AwardEliminationPoints(SingleElimination elim, int sim)
        {
            // Get all teams that participated in the elimination stage
            var remainingTeams = elim.GetFinalists();

            if (remainingTeams.Count < 2)
                return;

            // Determine winner and runner-up
            Team winner = remainingTeams[0]; // This should be the winner returned by elim.Run()
            Team runnerUp = remainingTeams[1];

            // Clear previously accumulated round-specific points before awarding final
            foreach (var t in remainingTeams)

            // Award points ONLY based on final standing
            winner.Points += WinnerPoints[sim];         // e.g., 20, 30, 40
            runnerUp.Points += SecondPoints[sim];      // e.g., 16, 24, 32

            // Award points to eliminated teams based on last round reached
            var eliminated = elim.GetEliminated(); // You’ll need to expose eliminated teams from SingleElimination
            foreach (var t in eliminated)
            {
                // Example logic for top 4 eliminated or top 8 eliminated
                if (t.Wins == 3) // reached semifinals but lost
                    t.Points += Top4Points[sim]; // e.g., 12, 16, 20
                else if (t.Wins == 2) // eliminated earlier
                    t.Points += Top8Points[sim]; // e.g., 8, 10, 12
                                                 // else no points
            }
        }



        private void DisplayFinalPointStandings()
        {
            Console.WriteLine("\n================ FINAL POINT STANDINGS ================");

            var top16 = allTeams
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.elo)
                .Take(16)
                .ToList();

            int rank = 1;
            foreach (var team in top16)
            {
                Console.WriteLine(
                    $"{rank,2}. {team.name,-25} Points:{team.Points,4} Elo:{team.elo}"
                );
                rank++;
            }
        }

    }
}
