namespace CircuitBreakerPattern.Breaker.Implementation
{
    internal class CircuitBreakerStateStoreFactory
    {
        public static ICircuitBreakerStateStore GetCircuitBreakerStateStore()
        {
            return new CircuitBreakerStateStore();
        }
    }
}