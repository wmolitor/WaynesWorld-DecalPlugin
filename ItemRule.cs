using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace WaynesWorld
{
    public class Rule
    {
        [XmlElement("rulename")]
        public string rulename { get; set; }

        [XmlElement("ruletype")]
        public string ruletype { get; set; }

        [XmlElement("regex")]
        public string regex { get; set; }

        [XmlElement("value")]
        public bool value { get; set; }

        [XmlElement("material")]
        public bool material { get; set; }

        [XmlElement("name")]
        public bool name { get; set; }

        [XmlElement("enabled")]
        public bool enabled { get; set; }

        [NonSerialized]
        public Regex CompiledRegex; // Add this

        public void Compile()
        {
            if (!string.IsNullOrWhiteSpace(regex) && enabled)
            {
                try 
                { 
                    CompiledRegex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }
                catch (Exception ex)
                {
                    ErrorLogging.log($"Error compiling regex '{regex}': {ex.Message}", 1);
                    CompiledRegex = null; // Set to null if compilation fails
                }
                CompiledRegex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }
    }
}
