using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Ifc2x3.SharedBldgElements;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Services.Interfaces;

namespace XbimFloorPlanGenerator.Services
{
    public class IfcWindowService : IIfcWindowService
    {
        private readonly IMapper _mapper;
        private readonly IIfcGeometryService _ifcGeometryService;
        public IfcWindowService(
            IMapper mapper,
            IIfcGeometryService ifcGeometryService)
        {
            _mapper = mapper;
            _ifcGeometryService = ifcGeometryService;
        }

        public Window CreateWindow(IfcWindow ifcWall, int floorId)
        {
            var dbWindow = _mapper.Map<Window>(ifcWall);
            dbWindow.FloorId = floorId;

            dbWindow.SerializedShapeGeometry = JsonConvert.SerializeObject(_ifcGeometryService.GetShape2DGeometryFromMeshTriangles(ifcWall));

            return dbWindow;
        }
    }
}
