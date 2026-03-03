using Fifa_Simulation.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fifa_Simulation.Helpers
    {
        public static class PointsTable
        {
            public static readonly int[] Top16Points = { 3, 4, 5 };
            public static readonly int[] Top12Points = { 4, 5, 6 };
            public static readonly int[] Top9Points = { 5, 6, 7 };
            public static readonly int[] Top8Points = { 8, 10, 12 };
            public static readonly int[] Top6Points = { 10, 13, 16 };
            public static readonly int[] Top4Points = { 12, 16, 20 };
            public static readonly int[] ThirdPoints = { 16, 20, 24 };
            public static readonly int[] SecondPoints = { 16, 24, 32 };
            public static readonly int[] WinnerPoints = { 20, 30, 40 };
        }

        public static class PointsAwarder
        {
            public static void AwardPoints(Team team, int sim)
            {
                    switch (team.Seed)
                    {
                        case 1:
                            team.Points += PointsTable.WinnerPoints[sim];
                            break;
                        case 2:
                            team.Points += PointsTable.SecondPoints[sim];
                            break;
                        case 3:
                            team.Points += PointsTable.ThirdPoints[sim];
                            break;
                        case 4:
                            team.Points += PointsTable.Top4Points[sim];
                            break;
                        case 6:
                            team.Points += PointsTable.Top6Points[sim];
                            break;
                        case 8:
                            team.Points += PointsTable.Top8Points[sim];
                            break;
                        case 9:
                            team.Points += PointsTable.Top9Points[sim];
                            break;
                        case 12:
                            team.Points += PointsTable.Top12Points[sim];
                            break;
                        case 16:
                            team.Points += PointsTable.Top16Points[sim];
                            break;
                    }
                }
            }
        }
