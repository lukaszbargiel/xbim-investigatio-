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
    public class IfcStairService : IIfcStairService
    {
        private readonly IMapper _mapper;
        private readonly IIfcGeometryService _ifcGeometryService;
        public IfcStairService(
            IMapper mapper,
            IIfcGeometryService ifcGeometryService)
        {
            _mapper = mapper;
            _ifcGeometryService = ifcGeometryService;
        }

        public Stair CreateStair(IfcStairFlight ifcStair, int floorId)
        {
            var dbstair = _mapper.Map<Stair>(ifcStair);
            dbstair.FloorId = floorId;

            dbstair.SerializedShapeGeometry = JsonConvert.SerializeObject(_ifcGeometryService.GetShape2DGeometryFromMeshTriangles(ifcStair));

            return dbstair;
        }
        public Stair CreateStair(IfcStair ifcStair, int floorId)
        {
            var dbstair = _mapper.Map<Stair>(ifcStair);
            dbstair.FloorId = floorId;

            dbstair.SerializedShapeGeometry = JsonConvert.SerializeObject(_ifcGeometryService.GetShape2DGeometryFromMeshTriangles(ifcStair));

            return dbstair;
        }
    }
}
