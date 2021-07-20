using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using KCWN.MultiBussiness.Core.OModel.QueueModel;
using KCWN.PublicClass;
using MultiBus.ILog.Model;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class ANCACallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as ANCA_HandleData;
            ResultObj result = new ResultObj();
            if (string.IsNullOrWhiteSpace(paramObj.userId) || string.IsNullOrWhiteSpace(paramObj.bizId) || string.IsNullOrWhiteSpace(paramObj.ejId) || string.IsNullOrWhiteSpace(paramObj.userId) || string.IsNullOrWhiteSpace(paramObj.downstreamSerialno) || string.IsNullOrWhiteSpace(paramObj.status) || string.IsNullOrWhiteSpace(paramObj.sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeId = paramObj.downstreamSerialno;
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
            //进行验签
            var supplierInfo = QuerySupplyInfo(chargeOrder.SupplierSupplyID);
            if (supplierInfo == null || supplierInfo.SupplierSupply == null || String.IsNullOrWhiteSpace(supplierInfo.SupplierSupply.AppKey))
            {
                result.IsSuccess = false;
                result.Msg = "订单渠道信息识别失败";
                result.ResponseData = result.Msg;
                return result;
            }
            string AppSecret = supplierInfo.SupplierSupply.AppSecret;
            var signStr = paramObj.bizId + paramObj.downstreamSerialno + paramObj.ejId + paramObj.status + paramObj.userId + AppSecret;
            var mySign = PubClass.MD5(signStr).ToLower();
            if (mySign != paramObj.sign)
            {
                result.IsSuccess = false;
                result.Msg = "签名异常";
                result.ResponseData = result.Msg;
                return result;
            }
            //订单状态 2 是成功 3 是失败
            if (paramObj.status == "2")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "充值成功";
                chargeResult.ChannelSerialNumber = paramObj.ejId;
            }
            else if (paramObj.status == "3")
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChargeMeessage = "充值失败";
            }

            if (chargeResult.ChargeState == ChargeState.Wait)
            {
                result.IsSuccess = false;
                result.Msg = "通知订单结果异常";
                result.ResponseData = "";
                return result;
            }
            bool isOk = SaveChargeResult(chargeOrder, chargeResult);
            result.IsSuccess = isOk;
            result.Msg = isOk ? "同步成功" : "同步失败";
            result.ResponseData = isOk?"success":"同步失败";
            return result;

        }
        public class ANCA_HandleData
        {
            public string userId { get; set; }
            public string bizId { get; set; }
            public string ejId { get; set; }
            public string downstreamSerialno { get; set; }
            public string status { get; set; }
            public string sign { get; set; }
            public string voucher { get; set; }
            public string voucherType { get; set; }
            public string ext { get; set; }

        }
    }
  


}