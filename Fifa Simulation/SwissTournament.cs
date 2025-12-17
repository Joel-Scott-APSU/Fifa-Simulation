using System;
using System.Collections.Generic;
using System.Linq;

namespace Fifa_Simulation
{
    public class SwissTournament
    {
        public List<Team> ActiveTeams { get; private set; }
        public List<Team> AdvancedTeams { get; } = new();
        public List<Team> EliminatedTeams { get; } = new();

        private readonly List<string> matchLog = new();

        public SwissTournament(List<Team> seededTeams)
        {
            ActiveTeams = new List<Team>(seededTeams);
        }


        public void Run()
        {
            for (int round = 1; round <= 5 && AdvancedTeams.Count < 8; round++)
            {
                var recordGroups = ActiveTeams
                    .GroupBy(t => (t.Wins, t.Losses))
                    .Select(g => g.OrderBy(t => t.Seed).ToList())
                    .ToList();

                foreach (var group in recordGroups)
                {
                    if (group.Count >= 2)
                        RunRound(PairBySeed(group));
                }
            }
        }

        private void RunRound(List<(Team, Team)> pairings)
        {
            foreach (var (a, b) in pairings)
            {
                matchLog.Add(
                    $"Seed {a.Seed} ({a.Wins}-{a.Losses}) vs Seed {b.Seed} ({b.Wins}-{b.Losses})"
                );

                new Match(a, b).Play();
            }

            ResolveTeams();

        }

        private void ResolveTeams()
        {
            foreach (var team in ActiveTeams.ToList())
            {
                if (team.Wins == 3)
                {
                    AdvancedTeams.Add(team);
                    ActiveTeams.Remove(team);
                }
                else if (team.Losses == 3)
                {
                    EliminatedTeams.Add(team);
                    ActiveTeams.Remove(team);
                }
            }
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

        public void DisplaySwissResults()
        {
            Console.WriteLine("\n--- ADVANCED FROM SWISS ---");
            foreach (var t in AdvancedTeams.OrderBy(t => t.Wins).ThenBy(t => t.Losses))
                Console.WriteLine($"{t.name} ({t.Wins}-{t.Losses})");

            Console.WriteLine("\n--- ELIMINATED IN SWISS ---");
            foreach (var t in EliminatedTeams.OrderBy(t => t.Wins).ThenBy(t => t.Losses))
                Console.WriteLine($"{t.name} ({t.Wins}-{t.Losses})");
        }

        public void DisplaySwissMatchLog()
        {
            Console.WriteLine("\n--- SWISS MATCH LOG ---");
            foreach (var entry in matchLog)
                Console.WriteLine(entry);
        }

        public void SwissPlacement()
        {
            foreach(Team team in EliminatedTeams)
            {
                if(team.Wins == 0)
                {
                    team.Seed = 15;
                }
                else if(team.Wins == 1)
                {
                    team.Seed = 12;
                }
                else
                {
                    team.Seed = 9;
                }
            }
        }
    }
}
