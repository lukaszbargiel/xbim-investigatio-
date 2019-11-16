using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Entities
{
    public class Project : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string IfcId { get; set; }
        public string IfcName { get; set; }
        public string IfcFileName { get; set; }
        public string EntityLabel { get; set; }

        public List<Site> Sites { get; set; }
    }
}
