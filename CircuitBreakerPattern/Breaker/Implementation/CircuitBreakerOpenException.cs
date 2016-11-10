using System;

namespace CircuitBreakerPattern.Breaker
{
    [Serializable]
    public class CircuitBreakerOpenException : Exception
    {
        private Exception _stateStoreLastException;

        public CircuitBreakerOpenException(Exception stateStoreLastException)
        {
            _stateStoreLastException = stateStoreLastException;
        }
    }
}