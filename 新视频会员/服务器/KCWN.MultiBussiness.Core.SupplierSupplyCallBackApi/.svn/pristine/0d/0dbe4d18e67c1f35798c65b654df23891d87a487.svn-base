using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
using System.Configuration;
using System.Collections.Generic;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class XMLYCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as XMLY_HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null || String.IsNullOrWhiteSpace(paramObj.xima_order_no) || paramObj.xima_order_status == null)
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = new { code = 1, message = result.Msg };
                return result;
            }
            string XMLY_SupplierSupplyID = ConfigurationManager.AppSettings["XMLY_SupplierSupplyID"];
            if (String.IsNullOrWhiteSpace(XMLY_SupplierSupplyID))
            {
                result.IsSuccess = false;
                result.Msg = "缺少XMLY_SupplierSupplyID配置";
                result.ResponseData = new { code = 1, message = result.Msg };
                return result;
            }
            Guid SupplierSupplyID = Guid.Parse(XMLY_SupplierSupplyID);
            string ChannelSerialNumber = paramObj.xima_order_no;
            var chargeOrder = QueryOrderChargeRecordByChannelSerialNumber(SupplierSupplyID, ChannelSerialNumber);//渠道订单号查询订单
            if (chargeOrder == null)
            {
                result.IsSuccess = false;
                result.Msg = "订单无法识别或不存在";
                result.ResponseData = new { code = 1, message = result.Msg };
                return result;
            }
            if (chargeOrder.ChargeState != (int)ChargeState.Wait)
            {
                result.IsSuccess = false;
                result.Msg = "订单已有充值结果拒绝接受通知";
                result.ResponseData = new { code = 1, message = result.Msg };
                return result;
            }

            var supplierInfo = QuerySupplyInfo(chargeOrder.SupplierSupplyID);
            if (supplierInfo == null || supplierInfo.SupplierSupply == null || String.IsNullOrWhiteSpace(supplierInfo.SupplierSupply.AppKey))
            {
                result.IsSuccess = false;
                result.Msg = "订单渠道信息识别失败";
                result.ResponseData = new { code = 1, message = result.Msg };
                return result;
            }

            string AppSecret = supplierInfo.SupplierSupply.AppSecret;
            Dictionary<string, string> dic = new Dictionary<string, string>() {
                {"app_key",paramObj.app_key},
                {"uid",paramObj.uid},
                {"xima_order_no",paramObj.xima_order_no},
                {"xima_order_status",paramObj.xima_order_status.ToString()},
                {"xima_order_created_at",paramObj.xima_order_created_at.ToString()},
                {"nonce",paramObj.nonce},
                {"timestamp",paramObj.timestamp},
            };
            string dic_str = PubClass.DictionaryToString(dic, 1) + "app_secret=" + AppSecret;
            string mySign = PubClass.MD5(dic_str).ToLower();
            if (mySign != paramObj.sig)
            {
                result.IsSuccess = false;
                result.Msg = "签名错误";
                result.ResponseData = result.Msg;
                return result;
            }
            if (paramObj.xima_order_status == 2)
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "充值成功";
                chargeResult.ChannelSerialNumber = paramObj.xima_order_no;
            }
            else if (paramObj.xima_order_status == 3)
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChargeMeessage = "充值失败";
                chargeResult.ChannelSerialNumber = paramObj.xima_order_no;
            }

            if (chargeResult.ChargeState == ChargeState.Wait)
            {
                result.IsSuccess = false;
                result.Msg = "通知订单结果异常";
                result.ResponseData = new { code = 1, message = result.Msg };
                return result;
            }

            //同步订单
            bool isOk = SaveChargeResult(chargeOrder, chargeResult);
            result.IsSuccess = isOk;
            result.Msg = isOk ? "同步成功" : "同步失败";
            result.ResponseData = new { code = 0, message = result.Msg };
            return result;

        }
        public class XMLY_HandleData
        {
            public string app_key { get; set; }
            public string uid { get; set; }
            public string xima_order_no { get; set; }
            public int? xima_order_status { get; set; }
            public int? xima_order_created_at { get; set; }
            public string nonce { get; set; }
            public string timestamp { get; set; }
            public string sig { get; set; }

        }
    }
}