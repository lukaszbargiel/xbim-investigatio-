using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Entities
{
    public class Building : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SiteId { get; set; }
        public string IfcId { get; set; }
        public string IfcName { get; set; }
        public double ElevationOfTerrain { get; set; }
        public string EntityLabel { get; set; }
        public string Description { get; set; }

        public List<Floor> Floors { get; set; }
        public Site Site { get; set; }
    }
}
