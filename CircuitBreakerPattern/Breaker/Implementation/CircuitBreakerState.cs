namespace CircuitBreakerPattern.Breaker.Implementation
{
    public enum CircuitBreakerState
    {
        Open = 0,
        HalfOpen = 10,
        Closed = 20
    }
}