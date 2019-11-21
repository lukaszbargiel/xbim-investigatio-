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

            dbwall.SerializedShapeGeometry = JsonConvert.SerializeObject(_ifcGeometryService.GetShape2DGeometryFromMeshTriangles(ifcWall));

            // extract openings
            dbwall.Openings = new List<Opening>();
            foreach(var opening in ifcWall.HasOpenings)
            {
                var dbOpening = _mapper.Map<Opening>(opening.RelatedOpeningElement);
                var openingGeometry =_ifcGeometryService.GetShape2DGeometryFromMeshTriangles(opening.RelatedOpeningElement);
                dbOpening.SerializedShapeGeometry = JsonConvert.SerializeObject(openingGeometry);
                dbwall.Openings.Add(dbOpening);
            }
            
            //var wallId = _wallRepository.Create(dbwall);
            //var wallShapesBoundries = _ifcGeometryService.GetShapeBoundry(model, (IfcProduct)wall);

            //foreach(var wallShape in wallShapes)
            //{
            //    wallShape.ProductId = wallId;
            //    wallShape.ShapeType = ProductShapeType.Wall;
            //    _productShapeRepository.Create(wallShape);
            //}

            //dbwall.SerializedShapeGeometry = JsonConvert.SerializeObject(_ifcGeometryService.GetShapeGeometry(ifcWall));


            return dbwall;
        }
    }
}
