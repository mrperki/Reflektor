using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflektor.Models
{
    public enum TriggerInterval
    {
        Minutes,
        Hours,
        Days
    }

    public class Trigger
    {
        public string Name { get; set; }
        public DateTime? StartDateTime { get; set; }
        public int RunEvery { get; set; }
        public TriggerInterval RunEveryInterval { get; set; } = TriggerInterval.Hours;
        public bool Enabled { get; set; } = true;
        public bool RunImmediatelyIfMissed { get; set; } = true;
        public string MirrorFolderPrefix { get; set; }
        public bool DisableOnFail { get; set; } = true;
    }
}
