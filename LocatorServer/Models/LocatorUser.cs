using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatorServer.Models
{
    public class LocatorUser : IdentityUser
    {
        [PersonalData]
        public string RealName { get; set; }
    }
}
