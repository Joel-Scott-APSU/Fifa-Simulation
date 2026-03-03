using Fifa_Simulation.Groups;
using Fifa_Simulation.Helpers;
using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using Fifa_Simulation.Groups;

namespace Fifa_Simulation.Tournaments
{
    public class Tournament
    {
        private readonly List<InitialRoundRobin> groups;
        private readonly List<Team> allTeams;

        public Tournament(List<InitialRoundRobin> groups, List<Team> allTeams)
        {
            this.groups = groups;
            this.allTeams = allTeams;
            
        }

        public List<Team> RunThreeSimulations(StreamWriter writer)
        {
            for (int sim = 0; sim < 3; sim++)
            {
                writer.WriteLine($"\n================ SIMULATION {sim + 1} ================");
                ResetWinsAndLosses();
                RunGroupStage(writer);
                List<Team> seeded = Seeder.SeedTopTeams(groups);
                foreach (Team team in allTeams)
                {
                    team.resetRecord();
                }

                if (sim == 0)
                {
                    SwissTournament swiss = new(seeded);
                    swiss.Run();
                    swiss.DisplaySwissResults(writer);
                    swiss.SwissPlacement();

                    SingleElimination elim = new(swiss.AdvancedTeams);
                    Team winner = elim.Run(writer);

                    winner.Seed = 1;
                    foreach (Team team in seeded)
                    {
                        PointsAwarder.AwardPoints(team, sim);
                    }
                }
                else if (sim == 1)
                {
                    DoubleElim doubleElim = new(seeded);
                    doubleElim.Run(writer);
                    foreach (Team team in seeded)
                    {
                        PointsAwarder.AwardPoints(team, sim);
                    }
                }
                else if (sim == 2)
                {
                    Groups.Groups group = new(groups);
                    group.RunAndFinish(writer, sim);
                }
            }

            StandingsPrinter.PrintFinalPointStandings(allTeams, writer);

            var top16 = allTeams
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.elo)
                .Take(16)
                .ToList();

            return top16;
        }

        private void ResetWinsAndLosses()
        {
            foreach (var team in allTeams)
            {
                team.Wins = 0;
                team.Losses = 0;
                team.MatchCounter = 0;
            }
        }

        private void RunGroupStage(StreamWriter writer)
        {
            foreach (var group in groups)
            {
                group.PlayMatches();
                group.DisplayFinalStandings(writer);
            }
        }
    }
}
