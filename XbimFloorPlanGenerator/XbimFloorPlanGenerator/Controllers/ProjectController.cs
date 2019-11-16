using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;
using XbimFloorPlanGenerator.Services.DTO;
using XbimFloorPlanGenerator.Services.Interfaces;

namespace XbimFloorPlanGenerator.Controllers
{
    [ApiController]
    [Route("project")]
    public class ProjectController : ControllerBase
    {
 
        private readonly ILogger<IfcFileController> _logger;
        private readonly IProjectRepository _projectRepository;
        public ProjectController(ILogger<IfcFileController> logger,
            IProjectRepository projectRepository)
        {
            _logger = logger;
            _projectRepository = projectRepository;
        }
        [HttpGet]
        public IEnumerable<Project> Get()
        {
            return _projectRepository.GetWithIncludes();
        }
    }
}
