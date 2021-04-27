using System;
using StackExchange.Redis;

namespace AzureCache.Redis.Lib.Connections
{
    internal class RedisConnection
    {
        public readonly ConnectionMultiplexer ConnectionMultiplexer;

        private static readonly Lazy<RedisConnection> LazyConnection =
            new Lazy<RedisConnection>(() =>
            {
                if (string.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("No valid connection string");

                return new RedisConnection();
            });

        private static string _connectionString;

        private RedisConnection()
        {
            ConnectionMultiplexer = ConnectionMultiplexer.Connect(_connectionString);
        }

        public static void Init(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static RedisConnection Instance => LazyConnection.Value;
    }
}
