namespace CircuitBreakerPattern.Breaker
{
    internal class CircuitBreakerStateStoreFactory
    {
        public static ICircuitBreakerStateStore GetCircuitBreakerStateStore()
        {
            return new CircuitBreakerStateStore();
        }
    }
}