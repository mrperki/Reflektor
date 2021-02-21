using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflektor.Models
{
    public class Job
    {
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public string MirrorFolderPrefix { get; set; } = "Reflektor";
        public int RetainFolderCount { get; set; } = 1;
        public int? FolderLifetimeHours { get; set; } = null;
        public bool Enabled { get; set; } = true;
        public List<Trigger> Triggers { get; set; } = new List<Trigger>();
    }
}
