using System;
using System.Collections.Generic;
using System.Text;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Repositories
{
    public class StairRepository : Repository<Stair>, IStairRepository
    {
        public StairRepository(DataContext dbContext)
            : base(dbContext)
        {

        }
    }
}
