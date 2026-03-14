using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fifa_Sim_Test
{
    public class TeamFactory
    {
        // WESTERN EUROPE (64)

        private static readonly List<Team> WesternEuropeTeams = new()
{
    new Team("Manchester City", 2050),
    new Team("Real Madrid", 2030),
    new Team("Bayern Munich", 2010),
    new Team("Liverpool", 1985),
    new Team("Barcelona", 1975),
    new Team("Arsenal", 1965),
    new Team("Paris Saint-Germain", 1955),
    new Team("Atletico Madrid", 1945),

    new Team("Borussia Dortmund", 1925),
    new Team("Bayer Leverkusen", 1905),
    new Team("Chelsea", 1895),
    new Team("Tottenham Hotspur", 1885),
    new Team("RB Leipzig", 1875),
    new Team("Manchester United", 1865),
    new Team("Newcastle United", 1855),
    new Team("Aston Villa", 1840),

    new Team("Real Sociedad", 1830),
    new Team("Villarreal", 1820),
    new Team("Sevilla", 1810),
    new Team("Athletic Bilbao", 1800),
    new Team("Brighton", 1790),
    new Team("West Ham United", 1780),
    new Team("Eintracht Frankfurt", 1770),
    new Team("Wolfsburg", 1760),

    new Team("Stuttgart", 1750),
    new Team("Lens", 1740),
    new Team("Lille", 1730),
    new Team("Monaco", 1720),
    new Team("Marseille", 1710),
    new Team("Lyon", 1700),
    new Team("Rennes", 1690),
    new Team("Nice", 1680),

    new Team("Real Betis", 1670),
    new Team("Valencia", 1660),
    new Team("Girona", 1650),
    new Team("Union Berlin", 1640),
    new Team("Freiburg", 1630),
    new Team("AZ Alkmaar", 1620),
    new Team("Feyenoord", 1610),
    new Team("Ajax", 1600),

    new Team("PSV Eindhoven", 1595),
    new Team("Club Brugge", 1590),
    new Team("Anderlecht", 1585),
    new Team("Genk", 1580),
    new Team("Gent", 1575),
    new Team("Celtic", 1570),
    new Team("Rangers", 1565),
    new Team("Utrecht", 1560),

    new Team("Heerenveen", 1555),
    new Team("Standard Liege", 1550),
    new Team("Charleroi", 1545),
    new Team("Montpellier", 1540),
    new Team("Nantes", 1535),
    new Team("Toulouse", 1530),
    new Team("Boavista", 1525),
    new Team("Vitoria Guimaraes", 1520),

    new Team("St Pauli", 1515),
    new Team("Sunderland", 1510),
    new Team("Middlesbrough", 1505)
};

        // CENTRAL EUROPE (32)

        private static readonly List<Team> CentralEuropeTeams = new()
{
    new Team("Red Bull Salzburg", 1810),
    new Team("Young Boys", 1750),
    new Team("Dinamo Zagreb", 1740),
    new Team("Slavia Prague", 1735),
    new Team("Sparta Prague", 1730),
    new Team("Viktoria Plzen", 1715),
    new Team("Ferencvaros", 1705),
    new Team("Rapid Vienna", 1695),

    new Team("Sturm Graz", 1685),
    new Team("Basel", 1680),
    new Team("FC Zurich", 1670),
    new Team("Austria Vienna", 1660),
    new Team("Servette", 1650),
    new Team("Grasshopper", 1640),
    new Team("Legia Warsaw", 1635),
    new Team("Lech Poznan", 1630),

    new Team("Rakow Czestochowa", 1625),
    new Team("CFR Cluj", 1620),
    new Team("FCSB", 1615),
    new Team("Universitatea Craiova", 1610),
    new Team("Slovan Bratislava", 1605),
    new Team("DAC Dunajska Streda", 1600),
    new Team("Hajduk Split", 1595),
    new Team("Rijeka", 1590),

    new Team("Osijek", 1585),
    new Team("Maribor", 1580),
    new Team("Olimpija Ljubljana", 1575),
    new Team("Banik Ostrava", 1570),
    new Team("Debrecen", 1565),
    new Team("Wisla Krakow", 1560),
    new Team("Piast Gliwice", 1555),
    new Team("Fehervar", 1550)
};

        // ITALY (16)

        private static readonly List<Team> ItalyTeams = new()
{
    new Team("Inter Milan", 1950),
    new Team("Juventus", 1910),
    new Team("AC Milan", 1890),
    new Team("Napoli", 1880),
    new Team("Atalanta", 1860),
    new Team("Roma", 1850),
    new Team("Lazio", 1840),
    new Team("Fiorentina", 1810),

    new Team("Bologna", 1790),
    new Team("Torino", 1760),
    new Team("Udinese", 1740),
    new Team("Sassuolo", 1730),
    new Team("Genoa", 1700),
    new Team("Parma", 1680),
    new Team("Cagliari", 1670),
    new Team("Sampdoria", 1660)
};

        // EASTERN EUROPE (16)

        private static readonly List<Team> EasternEuropeTeams = new()
{
    new Team("Shakhtar Donetsk", 1820),
    new Team("Zenit", 1800),
    new Team("Dynamo Kyiv", 1780),
    new Team("CSKA Moscow", 1760),
    new Team("Spartak Moscow", 1750),
    new Team("Lokomotiv Moscow", 1740),
    new Team("Red Star Belgrade", 1730),
    new Team("Partizan", 1710),

    new Team("Ludogorets", 1700),
    new Team("Sheriff Tiraspol", 1685),
    new Team("Qarabag", 1675),
    new Team("BATE Borisov", 1665),
    new Team("Dinamo Bucharest", 1650),
    new Team("PAOK", 1630),
    new Team("Astana", 1620),
    new Team("Levski Sofia", 1610)
};


        // SOUTH AMERICA (16)

        private static readonly List<Team> SouthAmericaTeams = new()
{
    new Team("Palmeiras", 1940),
    new Team("Flamengo", 1930),
    new Team("River Plate", 1920),
    new Team("Boca Juniors", 1910),
    new Team("Sao Paulo", 1880),
    new Team("Corinthians", 1870),
    new Team("Internacional", 1860),
    new Team("Gremio", 1850),

    new Team("Racing Club", 1840),
    new Team("Independiente", 1820),
    new Team("San Lorenzo", 1810),
    new Team("Atletico Nacional", 1790),
    new Team("Colo Colo", 1780),
    new Team("Penarol", 1770),
    new Team("Nacional", 1760),
    new Team("LDU Quito", 1750)
};

        // AMERICAS (8)

        private static readonly List<Team> AmericasTeams = new()
{
    new Team("Club America", 1840),
    new Team("Monterrey", 1830),
    new Team("Tigres UANL", 1820),
    new Team("LAFC", 1760),
    new Team("Seattle Sounders", 1750),
    new Team("LA Galaxy", 1730),
    new Team("New England Revolution", 2250),
    new Team("Inter Miami", 2230)
};

        // EAST ASIA (8)

        private static readonly List<Team> EastAsiaTeams = new()
{
    new Team("Kawasaki Frontale", 1740),
    new Team("Urawa Red Diamonds", 1730),
    new Team("Kashima Antlers", 1720),
    new Team("Yokohama F Marinos", 1710),
    new Team("Jeonbuk Hyundai Motors", 1700),
    new Team("Ulsan Hyundai", 1690),
    new Team("Shanghai Port", 1670),
    new Team("Shandong Taishan", 1660)
};

        // MIDDLE EAST (4)

        private static readonly List<Team> MiddleEastTeams = new()
{
    new Team("Al Hilal", 1780),
    new Team("Al Nassr", 1750),
    new Team("Al Ahli", 1730),
    new Team("Al Sadd", 1710)
};

        // AFRICA (4)

        private static readonly List<Team> AfricaTeams = new()
{
    new Team("Al Ahly", 1770),
    new Team("Wydad Casablanca", 1740),
    new Team("Esperance Tunis", 1730),
    new Team("Zamalek", 1720)
};

        private static readonly List<Team> WildcardTeams = new()
{
    new Team("Adidas All Stars", 2100),
    new Team("Soccer Aid", 2180),
    new Team("Galatasaray", 1830),
    new Team("Fenerbahce", 1820),
    new Team("Besiktas", 1780),
    new Team("Olympiacos", 1810),
    new Team("Panathinaikos", 1750),
    new Team("Maccabi Haifa", 1730)
};
    }
}
