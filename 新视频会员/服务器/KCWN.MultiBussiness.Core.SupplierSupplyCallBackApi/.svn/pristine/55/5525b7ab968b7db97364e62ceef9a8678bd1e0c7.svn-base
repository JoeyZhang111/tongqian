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
    public class CWCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as CW_HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null || string.IsNullOrWhiteSpace(paramObj.timestamp) || string.IsNullOrWhiteSpace(paramObj.app_id) || string.IsNullOrWhiteSpace(paramObj.sign) || string.IsNullOrWhiteSpace(paramObj.order_no)||String.IsNullOrWhiteSpace(paramObj.state))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeID = paramObj.order_no;
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
            //验证签名
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("timestamp",paramObj.timestamp);
            dic.Add("app_id", paramObj.app_id);
            dic.Add("sign", paramObj.sign);
            dic.Add("order_no", paramObj.order_no);
            dic.Add("state", paramObj.state);
            string signStr = PubClass.DictionaryToString(dic, 1, "=", "&") + $"&key={supplierInfo.SupplierSupply.AppSecret}";
            var mySign = PubClass.MD5(signStr).ToUpper();
            if (mySign != paramObj.sign) {
                result.IsSuccess = false;
                result.Msg = "签名验证失败";
                result.ResponseData = result.Msg;
                return result;
            }
            if (paramObj.state == "SUCCESS")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "回调:充值成功";
            }
            else if (paramObj.state == "FAILURE")
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
            bool isOk = SaveChargeResult(chargeOrder, chargeResult);
            result.IsSuccess = isOk;
            result.Msg = isOk ? "同步成功" : "同步失败";
            result.ResponseData = isOk ? "OK" : "同步失败";

            return result;

        }
        public class CW_HandleData
        {
            public string timestamp { get; set; }
            public string app_id { get; set; }
            public string sign { get; set; }
            public string order_no { get; set; }
            public string state { get; set; }
        }
    }
}