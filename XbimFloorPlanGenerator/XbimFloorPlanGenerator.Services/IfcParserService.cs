using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xbim.Common.Geometry;
using Xbim.Ifc;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.ModelGeometry.Scene;
using XbimFloorPlanGenerator.Data;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;
using XbimFloorPlanGenerator.Services.Interfaces;

namespace XbimFloorPlanGenerator.Services
{
    public class IfcParserService : IIfcParserService
    {
        private IMapper _mapper;
        private readonly IIfcSpaceService _ifcSpaceService;
        private readonly ISpaceRepository _spaceRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IWallRepository _wallRepository;
        private readonly IProductShapeRepository _productShapeRepository;
        private IfcStore model;
        public IfcParserService(
            IIfcSpaceService ifcSpaceService,
            ISpaceRepository spaceRepository,
            IProjectRepository projectRepository,
            ISiteRepository siteRepository,
            IBuildingRepository buildingRepository,
            IFloorRepository floorRepository,
            IWallRepository wallRepository,
            IProductShapeRepository productShapeRepository,
            IMapper mapper)
        {
            _spaceRepository = spaceRepository;
            _ifcSpaceService = ifcSpaceService;
            _projectRepository = projectRepository;
            _siteRepository = siteRepository;
            _buildingRepository = buildingRepository;
            _floorRepository = floorRepository;
            _wallRepository = wallRepository;
            _productShapeRepository = productShapeRepository;
            _mapper = mapper;
        }

        public void Parse(string ifcFileName)
        {
            using (model = IfcStore.Open(ifcFileName))
            {
                ExtractProjectStructure(model);
            }
        }

        private void ExtractBuilding(IfcSite site, int siteId)
        {
            foreach(IfcBuilding building in site.Buildings)
            {
                var dbbuilding = _mapper.Map<Building>(building);
                dbbuilding.SiteId = siteId;

                var buildingId = _buildingRepository.Create(dbbuilding);
                ExtractFloor(building, buildingId);
            }
        }
        private void ExtractFloor(IfcBuilding building, int buildingId)
        {
            foreach (IfcBuildingStorey storey in building.BuildingStoreys)
            {

                var dbfloor = _mapper.Map<Floor>(storey);
                dbfloor.BuildingId = buildingId;
                
                var floorId = _floorRepository.Create(dbfloor);
                ExtractWalls(storey, floorId);
                ExtractSpaces(storey, floorId);
            }
        }

        private void ExtractWalls(IfcBuildingStorey storey, int floorId)
        {
            foreach (var wall in storey.ContainsElements.SelectMany(rel => rel.RelatedElements).OfType<IfcWall>()
                .Where(p => !p.GetType().IsSubclassOf(typeof(IfcFeatureElementSubtraction))) // Ignore Openings  
                .OrderBy(o => o.GetType().Name))
            {

                var dbwall = _mapper.Map<Wall>(wall);
                dbwall.FloorId = floorId;

                var wallId = _wallRepository.Create(dbwall);
                var wallShapes = ExtractGeometryData(model, (IfcProduct) wall);
                foreach(var wallShape in wallShapes)
                {
                    wallShape.ProductId = wallId;
                    wallShape.ShapeType = ProductShapeType.Wall;
                    _productShapeRepository.Create(wallShape);
                }
            }
        }

        private void ExtractSpaces(IfcBuildingStorey storey, int floorId)
        {
            foreach (IfcSpace space in ((IfcBuildingStorey)storey).Spaces
                .Where(p => !p.GetType().IsSubclassOf(typeof(IfcFeatureElementSubtraction))) // Ignore Openings  
                .OrderBy(o => o.GetType().Name))
            {
                var dbSpace = _ifcSpaceService.CreateSpace(space, floorId);
                _spaceRepository.Create(dbSpace);
            }
        }
        private void ExtractProjectStructure(IfcStore model)
        {

            // extract ifc file project
            var ifcProject = model.Instances.OfType<IIfcProject>().FirstOrDefault();
            var dbproject = _mapper.Map<Project>((IfcProject)ifcProject);
            //dbproject.IfcFileName = 

            var projectId = _projectRepository.Create(dbproject);
            
            // extract ifc sites
            var ifcSites = model.Instances.OfType<IIfcSite>();
            foreach(var ifcSite in ifcSites)
            {
                var dbsite = _mapper.Map<Site>((IfcSite)ifcSite);
                dbsite.ProjectId = projectId;
                var siteId = _siteRepository.Create(dbsite);
                ExtractBuilding((IfcSite)ifcSite, siteId);                
            }           
        }
        private List<ProductShape> ExtractGeometryData(IfcStore model, IfcProduct product)
        {
            var context = new Xbim3DModelContext(model);
            var shapes = new List<ProductShape>();
            context.CreateContext();
 
            // https://github.com/xBimTeam/XbimEssentials/issues/121
            var productShape =
                context.ShapeInstancesOf(product)
                    .Where(p => p.RepresentationType != XbimGeometryRepresentationType.OpeningsAndAdditionsExcluded)
                .Distinct();


            if (productShape.Any())
            {
                foreach (var shapeInstance in productShape)
                {

                    var shapeGeometry = context.ShapeGeometry(shapeInstance.ShapeGeometryLabel);
                    if (shapeGeometry == null) continue;

                    var transformedBoundry = shapeGeometry.BoundingBox.Transform(shapeInstance.Transformation);
                    shapes.Add(new ProductShape()
                    {
                        BoundingBoxX = transformedBoundry.X,
                        BoundingBoxY = transformedBoundry.Y,
                        BoundingBoxSizeX = transformedBoundry.SizeX,
                        BoundingBoxSizeY = transformedBoundry.SizeY
                    });
                }
            }

            return shapes;
        }
    
}
}
