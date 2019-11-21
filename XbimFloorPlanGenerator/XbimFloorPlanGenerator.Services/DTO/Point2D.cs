using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public class Point2D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point2D()
        {

        }
        public Point2D(double coordinateX, double coordinateY)
        {
            X = coordinateX;
            Y = coordinateY;
        }
        public bool Equals(Point2D other)
        {
            if(X == other.X && Y == other.Y)
            {
                return true;
            }
            return false;
        }
    }
}
