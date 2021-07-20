
using KCWN.PublicClass;
using MultiBus.ILog.Model;
using MultiBus.Log.Factory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Common
{
    public class RedisOrderDispatcherHelper
    {
        private RedisOrderDispatcherHelper()
        {

        }
        public static RedisOrderDispatcherHelper Helper { get; } = new RedisOrderDispatcherHelper();

        protected virtual int PowerAuthRedisDBIndex
        {
            get
            {
                int dbindex = 0;

                try
                {
                    dbindex = int.Parse(ConfigurationManager.AppSettings["OrderDispatcherDBIndex"].ToString());
                }
                catch
                {
                    dbindex = 0;
                }

                return dbindex < 0 ? 0 : dbindex;
            }
        }
        /// <summary>
        /// 获取用户权限认证系统自定义 RedisKey 前缀。
        /// </summary>
        protected virtual string UserPowerAuthCustomRedisKeyPrefix
        {
            get
            {
                return string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["OrderDispatcherRedisKeyPrefix"]) ? "OrderDispatcher" : ConfigurationManager.AppSettings["OrderDispatcherRedisKeyPrefix"].ToString().Trim();
            }
        }



        public void Enqueue(string key, string value)
        {
            try
            {
                using (StackExchangeHelper redis = new StackExchangeHelper(PowerAuthRedisDBIndex))
                {
                    redis.SetCustomKeyPrefix(UserPowerAuthCustomRedisKeyPrefix);
                    redis.ListLeftPush(key, value);
                }
            }
            catch (Exception ex)
            {
                LogInstance.Loger.Write(new LogEntity { ExtendType = "RedisOrderDispatcherHelper", LogMsg = $"Redis异，KEY：{key}  VALUE：{value}", Error = ex });
            }

        }
        public void Enqueue<T>(string key, T value) where T : class
        {
            try
            {
                using (StackExchangeHelper redis = new StackExchangeHelper(PowerAuthRedisDBIndex))
                {
                    redis.SetCustomKeyPrefix(UserPowerAuthCustomRedisKeyPrefix);
                    redis.ListLeftPush(key, value);
                }
            }
            catch (Exception ex)
            {
                LogInstance.Loger.Write(new LogEntity { ExtendType = "RedisOrderDispatcherHelper", LogMsg = $"Redis异，KEY：{key}  VALUE：{value}", Error = ex });
            }

        }

        public string Dequeue(string key)
        {
            try
            {
                using (StackExchangeHelper redis = new StackExchangeHelper(PowerAuthRedisDBIndex))
                {
                    redis.SetCustomKeyPrefix(UserPowerAuthCustomRedisKeyPrefix);
                    return redis.ListRightPop(key);
                }
            }
            catch (Exception ex)
            {
                LogInstance.Loger.Write(new LogEntity { ExtendType = "RedisOrderDispatcherHelper", LogMsg = $"Redis异，KEY：{key}", Error = ex });
                return null;
            }
        }
        public T Dequeue<T>(string key)
        {
            try
            {
                using (StackExchangeHelper redis = new StackExchangeHelper(PowerAuthRedisDBIndex))
                {
                    redis.SetCustomKeyPrefix(UserPowerAuthCustomRedisKeyPrefix);
                    return redis.ListRightPop<T>(key);
                }
            }
            catch (Exception ex)
            {
                LogInstance.Loger.Write(new LogEntity { ExtendType = "RedisOrderDispatcherHelper", LogMsg = $"Redis异，KEY：{key} ", Error = ex });
                return default(T);
            }
        }


        public bool PushHash(string hashTableKey, string hashRowKey, string value)
        {
            try
            {
                using (StackExchangeHelper redis = new StackExchangeHelper(PowerAuthRedisDBIndex))
                {
                    redis.SetCustomKeyPrefix(UserPowerAuthCustomRedisKeyPrefix);
                    return redis.HashSet(hashTableKey, hashRowKey, value);
                }
            }
            catch (Exception ex)
            {
                LogInstance.Loger.Write(new LogEntity { ExtendType = "RedisOrderDispatcherHelper", LogMsg = $"Redis异，hashTableKey：{hashTableKey}  hashRowKey：{hashRowKey}  VALUE：{value}", Error = ex });
                return false;
            }
        }
        public bool PushHash<T>(string hashTableKey, string hashRowKey, T value)
        {
            try
            {
                using (StackExchangeHelper redis = new StackExchangeHelper(PowerAuthRedisDBIndex))
                {
                    redis.SetCustomKeyPrefix(UserPowerAuthCustomRedisKeyPrefix);
                    return redis.HashSet(hashTableKey, hashRowKey, value);
                }
            }
            catch (Exception ex)
            {
                //LogInstance.Loger.Write(new LogEntity { ExtendType = "RedisOrderDispatcherHelper", LogMsg = $"Redis异，hashTableKey：{hashTableKey}  hashRowKey：{hashRowKey}  VALUE：{value}", Error = ex });
                return false;
            }
        }

        public string PopHash(string hashTableKey, string hashRowKey)
        {
            try
            {
                using (StackExchangeHelper redis = new StackExchangeHelper(PowerAuthRedisDBIndex))
                {
                    redis.SetCustomKeyPrefix(UserPowerAuthCustomRedisKeyPrefix);
                    return redis.HashGet(hashTableKey, hashRowKey);
                }
            }
            catch (Exception ex)
            {
                //LogInstance.Loger.Write(new LogEntity { ExtendType = "RedisOrderDispatcherHelper", LogMsg = $"Redis异，hashTableKey：{hashTableKey}  hashRowKey：{hashRowKey}", Error = ex });

                return null;
            }
        }
        public T PopHash<T>(string hashTableKey, string hashRowKey)
        {
            try
            {
                using (StackExchangeHelper redis = new StackExchangeHelper(PowerAuthRedisDBIndex))
                {
                    redis.SetCustomKeyPrefix(UserPowerAuthCustomRedisKeyPrefix);
                    return redis.HashGet<T>(hashTableKey, hashRowKey);
                }
            }
            catch (Exception ex)
            {
                //LogInstance.Loger.Write(new LogEntity { ExtendType = "RedisOrderDispatcherHelper", LogMsg = $"Redis异，hashTableKey：{hashTableKey}  hashRowKey：{hashRowKey}", Error = ex });

                return default(T);
            }
        }

        public bool DeleteHash(string hashTableKey, string hashRowKey)
        {
            try
            {
                using (StackExchangeHelper redis = new StackExchangeHelper(PowerAuthRedisDBIndex))
                {
                    redis.SetCustomKeyPrefix(UserPowerAuthCustomRedisKeyPrefix);
                    return redis.HashDelete(hashTableKey, hashRowKey);
                }
            }
            catch (Exception ex)
            {
                //LogInstance.Loger.Write(new LogEntity { ExtendType = "RedisOrderDispatcherHelper", LogMsg = $"Redis异，hashTableKey：{hashTableKey}  hashRowKey：{hashRowKey}", Error = ex });

                return false;
            }
        }
    }
}