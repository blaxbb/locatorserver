using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatorServer.Services
{
    public class SecretsConfiguration
    {
        public const string Secrets = "Secrets";
        public string SharedPassword { get; set; }
        public string DbPassword { get; set; }
    }
}
