using System;
using CircuitBreakerPattern.Breaker;

namespace CircuitBreakerPattern.Switch.Implementation
{
    public class CircuitSwitch : ICircuitSwitch
    {
        private readonly ICircuitBreaker _circuitBreaker;
        private readonly IEventLogger _eventLogger;
        private Action _firstAction;
        private Action _secondAction;

        public CircuitSwitch(ICircuitBreaker circuitBreaker, IEventLogger eventLogger)
        {
            _circuitBreaker = circuitBreaker;
            _eventLogger = eventLogger;
        }
        public ICircuitSwitch Attempt(Action action)
        {
            _firstAction = action;
            return this;
        }

        public ICircuitSwitch Otherwise(Action action)
        {
            _secondAction = action;
            return this;
        }

        public void Execute()
        {
            if (_firstAction == null )
            {
                throw new InvalidOperationException("_firstAction is not set");
            }

            try
            {
                _circuitBreaker.ExecuteAction(() =>
                {
                    _firstAction.Invoke();
                });
                return;
            }
            catch (Exception ex)
            {
                if (_secondAction == null)
                {
                    return;
                }

                _secondAction.Invoke();
                _eventLogger.Exception(ex.Message + ex.InnerException?.Message + ex.StackTrace + ex.InnerException?.StackTrace);
            }
        }
    }
}
