using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.SharedBldgElements;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;
using XbimFloorPlanGenerator.Services.DTO;
using XbimFloorPlanGenerator.Services.Interfaces;

namespace XbimFloorPlanGenerator.Services
{
    public class IfcSpaceService : IIfcSpaceService
    {
        private readonly IMapper _mapper;
        private readonly IIfcGeometryService _ifcGeometryService;
        private readonly IIfcSpaceBoundriesService _spaceBoundriesService;
        public IfcSpaceService(
            IMapper mapper,
            IIfcGeometryService ifcGeometryService,
            IIfcSpaceBoundriesService spaceBoundriesService)
        {
            _mapper = mapper;
            _ifcGeometryService = ifcGeometryService;
            _spaceBoundriesService = spaceBoundriesService;
        }

        public Space CreateSpace(IfcSpace ifcSpace, int floorId)
        {
            var dbSpace = _mapper.Map<Space>(ifcSpace);
            dbSpace.FloorId = floorId;
            dbSpace.SerializedShapeGeometry = JsonConvert.SerializeObject(_ifcGeometryService.GetShapeGeometry(ifcSpace));
            _spaceBoundriesService.ExtractSpaceBoundries(ifcSpace);

            return dbSpace;
        }

        
    }
}
