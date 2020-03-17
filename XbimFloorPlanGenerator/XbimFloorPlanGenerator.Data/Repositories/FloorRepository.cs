using System;
using System.Collections.Generic;
using System.Text;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace XbimFloorPlanGenerator.Data.Repositories
{
    public class FloorRepository : Repository<Floor>, IFloorRepository
    {
        public FloorRepository(DataContext dbContext)
            : base(dbContext)
        {

        }

        public Floor GetWithIncludesById(int id)
        {
            return _dbContext.Set<Floor>()
                .Include(p => p.Spaces)
                .Include(p => p.Stairs)
                .Include(p => p.Walls)
                    .ThenInclude(y => y.Openings)
                        .FirstOrDefault(e => e.Id == id);
        }
    }
}
