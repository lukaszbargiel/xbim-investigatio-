using System;
using System.Collections.Generic;
using System.Text;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Repositories
{
    public class IfcFileRepository : Repository<IfcFile>, IIfcFileRepository
    {
        public IfcFileRepository(DataContext dbContext)
            : base(dbContext)
        {

        }
    }
}
