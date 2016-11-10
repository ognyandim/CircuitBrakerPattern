namespace CircuitBreakerPattern.Breaker
{
    public enum CircuitBreakerState
    {
        Open = 0,
        HalfOpen = 10,
        Closed = 20
    }
}