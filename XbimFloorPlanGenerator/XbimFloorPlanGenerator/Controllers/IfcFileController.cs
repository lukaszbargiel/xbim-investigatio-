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
    [Route("ifc")]
    public class IfcFileController : ControllerBase
    {
 
        private readonly ILogger<IfcFileController> _logger;
        private readonly IIfcFileRepository _fileRepository;
        private readonly IIfcParserService _parserService;
        public IfcFileController(ILogger<IfcFileController> logger, 
            IIfcFileRepository fileRepository,
            IIfcParserService parserService)
        {
            _logger = logger;
            _fileRepository = fileRepository;
            _parserService = parserService;
        }

        [HttpPost]
        public ActionResult Post([FromBody] ProcessIfcFileDTO request)
        {
            var requestedFile = _fileRepository.GetById(request.IfcFileId);
            _parserService.Parse(requestedFile.FullPath);
            requestedFile.WasProcessed = true;
            _fileRepository.Update(requestedFile.Id, requestedFile);
            return Ok();
        }
        [HttpGet]
        public IEnumerable<IfcFile> Get()
        {
            return _fileRepository.GetAll().ToList();
        }
    }
}
