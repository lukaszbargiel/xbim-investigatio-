using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public class ArbitraryClosedGeometry : ShapeGeometry
    {
        public ArbitraryClosedGeometry()
        {
            ShapeVertices = new List<ArbitraryClosedShapeVertices>();
            ShapeGeometryType = GeometryType.ArbitraryClosedShape;
        }
        public List<ArbitraryClosedShapeVertices> ShapeVertices { get; set; }
    }
}
