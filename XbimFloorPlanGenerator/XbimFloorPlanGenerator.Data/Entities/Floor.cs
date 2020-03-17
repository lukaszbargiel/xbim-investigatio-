using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Entities
{
    public class Floor : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public string IfcId { get; set; }
        public string IfcName { get; set; }
        //public double Elevation { get; set; }
        public string EntityLabel { get; set; }
        public string Description { get; set; }

        public List<Wall> Walls { get; set; }

        public List<Stair> Stairs { get; set; }
        public List<Window> Windows { get; set; }
        public List<Space> Spaces { get; set; }

        public Building Building { get; set; }
        public double Elevation { get; set; }
    }
}
