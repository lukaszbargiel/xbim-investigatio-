using AutoMapper;
using Newtonsoft.Json;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.SharedBldgElements;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;
using XbimFloorPlanGenerator.Services.Interfaces;

namespace XbimFloorPlanGenerator.Services
{
    public class IfcParserService : IIfcParserService
    {
        private IMapper _mapper;
        private readonly IIfcSpaceService _ifcSpaceService;
        private readonly IIfcGeometryService _ifcGeometryService;
        private readonly ISpaceRepository _spaceRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IWallRepository _wallRepository;
        private readonly IIfcWallService _ifcWallService;
        private IfcStore model;
        public IfcParserService(
            IIfcSpaceService ifcSpaceService,
            ISpaceRepository spaceRepository,
            IProjectRepository projectRepository,
            ISiteRepository siteRepository,
            IBuildingRepository buildingRepository,
            IFloorRepository floorRepository,
            IWallRepository wallRepository,
            IIfcWallService ifcWallService,
            IIfcGeometryService ifcGeometryService,
            IMapper mapper)
        {
            _ifcGeometryService = ifcGeometryService;
            _spaceRepository = spaceRepository;
            _ifcSpaceService = ifcSpaceService;
            _projectRepository = projectRepository;
            _siteRepository = siteRepository;
            _buildingRepository = buildingRepository;
            _floorRepository = floorRepository;
            _wallRepository = wallRepository;
            _ifcWallService = ifcWallService;
            _mapper = mapper;
        }

        public void Parse(string ifcFileName)
        {
            using (model = IfcStore.Open(ifcFileName))
            {
                _ifcGeometryService.InitializeService(model);

                ExtractProjectStructure(model);
            }
        }

        private void ExtractBuilding(IfcSite site, int siteId)
        {
            foreach (IfcBuilding building in site.Buildings)
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
                var dbWall = _ifcWallService.CreateWall(wall, floorId);
                _wallRepository.Create(dbWall);
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
            foreach (var ifcSite in ifcSites)
            {
                var dbsite = _mapper.Map<Site>((IfcSite)ifcSite);
                dbsite.ProjectId = projectId;
                var siteId = _siteRepository.Create(dbsite);
                ExtractBuilding((IfcSite)ifcSite, siteId);
            }
        }


    }
}
