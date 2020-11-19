using LocatorServer.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatorServer.Controllers
{
    [ApiController]
    public class SecureFileController : ControllerBase
    {
        /*
         * 
         * 
         * There is a pretty annoying "bug" in aspnet core 5 where using both mvc and webapi will
         * cause the webapi functions to redirect to the login page instead of just returning
         * 401.
         * 
         * Ideally, I would just throw an [Authorize(Roles="authorized")] on the class, but
         * instead I have to manually return Unauthorized for each method.  This is fine, but
         * should not be ignored in the future!!!!!
         * 
         * https://github.com/dotnet/aspnetcore/issues/9039
         * 
         * 
         */
        [HttpGet]
        [Route("api/securefile/{filename}")]
        public IActionResult Get(string filename)
        {
            if (!User.IsInRole("authorized"))
            {
                return Unauthorized();
            }
            var path = $"/app/Data/SecureFiles/{filename}";

            return PhysicalFile(path, "image/png");
        }
    }
}
