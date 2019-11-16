using System;
using System.Collections.Generic;
using System.Text;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Repositories
{
    public class WallRepository : Repository<Wall>, IWallRepository
    {
        public WallRepository(DataContext dbContext)
            : base(dbContext)
        {

        }
    }
}
