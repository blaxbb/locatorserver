using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LocatorServer.Models
{
    public class LocationEntry
    {
        public long ID { get; set; }
        [DisplayFormat(NullDisplayText = "None")]
        public virtual LocatorUser Author { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }
        public string Location { get; set; }
        public string SKU { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
