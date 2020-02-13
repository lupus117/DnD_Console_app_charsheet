using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class Components
    {
        public bool material { get; set; }
        public string raw { get; set; }
        public bool somatic { get; set; }
        public bool verbal { get; set; }
        public List<string> materials_needed { get; set; }
    }

    public class Spell
    {
        public string casting_time { get; set; }
        public List<string> classes { get; set; }
        public Components components { get; set; }
        public string description { get; set; }
        public string duration { get; set; }
        public string level { get; set; }
        public string name { get; set; }
        public string range { get; set; }
        public bool ritual { get; set; }
        public string school { get; set; }
        public List<string> tags { get; set; }
        public string type { get; set; }
        public string higher_levels { get; set; }
    }
}
