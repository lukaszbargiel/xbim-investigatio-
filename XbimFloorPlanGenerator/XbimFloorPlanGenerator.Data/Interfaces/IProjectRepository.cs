using System;
using System.Collections.Generic;
using System.Text;
using XbimFloorPlanGenerator.Data.Entities;

namespace XbimFloorPlanGenerator.Data.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        List<Project> GetWithIncludes();
    }
}
