using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.QueueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Common
{
    public class CommQueueApi
    {
        /// <summary>
        /// 从订单调度队列写入数据
        /// </summary>
        /// <returns></returns>
        public static void PushOrderDispatcherQueue(OrderDispatcherQueueModel model) => RedisOrderDispatcherHelper.Helper.Enqueue(CommonKey.OrderDispatcherQueueKey, model);

        /// <summary>
        /// 从订单调度队列取出数据
        /// </summary>
        /// <returns></returns>
        public static OrderDispatcherQueueModel GetOrderDispatcherQueue() => RedisOrderDispatcherHelper.Helper.Dequeue<OrderDispatcherQueueModel>(CommonKey.OrderDispatcherQueueKey);

        /// <summary>
        /// 写入供货商调度队列
        /// </summary>
        /// <returns></returns>
        public static void PushSupplierDispatcherQueue(SupplierDispatcherQueueModel model) => RedisOrderDispatcherHelper.Helper.Enqueue(CommonKey.SupplierDispatcherQueue, model);

        /// <summary>
        /// 取出供货商调度队列数据
        /// </summary>
        /// <returns></returns>
        public static SupplierDispatcherQueueModel GetSupplierDispatcherQueue() => RedisOrderDispatcherHelper.Helper.Dequeue<SupplierDispatcherQueueModel>(CommonKey.SupplierDispatcherQueue);

        /// <summary>
        /// 写入供货商充值结果调度队列
        /// </summary>
        /// <returns></returns>
        public static void PushSupplierChargeResultDispacherQueue(SupplierChargeResultDispacherQueueModel chargeResult) => RedisOrderDispatcherHelper.Helper.Enqueue(CommonKey.SupplierChargeResultDispacherQueue, chargeResult);


        /// <summary>
        /// 获取供货商充值结果调度数据
        /// </summary>
        /// <returns></returns>
        public static SupplierChargeResultDispacherQueueModel GetSupplierChargeResultDispacherQueue() => RedisOrderDispatcherHelper.Helper.Dequeue<SupplierChargeResultDispacherQueueModel>(CommonKey.SupplierChargeResultDispacherQueue);

        /// <summary>
        /// 写入订单充值结果调度数据
        /// </summary>
        /// <returns></returns>
        public static void PushOrderChargeResultDispacherQueue(OrderChargeResult chargeResult) => RedisOrderDispatcherHelper.Helper.Enqueue(CommonKey.OrderChargeResultDispacherQueue, chargeResult);

        /// <summary>
        /// 获取订单充值结果调度数据
        /// </summary>
        /// <returns></returns>
        public static OrderChargeResult GetOrderChargeResultDispacherQueue() => RedisOrderDispatcherHelper.Helper.Dequeue<OrderChargeResult>(CommonKey.OrderChargeResultDispacherQueue);


        /// <summary>
        /// 获取查单队列
        /// </summary>
        /// <returns></returns>
        public static SupplierOrderQueryQueueModel GetOrderQueryQueue() => RedisOrderDispatcherHelper.Helper.Dequeue<SupplierOrderQueryQueueModel>(CommonKey.OrderQueryQueue);

        /// <summary>
        /// 写入查单队列
        /// </summary>
        /// <param name="model"></param>
        public static void PushOrderQueryQueue(SupplierOrderQueryQueueModel model) => RedisOrderDispatcherHelper.Helper.Enqueue(CommonKey.OrderQueryQueue, model);

        /// <summary>
        /// 获取通知服务数据
        /// </summary>
        /// <returns></returns>
        public static CallbackNotificationQueueModel GetCallbackNotificationQueue() => RedisOrderDispatcherHelper.Helper.Dequeue<CallbackNotificationQueueModel>(CommonKey.CallbackNotificationServer);

        /// <summary>
        /// 写入通知服务队列
        /// </summary>
        /// <param name="model"></param>
        public static void PushCallbackNotificationServer(CallbackNotificationQueueModel model) => RedisOrderDispatcherHelper.Helper.Enqueue(CommonKey.CallbackNotificationServer, model);
        /// <summary>
        /// 写入查单/回调待处理Hash表
        /// </summary>
        /// <param name="model"></param>
        public static void PushOrderQueryAndCallbackProcessHash(string orderid, OrderQueryAndCallbackProcessHashModel model) => RedisOrderDispatcherHelper.Helper.PushHash(CommonKey.OrderQueryAndCallbackProcessHash, orderid, model);

        public static OrderQueryAndCallbackProcessHashModel GetOrderQueryAndCallbackProcessHash(string orderId) => RedisOrderDispatcherHelper.Helper.PopHash<OrderQueryAndCallbackProcessHashModel>(CommonKey.OrderQueryAndCallbackProcessHash, orderId);

        /// <summary>
        /// 清除查单/回调处理Hash
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool DeleteOrderQueryAndCallbackProcessHash(long orderId) => RedisOrderDispatcherHelper.Helper.DeleteHash(CommonKey.OrderQueryAndCallbackProcessHash, orderId.ToString());
    }
}