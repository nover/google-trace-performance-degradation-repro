using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleTracePerformanceRepro.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GoogleTracePerformanceRepro.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MaterialsController : ControllerBase
    {
        private readonly ILogger<MaterialsController> _logger;
        private readonly IGetStuffService _getStuffService;

        public MaterialsController(ILogger<MaterialsController> logger, IGetStuffService getStuffService)
        {
            _logger = logger;
            _getStuffService = getStuffService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            _logger.LogDebug("Getting file by id {Id}", id);
            
            // NOtes: force some Async IO that cannot be cached by the OS (like getting the file from a storage bucket)
            await _getStuffService.GetStuffAsync();
            
            // now read the file and return
            var bytes = await System.IO.File.ReadAllBytesAsync("./large_image.jpg");

            return File(bytes, "image/jpeg");
        }
    }
}