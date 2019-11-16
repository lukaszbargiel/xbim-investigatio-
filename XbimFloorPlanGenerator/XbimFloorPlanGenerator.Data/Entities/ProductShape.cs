using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XbimFloorPlanGenerator.Data.Interfaces;


namespace XbimFloorPlanGenerator.Data.Entities
{
    public enum ProductShapeType
    {
        Wall = 0,
        Door = 1,
        Window = 2,
        Space = 3,
    }
    public class ProductShape : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public ProductShapeType ShapeType { get; set; }

        [Column(TypeName = "float")]
        public double BoundingBoxX { get; set; }
        [Column(TypeName = "float")]
        public double BoundingBoxY { get; set; }
        [Column(TypeName = "float")]
        public double BoundingBoxSizeX { get; set; }
        [Column(TypeName = "float")]
        public double BoundingBoxSizeY { get; set; }

        public Wall Wall { get; set; }
    }
}
