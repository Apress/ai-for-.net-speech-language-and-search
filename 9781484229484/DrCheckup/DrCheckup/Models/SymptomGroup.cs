using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrCheckup.Models
{
    public class SymptomGroup
    {
        public string BodyPart { get; set; }
        public string[] Symptoms { get; set; }
    }
}