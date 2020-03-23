using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public class Polygon
    {
        public Polygon()
        {
            PolygonVertices = new List<Point2D>();
        }
        [JsonProperty("pv")]
        public List<Point2D> PolygonVertices { get; set; }
    }
}
