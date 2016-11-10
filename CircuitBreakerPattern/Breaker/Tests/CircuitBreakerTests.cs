using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Timer = System.Timers.Timer;

namespace CircuitBreakerPattern.Breaker.Tests
{

    public class CircuitBreakerTests
    {
        private ICircuitBreaker sut;

        public CircuitBreakerTests()
        {
            var eventLoggerMock = new Mock<IEventLogger>().Object;
            sut = (ICircuitBreaker)new CircuitBreaker(CircuitBreakerStateStoreFactory.GetCircuitBreakerStateStore(), eventLoggerMock);
        }

        [Fact]
        public void Breaker_trips_on_action_exception_and_remains_tripped()
        {
            // arrange 
            Exception expected = null;
            Exception unexpected = null;
            Exception circuitBreakerOpenException = null;

            // act
            try
            {
                sut.ExecuteAction(() => { throw new ArgumentNullException(); });
            }
            catch (Exception ex)
            {
                expected = ex;
            }

            var expectedEmptyString = string.Empty;
            try
            {
                sut.ExecuteAction(() => { expectedEmptyString = "I should not get executed."; });
            }
            catch (CircuitBreakerOpenException ex)
            {
                circuitBreakerOpenException = ex;
            }
            catch (Exception ex)
            {
                unexpected = ex;
            }

            // assert
            Assert.IsAssignableFrom(typeof(ArgumentNullException), expected);
            Assert.IsAssignableFrom(typeof(CircuitBreakerOpenException), circuitBreakerOpenException);
            Assert.Null(unexpected);
            Assert.Equal(expectedEmptyString, string.Empty);
        }

        [Fact]
        public void Breaker_resets_after_defined_openinterval()
        {
            // arrange 
            Exception expected = null;
            Exception unexpected = null;
            Exception circuitBreakerOpenException = null;
            var expectedEmptyString = string.Empty;
            var expectedNotEmptyString = string.Empty;

            sut.OpenToHalfOpenWaitTime = TimeSpan.FromMilliseconds(500);


            // act 
            try
            {
                sut.ExecuteAction(() => { throw new ArgumentNullException(); });
            }
            catch (Exception ex)
            {
                expected = ex;
            }


            try
            {
                sut.ExecuteAction(() => { expectedEmptyString = "I should not get executed."; });
            }
            catch (CircuitBreakerOpenException ex)
            {
                circuitBreakerOpenException = ex;
            }
            catch (Exception ex)
            {
                unexpected = ex;
            }

            var t = new Timer();
            t.Interval = sut.OpenToHalfOpenWaitTime.Milliseconds;
           
            t.Elapsed += (sender, args) =>
            {
                // assert
                Assert.True(sut.IsClosed);
                sut.ExecuteAction(() => { expectedNotEmptyString = "I should get executed."; });
                Assert.True(sut.IsClosed);
                
                Assert.IsAssignableFrom(typeof(ArgumentNullException), expected);
                Assert.IsAssignableFrom(typeof(CircuitBreakerOpenException), circuitBreakerOpenException);
                Assert.Null(unexpected);
                Assert.Equal(expectedEmptyString, string.Empty);
                Assert.NotEqual(expectedNotEmptyString, string.Empty);
            };
            t.Start();
        }

        [Fact]
        public void Breaker_holds_open_for_50_threads_during_openinterval()
        {
            // arrange 
            const int threadCount = 50;

            Exception expected = null;
            Exception unexpected = null;

            List<Exception> circuitBreakerOpenExceptions = new List<Exception>();
            List<string> expectedEmptyStrings = new List<string>();

            sut.OpenToHalfOpenWaitTime = TimeSpan.FromMilliseconds(10000);

            // act
            try
            {
                sut.ExecuteAction(() => { throw new ArgumentNullException(); });
            }
            catch (Exception ex)
            {
                expected = ex;
            }

            var tasks = new Task[threadCount];

            for (var i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    try
                    {
                        sut.ExecuteAction(() =>
                        {
                            expectedEmptyStrings.Add("I should not get executed.");
                        });
                    }
                    catch (CircuitBreakerOpenException ex)
                    {
                        circuitBreakerOpenExceptions.Add(ex);
                    }
                    catch (Exception ex)
                    {
                        unexpected = ex;
                    }
                });
            }

            // assert
            Task.WaitAll(tasks);
            Assert.IsAssignableFrom(typeof(ArgumentNullException), expected);
            Assert.Null(unexpected);
            Assert.Empty(expectedEmptyStrings);
            Assert.NotEmpty(circuitBreakerOpenExceptions);
        }

        [Fact]
        public void Breaker_closes_after_5_successful_executions()
        {
            // arrange 
            Exception expected = null;
            Exception unexpected = null;
            Exception circuitBreakerOpenException = null;
            string expectedNotEmptyString = string.Empty;

            // act
            try
            {
                sut.ExecuteAction(() => { throw new ArgumentNullException(); });
            }
            catch (Exception ex)
            {
                expected = ex;
            }

            var expectedEmptyString = string.Empty;
            try
            {
                sut.ExecuteAction(() => { expectedEmptyString = "I should not get executed."; });
            }
            catch (CircuitBreakerOpenException ex)
            {
                circuitBreakerOpenException = ex;
            }
            catch (Exception ex)
            {
                unexpected = ex;
            }

            Assert.True(sut.IsOpen);

            var t = new Timer();
            t.Interval = 501;
            
            t.Elapsed += (sender, args) =>
            {
                for (int i = 0; i <= sut.SuccessfulOperationsThreshold; i++)
                {
                    try
                    {
                        sut.ExecuteAction(() => { expectedNotEmptyString = "I should get executed."; });
                        Assert.True(sut.IsOpen);
                    }
                    catch (Exception ex)
                    {
                        // supress for test
                    }
                }

                // assert
                Assert.True(sut.IsClosed);
                Assert.IsAssignableFrom(typeof(ArgumentNullException), expected);
                Assert.IsAssignableFrom(typeof(CircuitBreakerOpenException), circuitBreakerOpenException);
                Assert.Null(unexpected);
                Assert.Empty(expectedEmptyString);
                Assert.NotEmpty(expectedNotEmptyString);
            };
            t.Start();
        }
    }
}
