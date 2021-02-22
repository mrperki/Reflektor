using System;

namespace Reflektor.Models
{
    public interface IJobInstance
    {
        Job Job { get; }
        Guid? InstanceId { get; }
    }
}
