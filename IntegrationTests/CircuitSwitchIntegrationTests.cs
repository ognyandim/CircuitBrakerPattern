using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using CircuitBreakerPattern;
using CircuitBreakerPattern.Breaker;
using CircuitBreakerPattern.Breaker.Implementation;
using CircuitBreakerPattern.Switch.Implementation;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace IntegrationTests
{
    public class CircuitSwitchIntegrationTests
    {
        private CircuitSwitch sut;
        private readonly IDummyService redisService;
        private readonly CircuitBreaker circuitBreaker;
        private Mock<IDummyService> dummyServiceMock;

        public CircuitSwitchIntegrationTests()
        {
            ICircuitBreakerStateStore circuitBreakerStateStore = CircuitBreakerStateStoreFactory.GetCircuitBreakerStateStore();
            IEventLogger eventLogger = new EventLogger();
            circuitBreaker = new CircuitBreaker(circuitBreakerStateStore, eventLogger);

            sut = new CircuitSwitch(circuitBreaker, eventLogger);

            var conn = ConnectionMultiplexer.Connect("localhost");
            redisService = (IDummyService) new RedisParameterCachingService(conn);
            dummyServiceMock = new Mock<IDummyService>();
        }

        [Fact]
        public void Switch_calls_secondary_service_silently_when_default_service_fails()
        {
            // arrange
            string result = string.Empty;

            redisService.Put("testKey", "alternativePathValue");
            dummyServiceMock.Setup( x => x.Get(It.IsAny<string>())).Throws<TimeoutException>();

            sut.TryFirst(() => result = dummyServiceMock.Object.Get("testKey"))
               .Otherwise(() => result = redisService.Get("testKey"));

            // act
            sut.Execute();
            
            // assert
            Assert.Equal("alternativePathValue", result);
        }

        [Fact]
        public void Breaker_holds_open_for_50_threads_during_openinterval()
        {
            // arrange 
            string result = string.Empty;

            redisService.Put("testKey", "alternativePathValue");
            dummyServiceMock.Setup(x => x.Get(It.IsAny<string>())).Throws<TimeoutException>();

            sut.TryFirst(() => result = dummyServiceMock.Object.Get("testKey"))
               .Otherwise(() => result = redisService.Get("testKey"));

            const int threadCount = 50;

            circuitBreaker.OpenToHalfOpenWaitTime = TimeSpan.FromMilliseconds(1000);

            // act
            sut.Execute();
            Assert.Equal("alternativePathValue", result);

            dummyServiceMock.Setup(x => x.Get(It.IsAny<string>())).Returns("directPathValue");

            var tasks = new Task[threadCount];

            for (var i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                   sut.Execute();
                });
            }

            Task.WaitAll(tasks);

            // assert
            Assert.NotEqual("directPathValue", result);

            var t = new Timer();
            t.Interval = circuitBreaker.OpenToHalfOpenWaitTime.TotalMilliseconds * 2;
            t.Elapsed += (sender, args) =>
            {
                // assert
                sut.Execute();

                Assert.Equal("directPathValue", result);
            };
            t.Start();

           
           
        }
    }
}
