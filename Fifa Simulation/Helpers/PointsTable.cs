using Fifa_Simulation.Teams;
using System;

namespace Fifa_Simulation.Helpers
{
    public static class PointsTable
    {
        // REGIONAL EVENTS
        public static readonly int[] RegionalTop64LowerPoints = { 2, 3, 4 };   // 49-64
        public static readonly int[] RegionalTop64UpperPoints = { 4, 5, 6 };   // 33-48
        public static readonly int[] RegionalTop32Points = { 6, 8, 10 };       // 17-32 / 25-32 depending on format
        public static readonly int[] RegionalTop24Points = { 8, 10, 12 };      // 17-24
        public static readonly int[] RegionalTop16Points = { 10, 13, 16 };     // 9-16 / 13-16
        public static readonly int[] RegionalTop12Points = { 12, 15, 18 };     // 9-12
        public static readonly int[] RegionalTop8Points = { 14, 18, 22 };      // 5-8 / 7-8
        public static readonly int[] RegionalTop6Points = { 16, 20, 24 };      // 5-6
        public static readonly int[] RegionalTop4Points = { 18, 24, 30 };      // 3-4 / 4th
        public static readonly int[] RegionalThirdPoints = { 22, 28, 34 };     // 3rd
        public static readonly int[] RegionalSecondPoints = { 26, 34, 42 };    // 2nd
        public static readonly int[] RegionalWinnerPoints = { 32, 42, 52 };    // 1st

        // MAJORS / WORLDS QUALIFICATION
        public static readonly int[] MajorTop32LowerPoints = { 4, 6, 8 };      // 25-32
        public static readonly int[] MajorTop32UpperPoints = { 8, 10, 12 };    // 17-24
        public static readonly int[] MajorTop16Points = { 14, 18, 22 };        // 13-16
        public static readonly int[] MajorTop12Points = { 20, 24, 28 };        // 9-12
        public static readonly int[] MajorTop8Points = { 28, 34, 40 };         // 7-8 / QF
        public static readonly int[] MajorTop6Points = { 36, 44, 52 };         // 5-6
        public static readonly int[] MajorTop4Points = { 46, 56, 66 };         // 3-4 / 4th
        public static readonly int[] MajorThirdPoints = { 58, 70, 82 };        // 3rd
        public static readonly int[] MajorSecondPoints = { 70, 84, 98 };       // 2nd
        public static readonly int[] MajorWinnerPoints = { 90, 108, 126 };     // 1st
    }

    public static class PointsAwarder
    {
        // MAJORS
        public static void AwardPoints(Team team, int sim)
        {
            switch (team.Seed)
            {
                case 1:
                    team.TotalPoints += PointsTable.MajorWinnerPoints[sim];
                    break;
                case 2:
                    team.TotalPoints += PointsTable.MajorSecondPoints[sim];
                    break;
                case 3:
                    team.TotalPoints += PointsTable.MajorThirdPoints[sim];
                    break;
                case 4:
                    team.TotalPoints += PointsTable.MajorTop4Points[sim];
                    break;
                case 6:
                    team.TotalPoints += PointsTable.MajorTop6Points[sim];
                    break;
                case 8:
                    team.TotalPoints += PointsTable.MajorTop8Points[sim];
                    break;
                case 12:
                    team.TotalPoints += PointsTable.MajorTop12Points[sim];
                    break;
                case 16:
                    team.TotalPoints += PointsTable.MajorTop16Points[sim];
                    break;
                case 24:
                    team.TotalPoints += PointsTable.MajorTop32UpperPoints[sim];
                    break;
                case 32:
                    team.TotalPoints += PointsTable.MajorTop32LowerPoints[sim];
                    break;
            }
        }

        // ALL REGIONS SHARE THE SAME TOP-END VALUES
        public static void Award64TeamRegionPoints(Team team, int sim)
        {
            switch (team.Seed)
            {
                case 1:
                    team.RegionPoints += PointsTable.RegionalWinnerPoints[sim];
                    team.TotalPoints += PointsTable.RegionalWinnerPoints[sim];
                    break;
                case 2:
                    team.RegionPoints += PointsTable.RegionalSecondPoints[sim];
                    team.TotalPoints += PointsTable.RegionalSecondPoints[sim];
                    break;
                case 4:
                    team.RegionPoints += PointsTable.RegionalTop4Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop4Points[sim];
                    break;
                case 8:
                    team.RegionPoints += PointsTable.RegionalTop8Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop8Points[sim];
                    break;
                case 16:
                    team.RegionPoints += PointsTable.RegionalTop16Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop16Points[sim];
                    break;
                case 32:
                    team.RegionPoints += PointsTable.RegionalTop32Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop32Points[sim];
                    break;
                case 48:
                    team.RegionPoints += PointsTable.RegionalTop64UpperPoints[sim];
                    team.TotalPoints += PointsTable.RegionalTop64UpperPoints[sim];
                    break;
                case 64:
                    team.RegionPoints += PointsTable.RegionalTop64LowerPoints[sim];
                    team.TotalPoints += PointsTable.RegionalTop64LowerPoints[sim];
                    break;
            }
        }

