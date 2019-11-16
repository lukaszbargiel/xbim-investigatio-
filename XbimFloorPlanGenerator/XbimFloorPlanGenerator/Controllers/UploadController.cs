using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;
using XbimFloorPlanGenerator.Services.Interfaces;

namespace XbimFloorPlanGenerator.Controllers
{
    [ApiController]
    [Route("upload")]
    public class UploadController : ControllerBase {
        private readonly IIfcFileRepository _fileRepository;

        public UploadController(IIfcFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "IFC");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var ifcFile = new IfcFile()
                    {
                        FileName = fileName,
                        FullPath = fullPath,
                        FileSize = file.Length,
                        WasProcessed = false
                    };
                    
                    _fileRepository.Create(ifcFile);

                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
