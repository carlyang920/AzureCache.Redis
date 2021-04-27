using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCache.Redis.Lib.Connections;
using AzureCache.Redis.Lib.Interfaces;
using StackExchange.Redis;

namespace AzureCache.Redis.Lib.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _cacheDb;

        public RedisService(string redisConnection, int dbIdx = -1)
        {
            RedisConnection.Init(redisConnection);

            _cacheDb = RedisConnection.Instance.ConnectionMultiplexer.GetDatabase(dbIdx);
        }

        public bool KeyExists(string key) => _cacheDb.KeyExists(key);

        public bool KeyDelete(string key) => _cacheDb.KeyDelete(key);

        public bool StringSet(
            List<KeyValuePair<string, RedisValue>> values,
            out string errorMsg,
            int expireSeconds = -1
            )
        {
            errorMsg = string.Empty;

            if (null != values && 0 < values.Count)
            {
                var skipped = 0;
                const int batchSize = 1000;

                IEnumerable<KeyValuePair<string, RedisValue>> batched;

                while ((batched = values.Skip(skipped).Take(batchSize)).Any())
                {
                    var batch = _cacheDb.CreateBatch();

                    var tasks = batched.Select(x => batch.StringSetAsync(
                            x.Key,
                            x.Value,
                            new TimeSpan(0, 0, expireSeconds))
                        )
                        .Cast<Task>()
                        .ToArray();

                    batch.Execute();

                    Task.WaitAll(tasks);

                    skipped += batchSize;
                }

                return true;
            }

            errorMsg = "Key or Value can't be null or empty";

            return false;
        }

        public bool StringGet(
            string key, 
            out RedisValue value,
            out string errorMsg
            )
        {
            value = string.Empty;
            errorMsg = string.Empty;

            if (string.IsNullOrEmpty(key))
            {
                errorMsg = "Key can't be null or empty";

                return false;
            }

            if (!_cacheDb.KeyExists(key))
            {
                errorMsg = "Key doesn't exist";

                return false;
            }

            value = _cacheDb.StringGet(key);

            return true;
        }

        public bool HashSet(
            string key,
            List<KeyValuePair<string, RedisValue>> values,
            out string errorMsg,
            int expireSeconds = -1
            )
        {
            errorMsg = string.Empty;

            if (null != values && 0 < values.Count)
            {
                var batch = _cacheDb.CreateBatch();
                var tasks = new List<Task>();

                values.ForEach(x =>
                {
                    tasks.Add(
                        batch.HashSetAsync(
                            key,
                            new[]
                            {
                                new HashEntry(x.Key, x.Value)
                            }
                            )
                        );
                });

                batch.Execute();

                Task.WaitAll(tasks.ToArray());

                if (KeyExists(key))
                    _cacheDb.KeyExpire(key, new TimeSpan(0, 0, expireSeconds));

                return true;
            }

            errorMsg = "Key or Value can't be null or empty";

            return false;
        }

        public bool HashGet(
            string key,
            out List<KeyValuePair<string, RedisValue>> values, 
            out string errorMsg
            )
        {
            values = null;
            errorMsg = string.Empty;

            if (string.IsNullOrEmpty(key))
            {
                errorMsg = "Key can't be null or empty";

                return false;
            }

            if (!_cacheDb.KeyExists(key))
            {
                errorMsg = "Key doesn't exist";

                return false;
            }

            values = _cacheDb
                .HashGetAll(key)
                .Select(x => 
                    new KeyValuePair<string, RedisValue>(x.Name, x.Value)
                )
                .ToList();

            return true;
        }
    }
}
