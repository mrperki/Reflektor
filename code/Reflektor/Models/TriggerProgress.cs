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

    public class TriggerProgress : IJobInstance
    {
        public Trigger Trigger { get; set; }
        public Job Job { get; set; }
        public Guid? InstanceId { get; set; }
        public TriggerStatus Status { get; set; }
        public DateTime? NextRun { get; set; }
    }
}
