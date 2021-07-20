using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
using System.Collections.Generic;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    /// <summary>
    /// 深圳米粒
    /// </summary>
    public class SZMLCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as SZML_HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null || String.IsNullOrWhiteSpace(paramObj.userid) || String.IsNullOrWhiteSpace(paramObj.orderno) || String.IsNullOrWhiteSpace(paramObj.account) || String.IsNullOrWhiteSpace(paramObj.status) || String.IsNullOrWhiteSpace(paramObj.sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeID = paramObj.orderno;
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
            string userid = supplierInfo.SupplierSupply.AppKey;
            string appSecret = supplierInfo.SupplierSupply.AppSecret;
            string mySign = PubClass.MD5($"account={paramObj.account}&orderno={paramObj.orderno}&status={paramObj.status}&userid={userid}&key={appSecret}").ToLower();
            if (mySign != paramObj.sign)
            {
                result.IsSuccess = false;
                result.Msg = "签名验证失败";
                result.ResponseData = result.Msg;
                return result;
            }
            if (paramObj.status == "3")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "充值成功";
            }
            else if (paramObj.status == "2" ||paramObj.status == "4" || paramObj.status == "6" || paramObj.status == "7")
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
            result.ResponseData = isOk ? "success" : "同步失败";

            return result;

        }
        public class SZML_HandleData
        {
            public string userid { get; set; }
            public string orderno { get; set; }
            public string account { get; set; }
            public string status { get; set; }
            public string sign { get; set; }
        }
    }
}
