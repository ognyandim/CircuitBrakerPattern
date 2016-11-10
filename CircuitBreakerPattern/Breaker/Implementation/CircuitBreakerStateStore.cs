using System;

namespace CircuitBreakerPattern.Breaker.Implementation
{
    class CircuitBreakerStateStore : ICircuitBreakerStateStore
    {
        private CircuitBreakerState _state;
        public bool IsClosed => _state == CircuitBreakerState.Closed;
        public CircuitBreakerState State
        {
            get
            {
                return _state;
            }
            set
            {
                LastStateChanged = DateTime.UtcNow;
                _state = value;
            } 
        }

        public Exception LastException { get; set; }
        public DateTime LastStateChanged { get; set; }

        public int SuccessfulOperations { get; set; } = 0;

        public CircuitBreakerStateStore()
        {
            LastStateChanged = DateTime.UtcNow;
            LastException = null;
            _state = CircuitBreakerState.Closed;
        }
        public void Trip(Exception ex)
        {
            State = CircuitBreakerState.Open;
            LastException = ex;
            SuccessfulOperations = 0;
            Console.WriteLine($"Braker tripped because : {ex.Message} on {ex.StackTrace}");
        }

        public void Reset()
        {
            State = CircuitBreakerState.Closed;
            SuccessfulOperations = 0;
            LastException = null;
        }

        public void HalfOpen()
        {
            State = CircuitBreakerState.HalfOpen;
        }

   

    }
}