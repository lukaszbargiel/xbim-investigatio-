using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public class ArbitraryClosedShapeVertices
    {
        public double X { get; set; }
        public double Y { get; set; }
        public ArbitraryClosedShapeVertices()
        {

        }
        public ArbitraryClosedShapeVertices(double coordinateX, double coordinateY)
        {
            X = coordinateX;
            Y = coordinateY;
        }
        public bool Equals(ArbitraryClosedShapeVertices other)
        {
            if(X == other.X && Y == other.Y)
            {
                return true;
            }
            return false;
        }
    }
}
