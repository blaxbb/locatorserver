using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatorServer.Controllers
{
    [Authorize("authorized")]
    public class SecureFileController : Controller
    {
        [HttpGet]
        [Route("securefile/{filename}")]
        public IActionResult Get(string filename)
        {
            return PhysicalFile($"/app/Data/SecureFiles/{filename}", "applictaion/text");
        }
    }
}
