﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Entities
{
    public class Site : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string IfcId { get; set; }
        public string IfcName { get; set; }
        public string EntityLabel { get; set; }
        public string Description { get; set; }
        //public decimal FootprintArea { get; set; }

        public List<Building> Buildings { get; set; }

        public Project Project { get; set; }
    }
}
