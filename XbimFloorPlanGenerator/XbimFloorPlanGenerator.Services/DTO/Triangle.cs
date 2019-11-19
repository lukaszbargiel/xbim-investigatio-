using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Common.Geometry;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public class Triangle : IEquatable<Triangle>
    {
        public ArbitraryClosedShapeVertices p1;
        public ArbitraryClosedShapeVertices p2;
        public ArbitraryClosedShapeVertices p3;
        public bool IsValid
        {
            get
            {
                if (p1.Equals(p2))
                    return false;
                if (p1.Equals(p3))
                    return false;
                if (p2.Equals(p3))
                    return false;
                return true;
            }
        }

        public Triangle(ArbitraryClosedShapeVertices vP1, ArbitraryClosedShapeVertices vP2, ArbitraryClosedShapeVertices vP3)
        {
            p1 = vP1;
            p2 = vP2;
            p3 = vP3;
        }

        public bool Equals(Triangle other)
        {
            if (p1.Equals(other.p1) | p1.Equals(other.p2) | p1.Equals(other.p3))
                if (p2.Equals(other.p1) | p2.Equals(other.p2) | p2.Equals(other.p3))
                    if (p3.Equals(other.p1) | p3.Equals(other.p2) | p3.Equals(other.p3))
                        return true;
            return false;
        }
    }
}
