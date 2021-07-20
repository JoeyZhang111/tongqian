using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using MultiBus.ILog.Model;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class JBCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as JB_HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null || String.IsNullOrWhiteSpace(paramObj.orderid) || String.IsNullOrWhiteSpace(paramObj.status))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeID = paramObj.orderid;
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
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("status", paramObj.status);
            dic.Add("msg", paramObj.msg);
            dic.Add("message", paramObj.message);
            dic.Add("userid", paramObj.userid);
            dic.Add("orderid", paramObj.orderid);
            dic.Add("orderno", paramObj.orderno);
            dic.Add("productid", paramObj.productid);
            dic.Add("account", paramObj.account);
            dic.Add("time", paramObj.time.ToString());
            var newdic = dic.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            string singStr = null;
            foreach (string item in newdic.Keys)
            {
                if (!String.IsNullOrWhiteSpace(newdic[item])) {
                    singStr += newdic[item];
                }
            }
            singStr = singStr + AppSecret;
            string mySing = PubClass.MD5(singStr).ToUpper();
            if (mySing != paramObj.sign)
            {
                result.IsSuccess = false;
                result.Msg = "签名错误";
                result.ResponseData = result.Msg;
                return result;
            }

            if (paramObj.status == "2")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = paramObj.msg;
                chargeResult.ChannelSerialNumber = paramObj.orderno;
            }
            else if (paramObj.status == "3")
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChargeMeessage = paramObj.msg;
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
            result.ResponseData = isOk ? "ok" : "同步失败";

            return result;

        }
        public class JB_HandleData
        {
            public string status { get; set; }
            public string msg { get; set; }
            public string message { get; set; }
            public string userid { get; set; }
            public string orderid { get; set; }
            public string orderno { get; set; }
            public string productid { get; set; }
            public string account { get; set; }
            public long? time { get; set; }
            public string sign { get; set; }
        }
    }
}