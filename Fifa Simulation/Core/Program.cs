using Fifa_Simulation.Groups;
using Fifa_Simulation.Helpers;
using Fifa_Simulation.Teams;
using Fifa_Simulation.Tournaments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    RunFifaSimulation();
                    break;

                case "2":
                    // RunIndividualMatch();
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
                Main();
            }
            else
            {
                Console.WriteLine("Exiting program. Goodbye!");
            }
        }

        static void RunFifaSimulation()
        {
            string folderPathSims = @"C:\Users\Joel\Desktop\Fifa Simulation\Output\Simulations";
            string folderPathSnapshots = @"C:\Users\Joel\Desktop\Fifa Simulation\Output\Snapshots";
            string baseName = "Sim";
            string snapshotBaseName = "Snapshots";
            string extension = ".txt";
            int counter = 1;

            if (!Directory.Exists(folderPathSims))
                Directory.CreateDirectory(folderPathSims);

            if(!Directory.Exists(folderPathSnapshots))
                Directory.CreateDirectory(folderPathSnapshots);

            while (File.Exists(Path.Combine(folderPathSims, $"{baseName} {counter}{extension}")) ||
                   File.Exists(Path.Combine(folderPathSnapshots, $"{snapshotBaseName} {counter}{extension}")))
            {
                counter++;
            }

            string simPath = Path.Combine(folderPathSims, $"{baseName} {counter}{extension}");
            string snapshotPath = Path.Combine(folderPathSnapshots, $"{snapshotBaseName} {counter}{extension}");

            List<SimulationSnapshot> snapshots = new();
            Dictionary<string, TeamStats> teamStatsByName = new();

            using (StreamWriter simWriter = new StreamWriter(simPath))
            {
                    simWriter.WriteLine($"\n\n################ FULL RUN {1} ################");

                    Tournament tournament = new Tournament();
                    SimulationSnapshot snapshot = tournament.RunThreeSimulations(simWriter, 1);

                    snapshots.Add(snapshot);
                    UpdateTeamStatsFromSnapshot(snapshot, teamStatsByName);
            }

            using (StreamWriter snapshotWriter = new StreamWriter(snapshotPath))
            {
                WriteSnapshots(snapshotWriter, snapshots);
                WriteTeamStats(snapshotWriter, teamStatsByName);
            }

            Console.WriteLine($"Simulation complete! Results saved to: {simPath}");
            Console.WriteLine($"Snapshot results saved to: {snapshotPath}");
            Console.ReadLine();
        }

        static TeamStats GetOrCreateTeamStats(string teamName, Dictionary<string, TeamStats> teamStatsByName)
        {
            if (!teamStatsByName.TryGetValue(teamName, out TeamStats stats))
            {
                stats = new TeamStats
                {
                    TeamName = teamName
                };
                teamStatsByName[teamName] = stats;
            }

            return stats;
        }

        static void UpdateTeamStatsFromSnapshot(SimulationSnapshot snapshot, Dictionary<string, TeamStats> teamStatsByName)
        {
            UpdateEventStats(snapshot.Major1Top4, teamStatsByName, 1);
            UpdateEventStats(snapshot.Major2Top4, teamStatsByName, 2);
            UpdateEventStats(snapshot.Major3Top4, teamStatsByName, 3);
            UpdateEventStats(snapshot.WorldsTop4, teamStatsByName, 4);
        }

        static void UpdateEventStats(List<Team> top4, Dictionary<string, TeamStats> teamStatsByName, int eventNumber)
        {
            if (top4 == null || top4.Count == 0)
                return;

            for (int i = 0; i < top4.Count; i++)
            {
                Team team = top4[i];
                TeamStats stats = GetOrCreateTeamStats(team.name, teamStatsByName);

                switch (eventNumber)
                {
                    case 1:
                        stats.Major1Top4s++;
                        if (i == 0) stats.Major1Wins++;
                        break;

                    case 2:
                        stats.Major2Top4s++;
                        if (i == 0) stats.Major2Wins++;
                        break;

                    case 3:
                        stats.Major3Top4s++;
                        if (i == 0) stats.Major3Wins++;
                        break;

                    case 4:
                        stats.WorldsTop4s++;
                        if (i == 0) stats.WorldsWins++;
                        break;
                }
            }
        }

        static void WriteSnapshots(StreamWriter writer, List<SimulationSnapshot> snapshots)
        {
            writer.WriteLine("==================================================");
            writer.WriteLine("SIMULATION SNAPSHOTS");
            writer.WriteLine("==================================================");

            foreach (var snapshot in snapshots)
            {
                writer.WriteLine($"\n================ RUN {snapshot.SimulationNumber} ================");

                WriteTop4(writer, "Major 1 Top 4", snapshot.Major1Top4);
                WriteTop4(writer, "Major 2 Top 4", snapshot.Major2Top4);
                WriteTop4(writer, "Major 3 Top 4", snapshot.Major3Top4);
                WriteTop4(writer, "Worlds Top 4", snapshot.WorldsTop4);
            }
        }

        static void WriteTop4(StreamWriter writer, string title, List<Team> teams)
        {
            writer.WriteLine($"\n{title}:");

            if (teams == null || teams.Count == 0)
            {
                writer.WriteLine("No teams recorded.");
                return;
            }

            for (int i = 0; i < teams.Count; i++)
            {
                writer.WriteLine($"{i + 1}. {teams[i].name} [{teams[i].Region}]");
            }
        }

        static void WriteTeamStats(StreamWriter writer, Dictionary<string, TeamStats> teamStatsByName)
        {
            writer.WriteLine("\n==================================================");
            writer.WriteLine("AGGREGATE TEAM STATS");
            writer.WriteLine("==================================================");

            var ordered = teamStatsByName.Values
                .OrderByDescending(t => t.WorldsWins)
                .ThenByDescending(t => t.WorldsTop4s)
                .ThenByDescending(t => t.Major1Wins + t.Major2Wins + t.Major3Wins)
                .ThenByDescending(t => t.Major1Top4s + t.Major2Top4s + t.Major3Top4s)
                .ThenBy(t => t.TeamName)
                .ToList();

            foreach (var team in ordered)
            {
                writer.WriteLine($"\n{team.TeamName}");
                writer.WriteLine($"  Major 1  - Wins: {team.Major1Wins}, Top 4s: {team.Major1Top4s}");
                writer.WriteLine($"  Major 2  - Wins: {team.Major2Wins}, Top 4s: {team.Major2Top4s}");
                writer.WriteLine($"  Major 3  - Wins: {team.Major3Wins}, Top 4s: {team.Major3Top4s}");
                writer.WriteLine($"  Worlds   - Wins: {team.WorldsWins}, Top 4s: {team.WorldsTop4s}");
            }
        }
    }
}