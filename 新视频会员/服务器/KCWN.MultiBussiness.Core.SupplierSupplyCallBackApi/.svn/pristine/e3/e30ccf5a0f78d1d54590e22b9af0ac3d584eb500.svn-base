using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib

{
    public class FULUCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as FULU_HandleData;
            ResultObj result = new ResultObj();
            if (string.IsNullOrWhiteSpace(paramObj.customer_order_no) || string.IsNullOrWhiteSpace(paramObj.order_status) || string.IsNullOrWhiteSpace(paramObj.sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeID = paramObj.customer_order_no;
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
            //当充值产品为卡密产品直接拒绝回调收单
            var productObj = QueryProduct(chargeOrder.OrderId);
            if (productObj == null) {
                result.IsSuccess = false;
                result.Msg = "订单产品信息识别鼠标";
                result.ResponseData = result.Msg;
                return result;
            }
            if (productObj.BusinessType == 2) {//卡密的回调不带卡密只能通过查单
                result.IsSuccess = false;
                result.Msg = "卡密产品回调,不做最终结果处理，直接响应：SUCCESS";
                result.ResponseData = "SUCCESS";
                return result;
            }
            string AppSecret = supplierInfo.SupplierSupply.AppSecret;
            if (paramObj.order_status == "success")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "充值成功";
                chargeResult.ChannelSerialNumber = paramObj.order_id;
            }
            else if (paramObj.order_status == "failed")
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
            result.ResponseData = isOk ? "SUCCESS" : "同步失败";

            return result;

        }
        public class FULU_HandleData
        {
            public string order_id { get; set; }
            public string charge_finish_time { get; set; }
            public string customer_order_no { get; set; }
            public string order_status { get; set; }
            public string recharge_description { get; set; }
            public string product_id { get; set; }
            public string price { get; set; }
            public string buy_num { get; set; }
            public string operator_serial_number { get; set; }
            public string sign { get; set; }

        }
    }



}