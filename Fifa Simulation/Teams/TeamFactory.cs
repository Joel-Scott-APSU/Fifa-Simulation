using Fifa_Simulation.Teams;
using System.Collections.Generic;

namespace Fifa_Simulation.Teams
{
    public class TeamFactory
    {
        public static List<Team> GetWesternEuropeTeams() => WesternEuropeTeams;
        public static List<Team> GetCentralEuropeTeams() => CentralEuropeTeams;
        public static List<Team> GetItalyTeams() => ItalyTeams;
        public static List<Team> GetEasternEuropeTeams() => EasternEuropeTeams;
        public static List<Team> GetSouthAmericaTeams() => SouthAmericaTeams;
        public static List<Team> GetAmericasTeams() => AmericasTeams;
        public static List<Team> GetEastAsiaTeams() => EastAsiaTeams;
        public static List<Team> GetMiddleEastTeams() => MiddleEastTeams;
        public static List<Team> GetAfricaTeams() => AfricaTeams;
        public static List<Team> GetWildcardTeams() => WildcardTeams;

        // WESTERN EUROPE (64)
        private static readonly List<Team> WesternEuropeTeams = new()
        {
            new Team("Manchester City", 2050, "Western Europe"),
            new Team("Real Madrid", 2030, "Western Europe"),
            new Team("Bayern Munich", 2010, "Western Europe"),
            new Team("Liverpool", 1985, "Western Europe"),
            new Team("Barcelona", 1975, "Western Europe"),
            new Team("Arsenal", 1965, "Western Europe"),
            new Team("Paris Saint-Germain", 1955, "Western Europe"),
            new Team("Atletico Madrid", 1945, "Western Europe"),

            new Team("Borussia Dortmund", 1925, "Western Europe"),
            new Team("Bayer Leverkusen", 1905, "Western Europe"),
            new Team("Chelsea", 1895, "Western Europe"),
            new Team("Tottenham Hotspur", 1885, "Western Europe"),
            new Team("RB Leipzig", 1875, "Western Europe"),
            new Team("Manchester United", 1865, "Western Europe"),
            new Team("Newcastle United", 1855, "Western Europe"),
            new Team("Aston Villa", 1840, "Western Europe"),

            new Team("Real Sociedad", 1830, "Western Europe"),
            new Team("Villarreal", 1820, "Western Europe"),
            new Team("Sevilla", 1810, "Western Europe"),
            new Team("Athletic Bilbao", 1800, "Western Europe"),
            new Team("Brighton", 1790, "Western Europe"),
            new Team("West Ham United", 1780, "Western Europe"),
            new Team("Eintracht Frankfurt", 1770, "Western Europe"),
            new Team("Wolfsburg", 1760, "Western Europe"),

            new Team("Stuttgart", 1750, "Western Europe"),
            new Team("Lens", 1740, "Western Europe"),
            new Team("Lille", 1730, "Western Europe"),
            new Team("Monaco", 1720, "Western Europe"),
            new Team("Marseille", 1710, "Western Europe"),
            new Team("Lyon", 1700, "Western Europe"),
            new Team("Rennes", 1690, "Western Europe"),
            new Team("Nice", 1680, "Western Europe"),

            new Team("Real Betis", 1670, "Western Europe"),
            new Team("Valencia", 1660, "Western Europe"),
            new Team("Girona", 1650, "Western Europe"),
            new Team("Union Berlin", 1640, "Western Europe"),
            new Team("Freiburg", 1630, "Western Europe"),
            new Team("AZ Alkmaar", 1620, "Western Europe"),
            new Team("Feyenoord", 1610, "Western Europe"),
            new Team("Ajax", 1600, "Western Europe"),

            new Team("PSV Eindhoven", 1595, "Western Europe"),
            new Team("Club Brugge", 1590, "Western Europe"),
            new Team("Anderlecht", 1585, "Western Europe"),
            new Team("Genk", 1580, "Western Europe"),
            new Team("Gent", 1575, "Western Europe"),
            new Team("Celtic", 1570, "Western Europe"),
            new Team("Rangers", 1565, "Western Europe"),
            new Team("Utrecht", 1560, "Western Europe"),

            new Team("Heerenveen", 1555, "Western Europe"),
            new Team("Standard Liege", 1550, "Western Europe"),
            new Team("Charleroi", 1545, "Western Europe"),
            new Team("Montpellier", 1540, "Western Europe"),
            new Team("Nantes", 1535, "Western Europe"),
            new Team("Toulouse", 1530, "Western Europe"),
            new Team("Boavista", 1525, "Western Europe"),
            new Team("Vitoria Guimaraes", 1520, "Western Europe"),

            new Team("St Pauli", 1515, "Western Europe"),
            new Team("Sunderland", 1510, "Western Europe"),
            new Team("Middlesbrough", 1505, "Western Europe"),
            new Team("Braga", 1500, "Western Europe"),
            new Team("Sporting CP", 1495, "Western Europe"),
            new Team("Benfica", 1490, "Western Europe"),
            new Team("Burnley", 1485, "Western Europe"),
            new Team("Norwich City", 1480, "Western Europe")
        };

