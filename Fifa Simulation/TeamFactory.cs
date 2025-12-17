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
    new Team("New England Revolution", 2200),
    new Team("Inter Miami", 2180),
    new Team("Soccer Aid", 2160),
    new Team("adidas All Stars", 2140)
};

        // Remaining 60 male pro teams, Elo capped at 2020
        private static readonly List<Team> ProTeams = new()
{
    new Team("Paris Saint-Germain", 2020),
    new Team("Bayern Munich", 2018),
    new Team("Manchester City", 2017),
    new Team("Liverpool", 2016),
    new Team("Real Madrid", 2015),
    new Team("Barcelona", 2014),
    new Team("Chelsea FC", 2013),
    new Team("Manchester United", 2012),
    new Team("Atletico Madrid", 2011),
    new Team("Juventus", 2010),
    new Team("Inter Milan", 2009),
    new Team("AC Milan", 2008),
    new Team("Ajax", 2007),
    new Team("RB Leipzig", 2006),
    new Team("Napoli", 2005),
    new Team("Sevilla FC", 2004),
    new Team("Atalanta", 2003),
    new Team("Sporting CP", 2002),
    new Team("Benfica", 2001),
    new Team("FC Porto", 2000),
    new Team("Borussia Dortmund", 1999),
    new Team("AS Roma", 1998),
    new Team("Olympique Lyonnais", 1997),
    new Team("AS Monaco", 1996),
    new Team("Bayer Leverkusen", 1995),
    new Team("Tottenham Hotspur", 1994),
    new Team("Leicester City", 1993),
    new Team("West Ham United", 1992),
    new Team("Aston Villa", 1991),
    new Team("Wolverhampton Wanderers", 1990),
    new Team("Real Sociedad", 1989),
    new Team("Villarreal", 1988),
    new Team("Fiorentina", 1987),
    new Team("Sassuolo", 1986),
    new Team("Monchengladbach", 1985),
    new Team("Schalke 04", 1984),
    new Team("Eintracht Frankfurt", 1983),
    new Team("Valencia CF", 1982),
    new Team("Real Betis", 1981),
    new Team("Celtic", 1980),
    new Team("Rangers", 1979),
    new Team("PSV Eindhoven", 1978),
    new Team("Feyenoord", 1977),
    new Team("Marseille", 1976),
    new Team("Nice", 1975),
    new Team("Lille", 1974),
    new Team("Lazio", 1973),
    new Team("Torino", 1972),
    new Team("Sampdoria", 1971),
    new Team("Udinese", 1970),
    new Team("Genoa", 1969),
    new Team("Hertha Berlin", 1968),
    new Team("Union Berlin", 1967),
    new Team("Hoffenheim", 1966),
    new Team("Stuttgart", 1965),
    new Team("Real Valladolid", 1964),
    new Team("Espanyol", 1963),
    new Team("Girona FC", 1962),
    new Team("Mallorca", 1961),
    new Team("Cagliari", 1960),
    new Team("Spezia", 1959),
    new Team("Salernitana", 1958),
    new Team("Bologna", 1957)
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