        public static void Award32TeamRegionPoints(Team team, int sim)
        {
            switch (team.Seed)
            {
                case 1:
                    team.RegionPoints += PointsTable.RegionalWinnerPoints[sim];
                    team.TotalPoints += PointsTable.RegionalWinnerPoints[sim];
                    break;
                case 2:
                    team.RegionPoints += PointsTable.RegionalSecondPoints[sim];
                    team.TotalPoints += PointsTable.RegionalSecondPoints[sim];
                    break;
                case 4:
                    team.RegionPoints += PointsTable.RegionalTop4Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop4Points[sim];
                    break;
                case 8:
                    team.RegionPoints += PointsTable.RegionalTop8Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop8Points[sim];
                    break;
                case 16:
                    team.RegionPoints += PointsTable.RegionalTop16Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop16Points[sim];
                    break;
                case 24:
                    team.RegionPoints += PointsTable.RegionalTop24Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop24Points[sim];
                    break;
                case 32:
                    team.RegionPoints += PointsTable.RegionalTop32Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop32Points[sim];
                    break;
            }
        }

        public static void Award16TeamRegionPoints(Team team, int sim)
        {
            switch (team.Seed)
            {
                case 1:
                    team.RegionPoints += PointsTable.RegionalWinnerPoints[sim];
                    team.TotalPoints += PointsTable.RegionalWinnerPoints[sim];
                    break;
                case 2:
                    team.RegionPoints += PointsTable.RegionalSecondPoints[sim];
                    team.TotalPoints += PointsTable.RegionalSecondPoints[sim];
                    break;
                case 4:
                    team.RegionPoints += PointsTable.RegionalTop4Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop4Points[sim];
                    break;
                case 8:
                    team.RegionPoints += PointsTable.RegionalTop8Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop8Points[sim];
                    break;
                case 12:
                    team.RegionPoints += PointsTable.RegionalTop12Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop12Points[sim];
                    break;
                case 16:
                    team.RegionPoints += PointsTable.RegionalTop16Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop16Points[sim];
                    break;
            }
        }

        public static void Award8TeamRegionPoints(Team team, int sim)
        {
            switch (team.Seed)
            {
                case 1:
                    team.RegionPoints += PointsTable.RegionalWinnerPoints[sim];
                    team.TotalPoints += PointsTable.RegionalWinnerPoints[sim];
                    break;
                case 2:
                    team.RegionPoints += PointsTable.RegionalSecondPoints[sim];
                    team.TotalPoints += PointsTable.RegionalSecondPoints[sim];
                    break;
                case 4:
                    team.RegionPoints += PointsTable.RegionalTop4Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop4Points[sim];
                    break;
                case 6:
                    team.RegionPoints += PointsTable.RegionalTop6Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop6Points[sim];
                    break;
                case 8:
                    team.RegionPoints += PointsTable.RegionalTop8Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop8Points[sim];
                    break;
            }
        }

        public static void Award4TeamRegionPoints(Team team, int sim)
        {
            switch (team.Seed)
            {
                case 1:
                    team.RegionPoints += PointsTable.RegionalWinnerPoints[sim];
                    team.TotalPoints += PointsTable.RegionalWinnerPoints[sim];
                    break;
                case 2:
                    team.RegionPoints += PointsTable.RegionalSecondPoints[sim];
                    team.TotalPoints += PointsTable.RegionalSecondPoints[sim];
                    break;
                case 3:
                    team.RegionPoints += PointsTable.RegionalThirdPoints[sim];
                    team.TotalPoints += PointsTable.RegionalThirdPoints[sim];
                    break;
                case 4:
                    team.RegionPoints += PointsTable.RegionalTop4Points[sim];
                    team.TotalPoints += PointsTable.RegionalTop4Points[sim];
                    break;
            }
        }
    }
}