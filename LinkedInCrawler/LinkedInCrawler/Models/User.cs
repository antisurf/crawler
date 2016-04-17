using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkedInCrawler.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string CurrentPosition { get; set; }
        public string Summary { get; set; }
        public List<string> Skills { get; set; }
        public int TopSkillsCounter { get; set; }
    }
}