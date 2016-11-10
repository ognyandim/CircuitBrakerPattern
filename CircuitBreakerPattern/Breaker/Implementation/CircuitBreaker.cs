using System;
using System.Threading;

namespace CircuitBreakerPattern.Breaker.Implementation
{
    public class CircuitBreaker : ICircuitBreaker
    {
        private readonly ICircuitBreakerStateStore _circuitBreakerStateStore;
        private readonly IEventLogger _eventLogger;
        private readonly object halfOpenSyncObject = new object();

        public TimeSpan OpenToHalfOpenWaitTime { get; set; }

        public int SuccessfulOperationsThreshold { get; set; }

        public bool IsClosed => _circuitBreakerStateStore.IsClosed;

        public bool IsOpen => !IsClosed;

        public CircuitBreaker(ICircuitBreakerStateStore circuitBreakerStateStore, IEventLogger eventLogger)
        {
            _circuitBreakerStateStore = circuitBreakerStateStore;
            _eventLogger = eventLogger;
            OpenToHalfOpenWaitTime = TimeSpan.FromMilliseconds(500);
            SuccessfulOperationsThreshold = 5;
        }

        public void ExecuteAction(Action action)
        {
            if (IsOpen)
            {
                if (_circuitBreakerStateStore.LastStateChanged + OpenToHalfOpenWaitTime < DateTime.UtcNow)
                {
                    bool lockTaken = false;
                    try
                    {
                        Monitor.TryEnter(halfOpenSyncObject, ref lockTaken);

                        if (lockTaken)
                        {
                            _circuitBreakerStateStore.HalfOpen();

                            action();

                            _circuitBreakerStateStore.SuccessfulOperations++;

                            if (_circuitBreakerStateStore.SuccessfulOperations <= SuccessfulOperationsThreshold)
                            {
                                return;
                            }

                            this._circuitBreakerStateStore.Reset();
                            _circuitBreakerStateStore.SuccessfulOperations = 0;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        TrackException(ex);
                        throw;
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            Monitor.Exit(halfOpenSyncObject);
                        }
                    }

                }

                throw new CircuitBreakerOpenException(_circuitBreakerStateStore.LastException);
            }

            try
            {
                action();
            }
            catch (Exception ex)
            {
                this.TrackException(ex);
                throw;
            }
        }

        private void TrackException(Exception ex)
        {
            this._circuitBreakerStateStore.Trip(ex);
            _eventLogger.Exception(ex.Message + ex.StackTrace);
        }
    }
}