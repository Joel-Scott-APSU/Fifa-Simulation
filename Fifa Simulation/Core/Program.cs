using Fifa_Simulation.Groups;
using Fifa_Simulation.Helpers;
using Fifa_Simulation.Teams;
using Fifa_Simulation.Tournaments;
using System.Collections.Generic;

namespace Fifa_Simulation.Core
{
    class Program
    {
        static void Main()
        { 
            Console.WriteLine("1 - Full FIFA Simulation");
            Console.WriteLine("2 - Individual Match Test");
            Console.Write("Select mode: ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    for (int i = 0; i < 5; i++)
                    {
                        RunFifaSimulation();
                    }
                    break;

                case "2":
                    //RunIndividualMatch();
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

            Console.ReadLine();

            Console.WriteLine("Would you like to run another simulation? (y/n)");
            string again = Console.ReadLine();
            if (again.ToLower() == "y")
            {
                Main(); // Restart the process
            }
            else
            {
                Console.WriteLine("Exiting program. Goodbye!");
            }
        }


        static void RunFifaSimulation()
        {
            string folderPath = @"C:\Users\Joel\Desktop\Fifa Simulation\Output";
            string baseName = "Sim";
            string extension = ".txt";
            int counter = 1;

            // 1. Ensure the directory exists first
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            // 2. Find the next available file number
            while (File.Exists(Path.Combine(folderPath, $"{baseName} {counter}{extension}")))
            {
                counter++;
            }

            string finalPath = Path.Combine(folderPath, $"{baseName} {counter}{extension}");

            // 3. Initialize your data
            TeamFactory.CreateGroups(out List<Team> groupA, out List<Team> groupB, out List<Team> groupC, out List<Team> groupD);
            var groups = new List<InitialRoundRobin> {
        new InitialRoundRobin("Group A", groupA), new InitialRoundRobin("Group B", groupB),
        new InitialRoundRobin("Group C", groupC), new InitialRoundRobin("Group D", groupD)
    };
            var allTeams = groups.SelectMany(g => g.Teams).ToList();

            foreach (var team in allTeams)
            {
                team.resetRecord();
            }
            // 4. Wrap everything in the StreamWriter
            using (StreamWriter sw = new StreamWriter(finalPath))
            {
                // Redirect simulation output to the file
                Tournament tournament = new Tournament(groups, allTeams);

                var top16 = tournament.RunThreeSimulations(sw);

                var finals = new FinalsTournament(top16);
                finals.Run(sw);

                Console.WriteLine($"Simulation complete! Results saved to: {finalPath}");
            }

            Console.ReadLine();
        }

       /* static void RunIndividualMatch()
        {
            Team team1 = new Team("Inter Miami", 2180, 0.84);
            Team team2 = new Team("Liverpool", 2016, 0.77);
            Team team3 = new Team("Real Madrid", 2015, 0.80);
            Team team4 = new Team("PSG", 2020, 0.76);
            Match match = new Match(team1, team2);
            Match match1 = new Match(team3, team4);
            Team winner = match.Play();
            Team winner1 = match1.Play();
            Console.WriteLine($"Winner: {winner.name}");
            Console.WriteLine($"Winner: {winner1.name}");
            Match match2 = new Match(winner, winner1);
            Console.WriteLine($"Final Winner: {match2.Play().name}");

        }*/
    }
}
