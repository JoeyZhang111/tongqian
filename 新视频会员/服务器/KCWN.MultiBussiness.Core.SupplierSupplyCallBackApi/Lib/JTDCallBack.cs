using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class JTDCallBack : ICallBack
    {

        public override ResultObj Handle<T>(T entity)
        {
            ResultObj result = new ResultObj();
            var paramObj = entity as JTD_HandleData;
            if (paramObj == null||string.IsNullOrEmpty(paramObj.customId)||string.IsNullOrEmpty(paramObj.status))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeID = paramObj.customId;
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

            string AppKey = supplierInfo.SupplierSupply.AppKey;
            string mySign = PubClass.MD5(AppKey + paramObj.mobile + paramObj.status + paramObj.customId).ToLower();
            if (mySign != paramObj.sign)
            {
                result.IsSuccess = false;
                result.Msg = "签名错误";
                result.ResponseData = result.Msg;
                return result;
            }
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            if (paramObj.status == "7")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "充值成功";
                chargeResult.ChannelSerialNumber = paramObj.orderNum;
            }
            else if (paramObj.status == "8") {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChargeMeessage = "充值失败";
                chargeResult.ChannelSerialNumber = paramObj.orderNum;
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
            result.ResponseData = isOk ? "SUCCESS" : "同步失败";
            return result;

        }
        public class JTD_HandleData
        {
            public string mobile { get; set; }
            public string status { get; set; }
            public string customId { get; set; }
            public string orderNum { get; set; }
            public string sign { get; set; }
        }
    }
}