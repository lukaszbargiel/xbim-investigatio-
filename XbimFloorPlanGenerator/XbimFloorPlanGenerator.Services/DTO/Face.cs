using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public class Face
    {
        [JsonProperty("p")]
        public List<Polygon> Polygons { get; set; }
        public Face()
        {
            Polygons = new List<Polygon>();
        }
    }
}
