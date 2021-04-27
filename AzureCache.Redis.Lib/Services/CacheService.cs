using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzureCache.Redis.Lib.Helpers;
using AzureCache.Redis.Lib.Interfaces;
using AzureCache.Redis.Lib.Interfaces;
using StackExchange.Redis;

namespace AzureCache.Redis.Lib.Services
{
    public class CacheService : ICacheService
    {
        private readonly IRedisService _redisService;

        public CacheService(
            IRedisService redisService
            )
        {
            _redisService = redisService;
        }

        public bool Exists(string key)
        {
            return _redisService.KeyExists(key);
        }

        public bool Delete(string key)
        {
            return _redisService.KeyDelete(key);
        }

        public string GetValue(string key)
        {
            if (!_redisService.KeyExists(key)) return null;

            var data = new StringBuilder();

            if (_redisService.HashGet(
                key,
                out var values,
                out var errorMsg
            ))
            {
                var temp = values
                    .Select(x => new
                    {
                        x.Key,
                        x.Value
                    })
                    .OrderBy(x => int.Parse(x.Key))
                    .ToList()
                    .Select(x => (byte[]) x.Value)
                    .ToList();

                var offset = 0;
                var bytes = new byte[temp.Sum(x => x.Length)];
                temp.ForEach(x =>
                {
                    Buffer.BlockCopy(
                        x, 
                        0, 
                        bytes,
                        offset, 
                        x.Length
                        );

                    offset += x.Length;
                });

                data.Append(GZipHelper.Decompress(bytes));
            }
            else
            {
                Console.WriteLine($"[{nameof(CacheService)}] {nameof(GetValue)}()");
                Console.WriteLine($"CurrentTime: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                Console.WriteLine($"Error Msg: {errorMsg}");
            }

            return data.ToString();
        }

        public void SetValue(string key, string value, int? expirySeconds = null)
        {
            var count = 0;
            var offset = 1048576;//1 mb
            var zipped = GZipHelper.Compress(value);

            var batchSet = new List<KeyValuePair<string, RedisValue>>();

            //Prepare batch list to insert
            do
            {
                var data = zipped.Skip(count * offset).Take(offset).ToArray();

                if (0 >= data.Length)
                    break;

                batchSet.Add(new KeyValuePair<string, RedisValue>($"{count + 1}", data));

                count += 1;
            } while (true);

            if (!_redisService.HashSet(
                key,
                batchSet,
                out var errorMsg,
                expirySeconds ?? 259200//3 days
            ))
            {
                Console.WriteLine($"[{nameof(CacheService)}] {nameof(SetValue)}()");
                Console.WriteLine($"CurrentTime: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                Console.WriteLine($"Error Msg: {errorMsg}");
            }
        }
    }
}
