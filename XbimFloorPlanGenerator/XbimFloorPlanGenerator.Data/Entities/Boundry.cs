using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Data.Entities
{
    public class Boundry
    {
        public bool IsPhysical { get; set; }
        public bool IsExternal { get; set; }
        public string BoundryType { get; set; }
        public double BounderyArea { get; set; }
        public string BoundryName { get; set; }
    }
}
