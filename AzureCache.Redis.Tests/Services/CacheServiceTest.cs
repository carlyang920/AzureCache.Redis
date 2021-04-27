using System;
using System.Collections.Generic;
using System.Diagnostics;
using AzureCache.Redis.Lib.Helpers;
using AzureCache.Redis.Lib.Interfaces;
using AzureCache.Redis.Lib.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace AzureCache.Redis.Tests.Services
{
    [TestClass]
    public class CacheServiceTest
    {
        private readonly ICacheService _cacheService;

        public CacheServiceTest()
        {
            _cacheService = new CacheService(
                new RedisService("[your Redis connection]", 0)
                );
        }

        [TestMethod]
        public void TestExists()
        {
            var key = HashHelper.Sha256Encrypt($"{DateTime.Now:yyyy-MM-ddHHmmssfff}_Test");

            try
            {
                var dataList = new List<string>();

                for(var i = 0;i < 1000000; i++)
                {
                    dataList.Add($"This is test{i}");
                }

                _cacheService.SetValue(
                    key,
                    JsonConvert.SerializeObject(dataList, Formatting.None)
                );

                Assert.IsTrue(_cacheService.Exists(key));
            }
            catch (Exception e)
            {
                Debug.Write(e);
                throw;
            }
            finally
            {
                if (_cacheService.Exists(key))
                    _cacheService.Delete(key);
            }
        }

        [TestMethod]
        public void TestSetValue()
        {
            var key = HashHelper.Sha256Encrypt($"{DateTime.Now:yyyy-MM-ddHHmmssfff}_Test");

            try
            {
                var result = false;

                var dataList = new List<string>();

                for (var i = 0; i < 1000000; i++)
                {
                    dataList.Add($"This is test{i}");
                }

                if (!_cacheService.Exists(key))
                    _cacheService.SetValue(
                        key,
                        JsonConvert.SerializeObject(dataList, Formatting.None)
                    );

                result = JsonConvert.DeserializeObject<List<string>>(_cacheService.GetValue(key)).Count > 0;

                Assert.IsTrue(result);
            }
            catch (Exception e)
            {
                Debug.Write(e);
                throw;
            }
            finally
            {
                if (_cacheService.Exists(key))
                    _cacheService.Delete(key);
            }
        }

        [TestMethod]
        public void TestGetValue()
        {
            var key = HashHelper.Sha256Encrypt($"{DateTime.Now:yyyy-MM-ddHHmmssfff}_Test");

            try
            {
                var result = false;

                var dataList = new List<string>();

                for (var i = 0; i < 1000000; i++)
                {
                    dataList.Add($"This is test{i}");
                }

                if (!_cacheService.Exists(key))
                    _cacheService.SetValue(
                        key,
                        JsonConvert.SerializeObject(dataList, Formatting.None)
                    );

                var list = JsonConvert.DeserializeObject<List<string>>(_cacheService.GetValue(key));

                result = list.Count > 0;

                Assert.IsTrue(result);
            }
            catch (Exception e)
            {
                Debug.Write(e);
                throw;
            }
            finally
            {
                if (_cacheService.Exists(key))
                    _cacheService.Delete(key);
            }
        }
    }
}
