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
    public class RPCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as RP_HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null || string.IsNullOrWhiteSpace(paramObj.userid) || string.IsNullOrWhiteSpace(paramObj.orderid) || string.IsNullOrWhiteSpace(paramObj.sporderid) || string.IsNullOrWhiteSpace(paramObj.merchantsubmittime) || String.IsNullOrWhiteSpace(paramObj.resultno) || string.IsNullOrWhiteSpace(paramObj.sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeID = paramObj.sporderid;
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
            string mySign = PubClass.MD5($"userid={paramObj.userid}&orderid={paramObj.orderid}&sporderid={paramObj.sporderid}&merchantsubmittime={paramObj.merchantsubmittime}&resultno={paramObj.resultno}&key={AppSecret}").ToUpper();
            if (mySign != paramObj.sign)
            {
                result.IsSuccess = false;
                result.Msg = "签名错误";
                result.ResponseData = result.Msg;
                return result;

            }
            if (paramObj.resultno == "1")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChannelSerialNumber = paramObj.orderid;
                chargeResult.ChargeMeessage = "回调:充值成功";
            }
            else if (paramObj.resultno == "9")
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChannelSerialNumber = paramObj.orderid;
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
        public class RP_HandleData
        {
            public string userid { get; set; }
            public string orderid { get; set; }
            public string sporderid { get; set; }
            public string merchantsubmittime { get; set; }
            public string resultno { get; set; }
            public string sign { get; set; }
            public string parvalue { get; set; }
            public string remark1 { get; set; }
            public string fundbalance { get; set; }

        }
    }
}