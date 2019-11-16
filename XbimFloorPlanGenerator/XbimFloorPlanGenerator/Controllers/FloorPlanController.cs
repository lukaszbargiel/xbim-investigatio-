using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XbimFloorPlanGenerator.Data.Interfaces;

namespace XbimFloorPlanGenerator.Data.Controllers
{
    [ApiController]
    [Route("floor-plan")]
    public class FloorPlanController : ControllerBase
    {
        private readonly ILogger<FloorPlanController> _logger;
        private readonly IFloorRepository _floorRepository;

        public FloorPlanController(ILogger<FloorPlanController> logger, IFloorRepository floorRepository)
        {
            _logger = logger;
            _floorRepository = floorRepository;
        }

        [HttpGet("{floorId:int}")] //int value for route
        public IActionResult GetById(int floorId)
        {
            var floor = _floorRepository.GetWithIncludesById(floorId);

            return Ok(floor);

        }
    }
}
