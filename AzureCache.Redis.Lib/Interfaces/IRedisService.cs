using System.Collections.Generic;
using StackExchange.Redis;

namespace AzureCache.Redis.Lib.Interfaces
{
    public interface IRedisService
    {
        bool KeyExists(string key);

        bool KeyDelete(string key);

        bool StringSet(
            List<KeyValuePair<string, RedisValue>> values,
            out string errorMsg,
            int expireSeconds = -1
        );

        bool StringGet(
            string key,
            out RedisValue value,
            out string errorMsg
        );

        bool HashSet(
            string key,
            List<KeyValuePair<string, RedisValue>> values,
            out string errorMsg,
            int expireSeconds = -1
        );

        bool HashGet(
            string key,
            out List<KeyValuePair<string, RedisValue>> values,
            out string errorMsg
        );
    }
}
