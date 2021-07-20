using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCWN.PublicClass;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class LSXDCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as LSXD_HandleData;
            ResultObj result = new ResultObj();
            if (string.IsNullOrWhiteSpace(paramObj.merchantId) || string.IsNullOrWhiteSpace(paramObj.outTradeNo) || string.IsNullOrWhiteSpace(paramObj.rechargeAccount) || string.IsNullOrWhiteSpace(paramObj.status) || string.IsNullOrWhiteSpace(paramObj.sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeId = paramObj.outTradeNo;
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
            //进行验签
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("merchantId", paramObj.merchantId);
            dic.Add("outTradeNo", paramObj.outTradeNo);
            dic.Add("rechargeAccount", paramObj.rechargeAccount);
            dic.Add("status", paramObj.status);
            string mySign = PubClass.MD5($"{PubClass.DictionaryToString(dic, 1)}&key={AppSecret}").ToUpper();
            if (mySign != paramObj.sign)
            {
                result.IsSuccess = false;
                result.Msg = "签名异常";
                result.ResponseData = result.Msg;
                return result;
            }
            if (paramObj.status == "01")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "充值成功";
            }
            else if (paramObj.status == "03")
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
        public class LSXD_HandleData
        {
            public string merchantId { get; set; }
            public string outTradeNo { get; set; }
            public string rechargeAccount { get; set; }
            public string status { get; set; }
            public string sign { get; set; }

        }
    }
   


}