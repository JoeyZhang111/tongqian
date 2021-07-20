
using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class YIPACallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as YIPA_HandleData;
            string businessId = paramObj.businessId;
            string chargeId = paramObj.userOrderId;
            string orderStatus = paramObj.status;

            ResultObj result = new ResultObj();
            if (string.IsNullOrWhiteSpace(chargeId) || string.IsNullOrWhiteSpace(orderStatus) || string.IsNullOrWhiteSpace(businessId) || string.IsNullOrWhiteSpace(paramObj.sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
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

            string AppSecret = supplierInfo.SupplierSupply.AppSecret;
            string mysign = PubClass.MD5((paramObj.businessId + paramObj.userOrderId + paramObj.status + AppSecret).ToLower());
            if (mysign.ToLower() != paramObj.sign.ToLower())
            {
                result.IsSuccess = false;
                result.Msg = "签名错误";
                result.ResponseData = result.Msg;
                return result;
            }
            if (orderStatus == "01")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "充值成功";
            }
            else if (orderStatus == "02")
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChargeMeessage = "充值失败";
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
            result.ResponseData = isOk ? "<receive>ok</receive>" : "同步失败";

            return result;

        }
        public class YIPA_HandleData
        {
            public string businessId { get; set; }
            public string userOrderId { get; set; }
            public string status { get; set; }
            public string mes { get; set; }
            public string payoffPriceTotal { get; set; }
            public string sign { get; set; }

        }
    }



}