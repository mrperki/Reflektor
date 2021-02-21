using System;

namespace Reflektor.Models
{
    public enum TriggerStatus
    {
        Disabled,
        WaitingToStart,
        ReadyToStart,
        Running,
        WaitingForNext,
        Error
    }

    public class TriggerProgress
    {
        public Trigger Trigger { get; set; }
        public Job Job { get; set; }
        public TriggerStatus Status { get; set; }
        public DateTime? NextRun { get; set; }
    }
}
