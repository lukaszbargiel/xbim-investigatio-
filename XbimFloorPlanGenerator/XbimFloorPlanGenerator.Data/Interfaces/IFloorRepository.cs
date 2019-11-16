using System;
using System.Collections.Generic;
using System.Text;
using XbimFloorPlanGenerator.Data.Entities;

namespace XbimFloorPlanGenerator.Data.Interfaces
{
    public interface IFloorRepository : IRepository<Floor>
    {
        Floor GetWithIncludesById(int id);
    }
}
