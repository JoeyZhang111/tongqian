using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class ZUOWANCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as ZUOWAN_HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null || string.IsNullOrWhiteSpace(paramObj.channel_order_id) || string.IsNullOrWhiteSpace(paramObj.order_id) || string.IsNullOrWhiteSpace(paramObj.order_status) || string.IsNullOrWhiteSpace(paramObj.product_code) || string.IsNullOrWhiteSpace(paramObj.order_finish_time))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeId = paramObj.channel_order_id;
            var chargeOrder = QueryOrdersChargeRecordByID(chargeId);
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
            if (paramObj.order_status == "5")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "回调：充值成功";
                chargeResult.ChannelSerialNumber = paramObj.order_id;
            }
            else if (paramObj.order_status == "6")
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChargeMeessage = "回调：充值失败";
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
            result.ResponseData = isOk ? "success" : "同步失败";

            return result;

        }
        public class ZUOWAN_HandleData
        {
            public string channel_order_id { get; set; }
            public string order_id { get; set; }
            public string province { get; set; }
            public string product_code { get; set; }
            public string request_time { get; set; }
            public string order_status { get; set; }
            public string fail_reason { get; set; }
            public string order_finish_time { get; set; }

        }
    }



}