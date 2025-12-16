using System.Collections.Generic;

namespace Fifa_Simulation
{
    class Program
    {
        static void Main()
        {
            // Create groups using your existing TeamFactory
            TeamFactory.CreateGroups(
                out List<Team> groupA,
                out List<Team> groupB,
                out List<Team> groupC,
                out List<Team> groupD
            );

            // Wrap into Group objects
            var groups = new List<Group>
            {
                new Group("Group A", groupA),
                new Group("Group B", groupB),
                new Group("Group C", groupC),
                new Group("Group D", groupD)
            };

            // Run the tournament (3 simulations, escalating points)
            Tournament tournament = new Tournament(groups);
            tournament.RunThreeSimulations();
            Console.ReadLine();
        }
    }
}
