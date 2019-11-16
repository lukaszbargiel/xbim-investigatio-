using System;
using System.Collections.Generic;
using System.Text;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Repositories
{
    public class ProductShapeRepository : Repository<ProductShape>, IProductShapeRepository
    {
        public ProductShapeRepository(DataContext dbContext)
            : base(dbContext)
        {

        }
    }
}
