using System;
using CircuitBreakerPattern.Breaker.Implementation;

namespace CircuitBreakerPattern.Breaker
{
    public interface ICircuitBreakerStateStore
    {
        CircuitBreakerState State { get; }

        Exception LastException { get; }

        DateTime LastStateChanged { get; }

        void Trip(Exception ex);

        void Reset();

        void HalfOpen();

        bool IsClosed { get; }

        int SuccessfulOperations { get; set; }
    }
}