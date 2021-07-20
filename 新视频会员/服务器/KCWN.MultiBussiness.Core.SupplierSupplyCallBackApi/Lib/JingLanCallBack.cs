using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    /// <summary>
    /// 净蓝
    /// </summary>
    public class JingLanCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as JingLan_HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null || string.IsNullOrWhiteSpace(paramObj.data) || string.IsNullOrWhiteSpace(paramObj.sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            RequestEntityData dataModel;
            try
            {
                dataModel = JsonHelper.FromJson<RequestEntityData>(paramObj.data);
            }
            catch (Exception)
            {
                result.IsSuccess = false;
                result.Msg = "Data格式错误";
                result.ResponseData = result.Msg;
                return result;
            }
            if (dataModel == null || string.IsNullOrWhiteSpace(dataModel.merAccount) || string.IsNullOrWhiteSpace(dataModel.orderNo) || string.IsNullOrWhiteSpace(dataModel.merOrderNo))
            {
                result.IsSuccess = false;
                result.Msg = "Data参数不全";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeID = dataModel.merOrderNo;
            var chargeOrder = QueryOrdersChargeRecordByID(chargeID);
            if (chargeOrder == null)
            {
                result.IsSuccess = false;
                result.Msg = "订单无法识别或不存在";
                result.ResponseData = result.Msg;
                return result;
            }
            if (chargeOrder.ChargeState != (int)ChargeState.Wait)
            {
                result.IsSuccess = false;
                result.Msg = "订单已有充值结果拒绝接受通知";
                result.ResponseData = result.Msg;
                return result;
            }

            var supplierInfo = QuerySupplyInfo(chargeOrder.SupplierSupplyID);
            if (supplierInfo == null || supplierInfo.SupplierSupply == null || String.IsNullOrWhiteSpace(supplierInfo.SupplierSupply.AppKey))
            {
                result.IsSuccess = false;
                result.Msg = "订单渠道信息识别失败";
                result.ResponseData = result.Msg;
                return result;
            }

            string AppSecret = supplierInfo.SupplierSupply.AppSecret;
            string signStr = "data=" + paramObj.data + "&key=" + AppSecret;
            string sign = PubClass.MD5(signStr);
            if (!sign.Equals(paramObj.sign, StringComparison.OrdinalIgnoreCase))
            {
                result.IsSuccess = false;
                result.Msg = "签名验证不匹配";
                result.ResponseData = result.Msg;
                return result;
            }
            if (dataModel.orderState == 24)
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "回调:充值成功";
            }
            else if (dataModel.orderState == 23)
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChargeMeessage = "回调:充值失败";
            }
            if (chargeResult.ChargeState == ChargeState.Wait)
            {
                result.IsSuccess = false;
                result.Msg = "通知订单结果异常";
                result.ResponseData = result.Msg;
                return result;
            }

            //同步订单
            bool isOk = SaveChargeResult(chargeOrder, chargeResult);
            result.IsSuccess = isOk;
            result.Msg = isOk ? "同步成功" : "同步失败";
            result.ResponseData = isOk ? "OK" : "同步失败";

            return result;

        }
        public class JingLan_HandleData
        {
            public string data { get; set; }
            public string sign { get; set; }

        }
        /// <summary>
        /// 净蓝订单回调Data类。
        /// </summary>
        public class RequestEntityData
        {
            /// <summary>
            /// 商户账号
            /// </summary>
            public string merAccount { get; set; }

            /// <summary>
            /// 商户用户Id
            /// </summary>
            public string merUserAccount { get; set; }

            /// <summary>
            /// 业务类型
            /// </summary>
            public int businessType { get; set; }

            /// <summary>
            /// 商户订单
            /// </summary>
            public string merOrderNo { get; set; }

            /// <summary>
            /// 商户订单时间yyyy-MM-dd HH:mm:ss
            /// </summary>
            public string merOrderTime { get; set; }

            /// <summary>
            /// 系统订单号
            /// </summary>
            public string orderNo { get; set; }

            /// <summary>
            /// 订单接收订单时间yyyy-MM-dd HH:mm:ss
            /// </summary>
            public string orderTime { get; set; }

            /// <summary>
            /// 订单涉及总金额，以厘为单位
            /// </summary>
            public long orderAmount { get; set; }

            /// <summary>
            /// 实际应付金额，以厘为单位
            /// </summary>
            public long payAmount { get; set; }

            /// <summary>
            /// 最终结算金额，以厘为单位
            /// </summary>
            public long discountAmount { get; set; }

            /// <summary>
            /// 成功到账金额，订单状态为失败时返回0
            /// </summary>
            public long successAmount { get; set; }

            /// <summary>
            /// 订单详情
            /// </summary>
            public string orderDetail { get; set; }

            /// <summary>
            /// 订单状态，下单成功时返回值为0
            /// </summary>
            public int orderState { get; set; }

            /// <summary>
            /// 订单状态描述
            /// </summary>
            public string orderStatusDesc { get; set; }

            /// <summary>
            /// 商户自定义字段，原样返回
            /// </summary>
            public string attach { get; set; }

        }
    }



}