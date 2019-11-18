using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public enum GeometryType
    {
        ArbitraryClosedShape = 0,
        RectangleProfile = 1
    }
    public abstract class ShapeGeometry
    {
        public GeometryType ShapeGeometryType { get; set; }
    }
}
