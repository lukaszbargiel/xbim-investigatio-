using System;
using System.Collections.Generic;
using System.Text;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace XbimFloorPlanGenerator.Data.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository 
    {
        public ProjectRepository(DataContext dbContext)
            : base(dbContext)
        {

        }
        public List<Project> GetWithIncludes()
        {
            return _dbContext.Set<Project>()
                .Include(p => p.Sites)
                    .ThenInclude(y => y.Buildings)
                        .ThenInclude(y => y.Floors).ToList();
        }
    }
}
