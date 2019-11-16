using Xbim.Ifc2x3.ProductExtension;
using XbimFloorPlanGenerator.Data.Entities;

namespace XbimFloorPlanGenerator.Services.Interfaces
{
    public interface IIfcSpaceService
    {
        Space CreateSpace(IfcSpace ifcSpace, int floorId);
    }
}