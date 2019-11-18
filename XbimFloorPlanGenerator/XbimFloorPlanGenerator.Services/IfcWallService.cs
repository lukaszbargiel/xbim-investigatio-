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
    public class IfcWallService : IIfcWallService
    {
        private readonly IMapper _mapper;
        private readonly IIfcGeometryService _ifcGeometryService;
        private readonly IIfcSpaceBoundriesService _spaceBoundriesService;
        public IfcWallService(
            IMapper mapper,
            IIfcGeometryService ifcGeometryService,
            IIfcSpaceBoundriesService spaceBoundriesService)
        {
            _mapper = mapper;
            _ifcGeometryService = ifcGeometryService;
            _spaceBoundriesService = spaceBoundriesService;
        }

        public Wall CreateWall(IfcWall ifcWall, int floorId)
        {
            var dbwall = _mapper.Map<Wall>(ifcWall);
            dbwall.FloorId = floorId;

            //var wallId = _wallRepository.Create(dbwall);
            //var wallShapesBoundries = _ifcGeometryService.GetShapeBoundry(model, (IfcProduct)wall);

            //foreach(var wallShape in wallShapes)
            //{
            //    wallShape.ProductId = wallId;
            //    wallShape.ShapeType = ProductShapeType.Wall;
            //    _productShapeRepository.Create(wallShape);
            //}

            dbwall.SerializedShapeGeometry = JsonConvert.SerializeObject(_ifcGeometryService.GetShapeGeometry(ifcWall));


            return dbwall;
        }
    }
}
