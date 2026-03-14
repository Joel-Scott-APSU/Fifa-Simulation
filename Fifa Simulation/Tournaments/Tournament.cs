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
                RunGroupStage(writer);
                List<Team> seeded = Seeder.SeedTopTeams(groups);

                if (sim == 2)
                {
                    Groups.Groups group = new(groups);   // <-- construct BEFORE resetRecord
                    foreach (Team team in allTeams) team.resetRecord(); // reset for stage play
                    group.RunAndFinish(writer, sim);
                }
                else
                {
                    foreach (Team team in allTeams) team.resetRecord();

                    if (sim == 0)
                    {
                        SwissTournament swiss = new(seeded);
                        List<Team> top8 = swiss.Run();
                        swiss.DisplaySwissResults(writer, top8);

                        writer.WriteLine("================== TOP 8 ================");
                        SingleElimination elim = new(top8);
                        Team winner = elim.Run(writer);

                        winner.Seed = 1;
                        foreach (Team team in seeded)
                            PointsAwarder.AwardPoints(team, sim);
                        foreach(Team team in allTeams)
                        {
                            team.resetRecord();
                        }
                       
                    }
                    else if (sim == 1)
                    {
                        DoubleElim doubleElim = new(seeded);
                        doubleElim.Run(writer);
                        foreach (Team team in seeded)
                            PointsAwarder.AwardPoints(team, sim);
                        foreach (Team team in allTeams)
                        {
                            team.resetRecord();
                        }
                    }
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
