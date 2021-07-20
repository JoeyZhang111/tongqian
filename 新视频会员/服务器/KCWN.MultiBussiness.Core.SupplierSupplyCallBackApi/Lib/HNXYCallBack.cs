using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    /// <summary>
    ///湘悦
    /// </summary>
    public class HNXYCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as HNXY_HandleData;
            ResultObj result = new ResultObj();
            if (string.IsNullOrWhiteSpace(paramObj.retcode) || string.IsNullOrWhiteSpace(paramObj.status) || string.IsNullOrWhiteSpace(paramObj.customerorderno) || string.IsNullOrWhiteSpace(paramObj.orderno) || string.IsNullOrWhiteSpace(paramObj.status) || string.IsNullOrWhiteSpace(paramObj.sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeId = paramObj.customerorderno;
            string orderStatus = paramObj.status;
            string orderId = paramObj.orderno;
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
            string s = AppSecret + paramObj.retcode + paramObj.status + paramObj.retmsg + paramObj.orderno + paramObj.customerorderno + paramObj.t;
            string md5 = PubClass.MD5(s);
            if (md5.ToUpper() != paramObj.sign.ToUpper())
            {
                result.IsSuccess = false;
                result.Msg = "签名错误";
                result.ResponseData = result.Msg;
                return result;
            }
            if (orderStatus == "success")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "回调：充值成功";
                chargeResult.ChannelSerialNumber = paramObj.orderno;
            }
            else if (orderStatus == "fail")
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
            result.ResponseData = isOk ? "1" : "同步失败";

            return result;

        }
        public class HNXY_HandleData
        {
            public string retcode { get; set; }
            public string status { get; set; }
            public string orderno { get; set; }
            public string retmsg { get; set; }
            public string customerorderno { get; set; }
            public string cardnumber { get; set; }
            public string cardpwd { get; set; }
            public string carddeadline { get; set; }
            public string t { get; set; }
            public string sign { get; set; }

        }
    }



}