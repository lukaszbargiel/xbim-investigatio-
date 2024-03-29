﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Entities
{
    public class Window : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FloorId { get; set; }
        public string IfcId { get; set; }
        public string IfcName { get; set; }
        public double OverallHeight { get; set; }
        public double OverallWidth { get; set; }
        public string EntityLabel { get; set; }
        public string Description { get; set; }

        public Floor Floor { get; set; }
        public string SerializedShapeGeometry { get; set; }
        public bool IsExternal { get; set; }
    }
}
