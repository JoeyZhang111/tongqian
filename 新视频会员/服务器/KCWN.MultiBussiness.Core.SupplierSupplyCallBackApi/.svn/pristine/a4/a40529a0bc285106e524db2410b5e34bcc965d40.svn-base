using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
using System.Collections.Generic;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class SxQyCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as SxQy_HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null || string.IsNullOrWhiteSpace(paramObj.appId) || string.IsNullOrWhiteSpace(paramObj.status) || string.IsNullOrWhiteSpace(paramObj.channelOrderNum) || string.IsNullOrWhiteSpace(paramObj.orderNum) || string.IsNullOrWhiteSpace(paramObj.status) || string.IsNullOrWhiteSpace(paramObj.sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeId = paramObj.channelOrderNum;
            string orderStatus = paramObj.status;
            string orderId = paramObj.orderNum;
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
            //进行验签
            string AppSecret = supplierInfo.SupplierSupply.AppSecret;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appId", paramObj.appId);
            dic.Add("appKey", AppSecret);
            dic.Add("timeStamp", paramObj.timeStamp.ToString());
            dic.Add("channelOrderNum", paramObj.channelOrderNum);
            dic.Add("orderNum", paramObj.orderNum);
            dic.Add("status", paramObj.status);
            string S = PubClass.DictionaryToString(dic, 1);
            string md5 = PubClass.MD5(S);
            if (md5.ToUpper() != paramObj.sign.ToUpper())
            {
                result.IsSuccess = false;
                result.Msg = "签名错误";
                result.ResponseData = result.Msg;
                return result;
            }

            if (orderStatus == "2")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "回调通：充值成功";
            }
            else if (orderStatus == "3")
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
            result.ResponseData = isOk ? "SUCCESS" : "同步失败";

            return result;

        }
        public class SxQy_HandleData
        {
            public string appId { get; set; }
            public long timeStamp { get; set; }
            public string channelOrderNum { get; set; }
            public string orderNum { get; set; }
            public string status { get; set; }
            public string sign { get; set; }
        }
    }



}