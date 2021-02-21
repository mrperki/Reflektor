using Reflektor.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reflektor
{
    public static class JobAndTriggerExtensions
    {
        public static List<TriggerProgress> GetAllInitialProgress(this IEnumerable<Job> jobs)
            => jobs.SelectMany(j => j.GetAllInitialProgress()).ToList();

        public static List<TriggerProgress> GetAllInitialProgress(this Job job)
            => job.Triggers.Select(t => job.GetInitialProgress(t)).ToList();

        public static TriggerProgress GetInitialProgress(this Job job, Trigger trigger)
        {
            var progress = new TriggerProgress
            {
                Trigger = trigger,
                Job = job,
                Status = job.GetInitialTriggerStatus(trigger),
            };
            progress.NextRun = progress.GetInitialNextRun();

            return progress;
        }

        public static TriggerStatus GetInitialTriggerStatus(this Job job, Trigger trigger)
        {
            if (!job.Enabled) return TriggerStatus.Disabled;
            if (!trigger.Enabled) return TriggerStatus.Disabled;
            if (trigger.StartDateTime.HasValue) return TriggerStatus.WaitingToStart;
            return TriggerStatus.ReadyToStart;
        }

        public static DateTime? GetInitialNextRun(this TriggerProgress triggerProgress)
            => triggerProgress.Status switch
            {
                TriggerStatus.Disabled => null,
                TriggerStatus.WaitingToStart => triggerProgress.Trigger.StartDateTime,
                _ => DateTime.Now,
            };

        public static bool CheckReadiness(this TriggerProgress triggerProgress)
            => triggerProgress.Status switch
            {
                TriggerStatus.ReadyToStart => true,
                TriggerStatus.WaitingToStart
                or TriggerStatus.WaitingForNext
                    => triggerProgress.CheckNextRun(),
                _ => false,
            };

        private static bool CheckNextRun(this TriggerProgress triggerProgress)
        {
            if (triggerProgress.Status == TriggerStatus.WaitingToStart)
                return triggerProgress.Trigger.StartDateTime <= DateTime.Now;

            if (triggerProgress.Trigger.RunImmediatelyIfMissed)
                return triggerProgress.NextRun <= DateTime.Now;

            var timeDiff = DateTime.Now - triggerProgress.NextRun;
            if (!timeDiff.HasValue || timeDiff.Value.Ticks < 0) return false;

            // If we missed the time by more than a minute, reschedule
            if (timeDiff.Value.TotalMinutes <= 1) return true;
            triggerProgress.UpdateNextRun();
            return false;
        }

        public static void UpdateNextRun(this TriggerProgress triggerProgress)
        {
            switch (triggerProgress.Trigger.RunEveryInterval)
            {
                case TriggerInterval.Days:
                    triggerProgress.NextRun.Value.AddDays(triggerProgress.Trigger.RunEvery);
                    break;
                case TriggerInterval.Hours:
                    triggerProgress.NextRun.Value.AddHours(triggerProgress.Trigger.RunEvery);
                    break;
                case TriggerInterval.Minutes:
                    triggerProgress.NextRun.Value.AddMinutes(triggerProgress.Trigger.RunEvery);
                    break;
            }
        }
    }
}
