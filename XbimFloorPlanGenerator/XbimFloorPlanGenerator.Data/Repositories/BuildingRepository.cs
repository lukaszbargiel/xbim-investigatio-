using System;
using System.Collections.Generic;
using System.Text;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Repositories
{
    public class BuildingRepository : Repository<Building>, IBuildingRepository
    {
        public BuildingRepository(DataContext dbContext)
            : base(dbContext)
        {

        }
    }
}
