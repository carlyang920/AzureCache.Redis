using System;
using System.Collections.Generic;
using System.Diagnostics;
using AzureCache.Redis.Lib.Interfaces;
using AzureCache.Redis.Lib.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;

namespace AzureCache.Redis.Tests.Services
{
    [TestClass]
    public class RedisServiceTest : BaseServiceTest
    {
        private readonly IRedisService _redisService;

        public RedisServiceTest()
        {
            _redisService = new RedisService("[your Redis connection]", 0);
        }

        [TestMethod]
        public void TestKeyExists()
        {
            var key = Sha256Encrypt($"{DateTime.Now:yyyy-MM-ddHHmmssfff}_Test");

            try
            {
                var result = false;

                if (!_redisService.KeyExists(key))
                    result = _redisService.StringSet(
                        new List<KeyValuePair<string, RedisValue>>()
                        {
                            new KeyValuePair<string, RedisValue>(key, "This is test")
                        },
                        out var errorMsg,
                        100
                    );

                Assert.IsTrue(result && _redisService.KeyExists(key));
            }
            catch (Exception e)
            {
                Debug.Write(e);
                throw;
            }
            finally
            {
                if (_redisService.KeyExists(key))
                    _redisService.KeyDelete(key);
            }
        }

        [TestMethod]
        public void TestKeyDelete()
        {
            var key = Sha256Encrypt($"{DateTime.Now:yyyy-MM-ddHHmmssfff}_Test");

            try
            {
                var result = false;

                if (!_redisService.KeyExists(key))
                    result = _redisService.StringSet(
                        new List<KeyValuePair<string, RedisValue>>()
                        {
                            new KeyValuePair<string, RedisValue>(key, "This is test")
                        },
                        out var errorMsg,
                        100
                    );

                _redisService.KeyDelete(key);

                Assert.IsTrue(result && !_redisService.KeyExists(key));
            }
            catch (Exception e)
            {
                Debug.Write(e);
                throw;
            }
            finally
            {
                if (_redisService.KeyExists(key))
                    _redisService.KeyDelete(key);
            }
        }

        [TestMethod]
        public void TestStringSet()
        {
            var key1 = Sha256Encrypt($"{Guid.NewGuid()}_Test");
            var key2 = Sha256Encrypt($"{Guid.NewGuid()}_Test");
            var key3 = Sha256Encrypt($"{Guid.NewGuid()}_Test");

            try
            {
                var result = false;

                result = _redisService.StringSet(
                    new List<KeyValuePair<string, RedisValue>>()
                    {
                        new KeyValuePair<string, RedisValue>(key1, "This is test1"),
                        new KeyValuePair<string, RedisValue>(key2, "This is test2"),
                        new KeyValuePair<string, RedisValue>(key3, "This is test3")
                    },
                    out var errorMsg,
                    100
                );

                Assert.IsTrue(result
                              && _redisService.KeyExists(key1)
                              && _redisService.KeyExists(key2)
                              && _redisService.KeyExists(key3)
                );
            }
            catch (Exception e)
            {
                Debug.Write(e);
                throw;
            }
            finally
            {
                _redisService.KeyDelete(key1);
                _redisService.KeyDelete(key2);
                _redisService.KeyDelete(key3);
            }
        }

        [TestMethod]
        public void TestStringGet()
        {
            var key1 = Sha256Encrypt($"{Guid.NewGuid()}_Test");

            try
            {
                var result = false;

                if (!_redisService.KeyExists(key1))
                    _redisService.StringSet(
                        new List<KeyValuePair<string, RedisValue>>()
                        {
                            new KeyValuePair<string, RedisValue>(key1, "This is test1")
                        },
                        out var errorMsg1,
                        100
                    );

                result = _redisService.StringGet(key1, out var value, out var errorMsg);

                Assert.IsTrue(result && string.Equals(value, "This is test1"));
            }
            catch (Exception e)
            {
                Debug.Write(e);
                throw;
            }
            finally
            {
                _redisService.KeyDelete(key1);
            }
        }

        [TestMethod]
        public void TestHashSet()
        {
            var key1 = Sha256Encrypt($"{DateTime.Now:yyyy-MM-ddHHmmssfff}_Test");

            try
            {
                var result = false;

                if (!_redisService.KeyExists(key1))
                    _redisService.HashSet(
                        key1,
                        new List<KeyValuePair<string, RedisValue>>()
                        {
                            new KeyValuePair<string, RedisValue>(Guid.NewGuid().ToString(), "This is test1"),
                            new KeyValuePair<string, RedisValue>(Guid.NewGuid().ToString(), "This is test2"),
                            new KeyValuePair<string, RedisValue>(Guid.NewGuid().ToString(), "This is test3")
                        },
                        out var errorMsg1,
                        100
                    );

                result = _redisService.HashGet(key1, out var values, out var errorMsg);

                Assert.IsTrue(result && 3 == values.Count);
            }
            catch (Exception e)
            {
                Debug.Write(e);
                throw;
            }
            finally
            {
                _redisService.KeyDelete(key1);
            }
        }

        [TestMethod]
        public void TestHashGet()
        {
            var key1 = Sha256Encrypt($"{DateTime.Now:yyyy-MM-ddHHmmssfff}_Test");

            try
            {
                var result = false;

                if (!_redisService.KeyExists(key1))
                    _redisService.HashSet(
                        key1,
                        new List<KeyValuePair<string, RedisValue>>()
                        {
                            new KeyValuePair<string, RedisValue>(Guid.NewGuid().ToString(), "This is test1"),
                            new KeyValuePair<string, RedisValue>(Guid.NewGuid().ToString(), "This is test2"),
                            new KeyValuePair<string, RedisValue>(Guid.NewGuid().ToString(), "This is test3")
                        },
                        out var errorMsg1,
                        100
                    );

                result = _redisService.HashGet(key1, out var values, out var errorMsg);

                Assert.IsTrue(result && 3 == values.Count);
            }
            catch (Exception e)
            {
                Debug.Write(e);
                throw;
            }
            finally
            {
                _redisService.KeyDelete(key1);
            }
        }
    }
}
