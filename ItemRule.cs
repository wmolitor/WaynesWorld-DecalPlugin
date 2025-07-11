using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WaynesWorld
{
    public class ItemRule
    {
        [XmlElement("rulename")]
        public string RuleName { get; set; }

        [XmlElement("regex")]
        public string Regex { get; set; }

        [XmlElement("enabled")]
        public bool Enabled { get; set; }
    }
}
