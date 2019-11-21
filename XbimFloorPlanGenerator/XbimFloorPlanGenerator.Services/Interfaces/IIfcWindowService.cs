using Xbim.Ifc2x3.SharedBldgElements;
using XbimFloorPlanGenerator.Data.Entities;

namespace XbimFloorPlanGenerator.Services.Interfaces
{
    public interface IIfcWindowService
    {
        Window CreateWindow(IfcWindow ifcWall, int floorId);
    }
}