        // CENTRAL EUROPE (32)
        private static readonly List<Team> CentralEuropeTeams = new()
        {
            new Team("Red Bull Salzburg", 1810, "Central Europe"),
            new Team("Young Boys", 1750, "Central Europe"),
            new Team("Dinamo Zagreb", 1740, "Central Europe"),
            new Team("Slavia Prague", 1735, "Central Europe"),
            new Team("Sparta Prague", 1730, "Central Europe"),
            new Team("Viktoria Plzen", 1715, "Central Europe"),
            new Team("Ferencvaros", 1705, "Central Europe"),
            new Team("Rapid Vienna", 1695, "Central Europe"),

            new Team("Sturm Graz", 1685, "Central Europe"),
            new Team("Basel", 1680, "Central Europe"),
            new Team("FC Zurich", 1670, "Central Europe"),
            new Team("Austria Vienna", 1660, "Central Europe"),
            new Team("Servette", 1650, "Central Europe"),
            new Team("Grasshopper", 1640, "Central Europe"),
            new Team("Legia Warsaw", 1635, "Central Europe"),
            new Team("Lech Poznan", 1630, "Central Europe"),

            new Team("Rakow Czestochowa", 1625, "Central Europe"),
            new Team("CFR Cluj", 1620, "Central Europe"),
            new Team("FCSB", 1615, "Central Europe"),
            new Team("Universitatea Craiova", 1610, "Central Europe"),
            new Team("Slovan Bratislava", 1605, "Central Europe"),
            new Team("DAC Dunajska Streda", 1600, "Central Europe"),
            new Team("Hajduk Split", 1595, "Central Europe"),
            new Team("Rijeka", 1590, "Central Europe"),

            new Team("Osijek", 1585, "Central Europe"),
            new Team("Maribor", 1580, "Central Europe"),
            new Team("Olimpija Ljubljana", 1575, "Central Europe"),
            new Team("Banik Ostrava", 1570, "Central Europe"),
            new Team("Debrecen", 1565, "Central Europe"),
            new Team("Wisla Krakow", 1560, "Central Europe"),
            new Team("Piast Gliwice", 1555, "Central Europe"),
            new Team("Fehervar", 1550, "Central Europe")
        };

        // ITALY (16)
        private static readonly List<Team> ItalyTeams = new()
        {
            new Team("Inter Milan", 1950, "Italy"),
            new Team("Juventus", 1910, "Italy"),
            new Team("AC Milan", 1890, "Italy"),
            new Team("Napoli", 1880, "Italy"),
            new Team("Atalanta", 1860, "Italy"),
            new Team("Roma", 1850, "Italy"),
            new Team("Lazio", 1840, "Italy"),
            new Team("Fiorentina", 1810, "Italy"),

            new Team("Bologna", 1790, "Italy"),
            new Team("Torino", 1760, "Italy"),
            new Team("Udinese", 1740, "Italy"),
            new Team("Sassuolo", 1730, "Italy"),
            new Team("Genoa", 1700, "Italy"),
            new Team("Parma", 1680, "Italy"),
            new Team("Cagliari", 1670, "Italy"),
            new Team("Sampdoria", 1660, "Italy")
        };

        // EASTERN EUROPE (16)
        private static readonly List<Team> EasternEuropeTeams = new()
        {
            new Team("Shakhtar Donetsk", 1820, "Eastern Europe"),
            new Team("Zenit", 1800, "Eastern Europe"),
            new Team("Dynamo Kyiv", 1780, "Eastern Europe"),
            new Team("CSKA Moscow", 1760, "Eastern Europe"),
            new Team("Spartak Moscow", 1750, "Eastern Europe"),
            new Team("Lokomotiv Moscow", 1740, "Eastern Europe"),
            new Team("Red Star Belgrade", 1730, "Eastern Europe"),
            new Team("Partizan", 1710, "Eastern Europe"),

            new Team("Ludogorets", 1700, "Eastern Europe"),
            new Team("Sheriff Tiraspol", 1685, "Eastern Europe"),
            new Team("Qarabag", 1675, "Eastern Europe"),
            new Team("BATE Borisov", 1665, "Eastern Europe"),
            new Team("Dinamo Bucharest", 1650, "Eastern Europe"),
            new Team("PAOK", 1630, "Eastern Europe"),
            new Team("Astana", 1620, "Eastern Europe"),
            new Team("Levski Sofia", 1610, "Eastern Europe")
        };

