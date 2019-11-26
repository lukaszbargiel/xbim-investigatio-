using System.Collections.Generic;
using Xbim.Ifc2x3.ProductExtension;
using XbimFloorPlanGenerator.Data.Entities;

namespace XbimFloorPlanGenerator.Services.Interfaces
{
    public interface IIfcSpaceBoundriesService
    {
        List<Boundry> ExtractSpaceBoundries(IfcSpace space);
    }
}