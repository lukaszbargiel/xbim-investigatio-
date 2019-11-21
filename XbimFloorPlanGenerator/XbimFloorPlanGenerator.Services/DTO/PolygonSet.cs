using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public class PolygonSet
    {
        public List<Polygon> Polygons { get; set; }
        public PolygonSet()
        {
            Polygons = new List<Polygon>();
        }
    }
}
