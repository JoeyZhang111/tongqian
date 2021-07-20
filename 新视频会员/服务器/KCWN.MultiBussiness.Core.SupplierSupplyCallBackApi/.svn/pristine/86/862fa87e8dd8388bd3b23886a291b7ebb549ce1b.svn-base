using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class RULICallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as RULI_HandleData;
            ResultObj result = new ResultObj();
            string chargeID = paramObj.oid;

            if (string.IsNullOrWhiteSpace(paramObj.ste) || string.IsNullOrWhiteSpace(paramObj.cid) || string.IsNullOrWhiteSpace(paramObj.oid) || string.IsNullOrWhiteSpace(paramObj.sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
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
            string signStr = $"{paramObj.ste}+{paramObj.cid}+{paramObj.oid}+{paramObj.pn}+{AppSecret}";
            string sign = PubClass.MD5(signStr).ToLower();
            if (sign != paramObj.sign)
            {
                result.IsSuccess = false;
                result.Msg = "签名验证失败";
                result.ResponseData = result.Msg;
                return result;
            }

            if (paramObj.ste == "0")//0为成功，1为失败 (已与对方确认，和查单状态值不一致，对方要求按文档为准)
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "回调:充值成功";
            }
            else if (paramObj.ste == "1")
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChargeMeessage = "回调:充值失败";
            }
            if (chargeResult.ChargeState == ChargeState.Wait)
            {
                result.IsSuccess = false;
                result.Msg = "通知订单结果异常";
                result.ResponseData = "";
                return result;
            }

            //同步订单
            bool isOk = SaveChargeResult(chargeOrder, chargeResult);
            result.IsSuccess = isOk;
            result.Msg = isOk ? "同步成功" : "同步失败";
            result.ResponseData = isOk ? "success" : "同步失败";

            return result;

        }
        public class RULI_HandleData
        {
            public string ste { get; set; }
            public string cid { get; set; }
            public string oid { get; set; }
            public string pn { get; set; }
            public string sign { get; set; }
            public string info1 { get; set; }
            public string info2 { get; set; }

        }
    }



}