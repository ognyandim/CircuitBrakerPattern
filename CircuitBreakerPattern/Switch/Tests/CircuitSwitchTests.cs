using System;
using System.Timers;
using CircuitBreakerPattern.Breaker;
using Xunit;

namespace CircuitBreakerPattern.Switch.Tests
{
    public class CircuitSwitchTests
    {
        private CircuitSwitch sut;
        private CircuitBreaker circuitBreaker;
        private Action failingAction;
        private Action secondryAction;

        public CircuitSwitchTests()
        {
            ICircuitBreakerStateStore circuitBreakerStateStore = CircuitBreakerStateStoreFactory.GetCircuitBreakerStateStore();
            IEventLogger eventLogger = new EventLogger();
            circuitBreaker = new CircuitBreaker(circuitBreakerStateStore, eventLogger);

            failingAction = () => { throw new Exception(); };
            secondryAction = () => { Console.WriteLine("Nothing to see here"); };

            sut = new CircuitSwitch(circuitBreaker, eventLogger);
        }

        [Fact]
        public void Switch_calls_secondary_action_silently_when_default_action_fails()
        {
            // arrange
            sut.Attempt(failingAction).Otherwise(secondryAction);

            // act
            sut.Execute();

            // assert
            Assert.True(true); // happend to come this far.
        }

        [Fact]
        public void Switch_trips_on_action_failure_and_remains_tripped()
        {
            // arrange
            var result = string.Empty;
            Action secondryAction = () => { result = "Nothing to see here" + DateTime.UtcNow.Ticks; };
            sut.Attempt(failingAction).Otherwise(secondryAction);

            // act & assert
            Assert.True(circuitBreaker.IsClosed);

            sut.Execute();

            Assert.True(circuitBreaker.IsOpen);
            Assert.NotEmpty(result);
            var lastResult = result;

            sut.Execute();

            Assert.True(circuitBreaker.IsOpen);
            Assert.NotEmpty(result);
            Assert.NotEqual(lastResult, result);

            Assert.True(true); // happend to come this far.
        }


        [Fact]
        public void Switch_resets_after_defined_openinterval()
        {
            // arrange
            var result = string.Empty;
            Action secondryAction = () => { result = "Nothing to see here" + DateTime.UtcNow.Ticks; };
            sut.Attempt(failingAction).Otherwise(secondryAction);
            circuitBreaker.OpenToHalfOpenWaitTime = TimeSpan.FromMilliseconds(200);

            // act
            sut.Execute();

            Assert.NotEmpty(result);

            Timer t = new Timer();
            t.Interval = circuitBreaker.OpenToHalfOpenWaitTime.Milliseconds * 2;
            t.Elapsed += (sender, args) =>
            {
                sut.Attempt(() => result = "Success");
                Assert.Equal("Success", result);
            };
            t.Start();
        }
    }
}