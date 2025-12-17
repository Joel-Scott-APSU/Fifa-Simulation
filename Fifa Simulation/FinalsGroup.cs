using Fifa_Simulation;

public class FinalsGroup
{
    public string Name { get; }
    public List<Team> Teams { get; }
    private readonly HeadToHead h2h = new();

    public FinalsGroup(string name, List<Team> teams)
    {
        Name = name;
        Teams = teams;

        // Reset group stats
        foreach (var t in Teams)
        {
            t.Wins = 0;
            t.Losses = 0;
        }
    }

    public void Run()
    {
        Console.WriteLine($"\n=== GROUP {Name} ===");

        for (int i = 0; i < Teams.Count; i++)
        {
            for (int j = i + 1; j < Teams.Count; j++)
            {
                PlayDoubleMatch(Teams[i], Teams[j]);
            }
        }

        SeedGroup();
        PrintStandings();
    }

    private void PlayDoubleMatch(Team a, Team b)
    {
        for (int i = 0; i < 2; i++)
        {
            Team winner = new Match(a, b).Play();
            Team loser = winner == a ? b : a;

            h2h.RecordWin(winner, loser);

            Console.WriteLine($"{a.name} vs {b.name} --- {winner.name}");
        }
    }


    private void SeedGroup()
    {
        Teams.Sort((a, b) =>
        {
            // 1️⃣ Wins
            int cmp = b.Wins.CompareTo(a.Wins);
            if (cmp != 0) return cmp;

            // 2️⃣ Head-to-head
            int h2hCmp = h2h.GetWins(a, b).CompareTo(h2h.GetWins(b, a));
            if (h2hCmp != 0) return -h2hCmp;

            // 3️⃣ Original finals seeding
            return a.Seed.CompareTo(b.Seed);
        });

        // Assign final group placement
        for (int i = 0; i < Teams.Count; i++)
            Teams[i].Seed = i + 1;
    }

    private void PrintStandings()
    {
        Console.WriteLine($"\n--- GROUP {Name} STANDINGS ---");
        for (int i = 0; i < Teams.Count; i++)
        {
            var t = Teams[i];
            Console.WriteLine(
                $"{i + 1}. {t.name}  W:{t.Wins} L:{t.Losses}"
            );
        }
    }


}
