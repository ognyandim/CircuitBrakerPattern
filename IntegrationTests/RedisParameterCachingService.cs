using System;
using StackExchange.Redis;

namespace IntegrationTests
{
    public class RedisParameterCachingService : IDummyService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisParameterCachingService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }
        public string Get(string parameterKey)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();
            return db.StringGet(parameterKey);
        }

        public void Put(string parameterKey, string parameterValue)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();
            db.StringSet(parameterKey, parameterValue);
        }
    }
}