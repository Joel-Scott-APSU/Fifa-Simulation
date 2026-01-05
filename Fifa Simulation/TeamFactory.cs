using Fifa_Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fifa_Simulation
{
    public static class TeamFactory
    {
        private static readonly Random rng = new Random();

        // Top 4 special teams
        private static readonly List<Team> Top4 = new()
        {
            new Team("New England Revolution", 2200, 0.88),
            new Team("Inter Miami", 2180, 0.84),
            new Team("Soccer Aid", 2160, 0.80),
            new Team("adidas All Stars", 2140, 0.78)
        };

        // Remaining pro teams (lowest 3 Elo removed)
        private static readonly List<Team> ProTeams = new()
        {
            new Team("Paris Saint-Germain", 2020, 0.76),
            new Team("Bayern Munich", 2018, 0.78),
            new Team("Manchester City", 2017, 0.79),
            new Team("Liverpool", 2016, 0.77),
            new Team("Real Madrid", 2015, 0.80),
            new Team("Barcelona", 2014, 0.74),
            new Team("Chelsea FC", 2013, 0.71),
            new Team("Manchester United", 2012, 0.70),
            new Team("Atletico Madrid", 2011, 0.75),
            new Team("Juventus", 2010, 0.73),
            new Team("Inter Milan", 2009, 0.74),
            new Team("AC Milan", 2008, 0.72),
            new Team("Ajax", 2007, 0.69),
            new Team("RB Leipzig", 2006, 0.67),
            new Team("Napoli", 2005, 0.71),
            new Team("Sevilla FC", 2004, 0.68),
            new Team("Atalanta", 2003, 0.66),
            new Team("Sporting CP", 2002, 0.65),
            new Team("Benfica", 2001, 0.67),
            new Team("FC Porto", 2000, 0.68),
            new Team("Borussia Dortmund", 1999, 0.64),
            new Team("AS Roma", 1998, 0.66),
            new Team("Olympique Lyonnais", 1997, 0.63),
            new Team("AS Monaco", 1996, 0.64),
            new Team("Bayer Leverkusen", 1995, 0.66),
            new Team("Tottenham Hotspur", 1994, 0.62),
            new Team("Leicester City", 1993, 0.58),
            new Team("West Ham United", 1992, 0.57),
            new Team("Aston Villa", 1991, 0.56),
            new Team("Wolverhampton Wanderers", 1990, 0.55),
            new Team("Real Sociedad", 1989, 0.60),
            new Team("Villarreal", 1988, 0.59),
            new Team("Fiorentina", 1987, 0.57),
            new Team("Sassuolo", 1986, 0.54),
            new Team("Monchengladbach", 1985, 0.56),
            new Team("Schalke 04", 1984, 0.50),
            new Team("Eintracht Frankfurt", 1983, 0.58),
            new Team("Valencia CF", 1982, 0.55),
            new Team("Real Betis", 1981, 0.57),
            new Team("Celtic", 1980, 0.61),
            new Team("Rangers", 1979, 0.60),
            new Team("PSV Eindhoven", 1978, 0.62),
            new Team("Feyenoord", 1977, 0.61),
            new Team("Marseille", 1976, 0.54),
            new Team("Nice", 1975, 0.52),
            new Team("Lille", 1974, 0.53),
            new Team("Lazio", 1973, 0.56),
            new Team("Torino", 1972, 0.49),
            new Team("Sampdoria", 1971, 0.47),
            new Team("Udinese", 1970, 0.48),
            new Team("Genoa", 1969, 0.46),
            new Team("Hertha Berlin", 1968, 0.45),
            new Team("Union Berlin", 1967, 0.50),
            new Team("Hoffenheim", 1966, 0.49),
            new Team("Stuttgart", 1965, 0.47),
            new Team("Real Valladolid", 1964, 0.44),
            new Team("Espanyol", 1963, 0.45),
            new Team("Girona FC", 1962, 0.46),
            new Team("Mallorca", 1961, 0.44),
            new Team("Cagliari", 1960, 0.43)
        };

        public static void CreateGroups(out List<Team> groupA,
                                        out List<Team> groupB,
                                        out List<Team> groupC,
                                        out List<Team> groupD)
        {
            groupA = new List<Team> { Top4[0] };
            groupB = new List<Team> { Top4[1] };
            groupC = new List<Team> { Top4[2] };
            groupD = new List<Team> { Top4[3] };

            // Shuffle remaining pro teams
            List<Team> shuffled = ProTeams.OrderBy(x => rng.Next()).ToList();

            // Round-robin distribution into 4 groups
            int index = 0;
            foreach (var team in shuffled)
            {
                switch (index)
                {
                    case 0: groupA.Add(team); break;
                    case 1: groupB.Add(team); break;
                    case 2: groupC.Add(team); break;
                    case 3: groupD.Add(team); break;
                }
                index = (index + 1) % 4;
            }
        }
    }
}
