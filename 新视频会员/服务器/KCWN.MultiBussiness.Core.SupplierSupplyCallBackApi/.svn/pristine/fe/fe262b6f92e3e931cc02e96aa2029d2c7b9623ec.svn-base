using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using KCWN.PublicClass;
using MultiBus.ILog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class RYCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as RY_HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null || string.IsNullOrWhiteSpace(paramObj.orderNo) || string.IsNullOrWhiteSpace(paramObj.thirdOrderNo) || string.IsNullOrWhiteSpace(paramObj.rechargeTarget) || string.IsNullOrWhiteSpace(paramObj.code))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeID = paramObj.thirdOrderNo;
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
         
            if (paramObj.code == "2")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChannelSerialNumber = paramObj.orderNo;
                chargeResult.ChargeMeessage = "回调:充值成功";
            }
            else if (paramObj.code == "3")
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChannelSerialNumber = paramObj.orderNo;
                chargeResult.ChargeMeessage = "回调:充值失败";
            }
            if (chargeResult.ChargeState == ChargeState.Wait)
            {
                result.IsSuccess = false;
                result.Msg = "通知订单结果异常";
                result.ResponseData = result.Msg;
                return result;
            }
            bool isOk = SaveChargeResult(chargeOrder, chargeResult);
            result.IsSuccess = isOk;
            result.Msg = isOk ? "同步成功" : "同步失败";
            result.ResponseData = isOk ? "ok" : "同步失败";

            return result;

        }
        public class RY_HandleData
        {
            public string orderNo { get; set; }
            public string thirdOrderNo { get; set; }
            public string rechargeTarget { get; set; }
            public string code { get; set; }
            public string msg { get; set; }
        }
    }
}