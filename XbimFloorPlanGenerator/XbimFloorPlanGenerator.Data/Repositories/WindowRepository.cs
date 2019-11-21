using System;
using System.Collections.Generic;
using System.Text;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Repositories
{
    public class WindowRepository : Repository<Window>, IWindowRepository
    {
        public WindowRepository(DataContext dbContext)
            : base(dbContext)
        {

        }
    }
}
