using Xbim.Ifc2x3.SharedBldgElements;
using XbimFloorPlanGenerator.Data.Entities;

namespace XbimFloorPlanGenerator.Services.Interfaces
{
    public interface IIfcWallService
    {
        Wall CreateWall(IfcWall ifcWall, int floorId);
    }
}