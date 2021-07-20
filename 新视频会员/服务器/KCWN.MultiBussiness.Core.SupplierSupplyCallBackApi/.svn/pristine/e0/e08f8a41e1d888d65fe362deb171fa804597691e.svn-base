using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    /// <summary>
    /// 聚合数据
    /// </summary>
    public class JHSJCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as JHSJ_HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null || String.IsNullOrWhiteSpace(paramObj.sporder_id) || String.IsNullOrWhiteSpace(paramObj.orderid) || String.IsNullOrWhiteSpace(paramObj.sta) || String.IsNullOrWhiteSpace(paramObj.sign))
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
            string mySing = PubClass.MD5(supplierInfo.SupplierSupply.AppKey + paramObj.sporder_id + paramObj.orderid).ToLower();
            if (mySing != paramObj.sign)
            {
                result.IsSuccess = false;
                result.Msg = "签名错误";
                result.ResponseData = result.Msg;
                return result;
            }
            if (paramObj.sta == "1")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChannelSerialNumber = paramObj.sporder_id;
                chargeResult.ChargeMeessage = String.IsNullOrWhiteSpace(paramObj.err_msg) ? "回调：充值成功" : paramObj.err_msg;
            }
            else if (paramObj.sta == "9")
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChannelSerialNumber = paramObj.sporder_id;
                chargeResult.ChargeMeessage = String.IsNullOrWhiteSpace(paramObj.err_msg) ? "回调：充值失败" : paramObj.err_msg;
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
        public class JHSJ_HandleData
        {
            public string sporder_id { get; set; }
            public string orderid { get; set; }
            public string sta { get; set; }
            public string sign { get; set; }
            public string err_msg { get; set; }

        }
    }
}