        // SOUTH AMERICA (16)
        private static readonly List<Team> SouthAmericaTeams = new()
        {
            new Team("Palmeiras", 1940, "South America"),
            new Team("Flamengo", 1930, "South America"),
            new Team("River Plate", 1920, "South America"),
            new Team("Boca Juniors", 1910, "South America"),
            new Team("Sao Paulo", 1880, "South America"),
            new Team("Corinthians", 1870, "South America"),
            new Team("Internacional", 1860, "South America"),
            new Team("Gremio", 1850, "South America"),

            new Team("Racing Club", 1840, "South America"),
            new Team("Independiente", 1820, "South America"),
            new Team("San Lorenzo", 1810, "South America"),
            new Team("Atletico Nacional", 1790, "South America"),
            new Team("Colo Colo", 1780, "South America"),
            new Team("Penarol", 1770, "South America"),
            new Team("Nacional", 1760, "South America"),
            new Team("LDU Quito", 1750, "South America")
        };

        // AMERICAS (8)
        private static readonly List<Team> AmericasTeams = new()
        {
            new Team("Club America", 1840, "Americas"),
            new Team("Monterrey", 1830, "Americas"),
            new Team("Tigres UANL", 1820, "Americas"),
            new Team("LAFC", 1760, "Americas"),
            new Team("Seattle Sounders", 1750, "Americas"),
            new Team("LA Galaxy", 1730, "Americas"),
            new Team("New England Revolution", 2180, "Americas"),
            new Team("Inter Miami", 2150, "Americas")
        };

        // EAST ASIA (8)
        private static readonly List<Team> EastAsiaTeams = new()
        {
            new Team("Kawasaki Frontale", 1740, "East Asia"),
            new Team("Urawa Red Diamonds", 1730, "East Asia"),
            new Team("Kashima Antlers", 1720, "East Asia"),
            new Team("Yokohama F Marinos", 1710, "East Asia"),
            new Team("Jeonbuk Hyundai Motors", 1700, "East Asia"),
            new Team("Ulsan Hyundai", 1690, "East Asia"),
            new Team("Shanghai Port", 1670, "East Asia"),
            new Team("Shandong Taishan", 1660, "East Asia")
        };

        // MIDDLE EAST (4)
        private static readonly List<Team> MiddleEastTeams = new()
        {
            new Team("Al Hilal", 1780, "Middle East"),
            new Team("Al Nassr", 1750, "Middle East"),
            new Team("Al Ahli", 1730, "Middle East"),
            new Team("Al Sadd", 1710, "Middle East")
        };

        // AFRICA (4)
        private static readonly List<Team> AfricaTeams = new()
        {
            new Team("Al Ahly", 1770, "Africa"),
            new Team("Wydad Casablanca", 1740, "Africa"),
            new Team("Esperance Tunis", 1730, "Africa"),
            new Team("Zamalek", 1720, "Africa")
        };

        // WILDCARD (8)
        private static readonly List<Team> WildcardTeams = new()
        {
            new Team("Adidas All Stars", 2100, "Wildcard"),
            new Team("Soccer Aid", 2180, "Wildcard"),
            new Team("Galatasaray", 1830, "Wildcard"),
            new Team("Fenerbahce", 1820, "Wildcard"),
            new Team("Besiktas", 1780, "Wildcard"),
            new Team("Olympiacos", 1810, "Wildcard"),
            new Team("Panathinaikos", 1750, "Wildcard"),
            new Team("Maccabi Haifa", 1730, "Wildcard")
        };

        public static List<Team> GetAllTeams()
        {
            var allTeams = new List<Team>();

            allTeams.AddRange(GetWesternEuropeTeams());
            allTeams.AddRange(GetCentralEuropeTeams());
            allTeams.AddRange(GetItalyTeams());
            allTeams.AddRange(GetEasternEuropeTeams());
            allTeams.AddRange(GetSouthAmericaTeams());
            allTeams.AddRange(GetAmericasTeams());
            allTeams.AddRange(GetEastAsiaTeams());
            allTeams.AddRange(GetMiddleEastTeams());
            allTeams.AddRange(GetAfricaTeams());
            allTeams.AddRange(GetWildcardTeams());

            return allTeams;
        }
    }
}