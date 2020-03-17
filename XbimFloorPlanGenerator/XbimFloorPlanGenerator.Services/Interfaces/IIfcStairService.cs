using Xbim.Ifc2x3.SharedBldgElements;
using XbimFloorPlanGenerator.Data.Entities;

namespace XbimFloorPlanGenerator.Services.Interfaces
{
    public interface IIfcStairService
    {
        Stair CreateStair(IfcStair ifcStair, int floorId);
        Stair CreateStair(IfcStairFlight ifcStair, int floorId);
    }
}