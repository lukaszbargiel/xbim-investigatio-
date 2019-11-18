using Xbim.Ifc2x3.ProductExtension;

namespace XbimFloorPlanGenerator.Services.Interfaces
{
    public interface IIfcSpaceBoundriesService
    {
        void ExtractSpaceBoundries(IfcSpace space);
    }
}