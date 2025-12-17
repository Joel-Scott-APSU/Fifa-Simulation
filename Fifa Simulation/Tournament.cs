using System;
using System.Collections.Generic;
using System.Linq;

namespace Fifa_Simulation
{
    public class Tournament
    {
        private readonly List<Group> groups;
        private readonly List<Team> allTeams;

        private static readonly int[] NoWinsSwiss = { 3, 4, 5 };
        private static readonly int[] OneWinSwiss = { 4, 5, 6 };
        private static readonly int[] TwoWinsSiwss = {5, 6, 7};
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
                swiss.SwissPlacement();

                SingleElimination elim = new(swiss.AdvancedTeams);
                Team winner = elim.Run();

                winner.Seed = 1;
                AwardPoints(seeded, sim);
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

        private void AwardPoints(List<Team> teams, int sim)
        {
            foreach(var team in teams)
            {
                Console.WriteLine($"Team: {team.name} Seed: {team.Seed}");

                switch (team.Seed)
                {
                    case 1:
                        team.Points += WinnerPoints[sim];
                        break;

                    case 2:
                        team.Points += SecondPoints[sim];
                        break;

                    case 4:
                        team.Points += Top4Points[sim];
                        break;

                    case 8:
                        team.Points += Top8Points[sim];
                        break;

                    case 9:
                        team.Points += TwoWinsSiwss[sim];
                        break;

                    case 12:
                        team.Points += OneWinSwiss[sim];
                        break;

                    case 15:
                        team.Points += NoWinsSwiss[sim];
                        break;                        
                }
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
