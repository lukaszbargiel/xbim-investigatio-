using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public class ProducShape
    {
        [JsonProperty("f")]
        public List<Face> Faces { get; set; }
        public ProducShape()
        {
            Faces = new List<Face>();
        }
    }
}
