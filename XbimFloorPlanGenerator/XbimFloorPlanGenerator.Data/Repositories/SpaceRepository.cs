using System;
using System.Collections.Generic;
using System.Text;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Repositories
{
    public class SpaceRepository : Repository<Space>, ISpaceRepository
    {
        public SpaceRepository(DataContext dbContext)
            : base(dbContext)
        {

        }
    }
}
