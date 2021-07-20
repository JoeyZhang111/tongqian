using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{

    public class YDCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as YD_HandleData;
            ResultObj result = new ResultObj();
           
            if (paramObj==null|| string.IsNullOrWhiteSpace(paramObj.ordno) || string.IsNullOrWhiteSpace(paramObj.mercid) || string.IsNullOrWhiteSpace(paramObj.account) || string.IsNullOrWhiteSpace(paramObj.goodsid) || string.IsNullOrWhiteSpace(paramObj.goodsname) || String.IsNullOrWhiteSpace(paramObj.transid) || string.IsNullOrWhiteSpace(paramObj.status) || string.IsNullOrWhiteSpace(paramObj.sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeID = paramObj.ordno;
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
            var signStr = $"ordno={paramObj.ordno}&mercid={paramObj.mercid}&account={paramObj.account}&goodsid={paramObj.goodsid}&goodsname={paramObj.goodsname}&transid={paramObj.transid}&status={paramObj.status}&recordid={paramObj.recordid}&key={AppSecret}";
            var mySign = PubClass.MD5(signStr).ToUpper();
            if (mySign != paramObj.sign)
            {
                result.IsSuccess = false;
                result.Msg = "签名验证失败";
                result.ResponseData = result.Msg;
                return result;
            }

            if (paramObj.status == "1")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "充值成功";
                chargeResult.ChannelSerialNumber = paramObj.transid;
            }
            else if (paramObj.status == "2")
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
            result.ResponseData = isOk ? "ok" : "同步失败";
            return result;

        }
        public class YD_HandleData
        {
            public string ordno { get; set; }
            public string mercid { get; set; }
            public string account { get; set; }
            public string goodsid { get; set; }
            public string goodsname { get; set; }
            public string transid { get; set; }
            public string status { get; set; }
            public string recordid { get; set; }
            public string sign { get; set; }

        }
    }



}