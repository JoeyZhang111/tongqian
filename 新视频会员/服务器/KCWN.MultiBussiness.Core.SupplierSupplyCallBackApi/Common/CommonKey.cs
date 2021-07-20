using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Common
{
    public static class CommonKey
    {
        /// <summary>
        /// 订单调度队列
        /// </summary>
        public readonly static string OrderDispatcherQueueKey = "MusOrderDispatcherQueue";
        /// <summary>
        /// 供货商调度队列
        /// </summary>
        public readonly static string SupplierDispatcherQueue = "SupDispatcherQueue";
        /// <summary>
        /// 供货商充值结果调度队列
        /// </summary>
        public readonly static string SupplierChargeResultDispacherQueue = "SupplierChargeResultDispacherQueue";
        /// <summary>
        /// 订单最终充值结果调度队列
        /// </summary>
        public readonly static string OrderChargeResultDispacherQueue = "OrderChargeResultDispacherQueue";
        /// <summary>
        /// 订单查单队列
        /// </summary>
        public readonly static string OrderQueryQueue = "OrderQueryQueue";
        /// <summary>
        /// 订单回调通知队列
        /// </summary>
        public readonly static string CallbackNotificationServer = "CallbackNotificationServer";

        /// <summary>
        /// 非供货结束订单hash
        /// </summary>
        public readonly static string UnusualOrderHash = "UnusualOrderKey";
        /// <summary>
        /// 回调/查询等待处理Hash
        /// </summary>

        public readonly static string OrderQueryAndCallbackProcessHash = "OrderQueryAndCallbackProcessHash";
    }
}