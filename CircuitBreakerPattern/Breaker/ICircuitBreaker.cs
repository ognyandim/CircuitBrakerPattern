using System;

namespace CircuitBreakerPattern.Breaker
{
    public interface ICircuitBreaker
    {
        bool IsClosed { get; }

        bool IsOpen { get; }

        void ExecuteAction(Action action);

        TimeSpan OpenToHalfOpenWaitTime { get; set; }

        int SuccessfulOperationsThreshold { get; set; }
    }
}