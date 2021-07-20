using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
using MultiBus.ILog.Model;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class MGXWCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as MGXW_HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null || string.IsNullOrWhiteSpace(paramObj.Result) || string.IsNullOrWhiteSpace(paramObj.Alias) || string.IsNullOrWhiteSpace(paramObj.OrderNo) || string.IsNullOrWhiteSpace(paramObj.OutNo) || string.IsNullOrWhiteSpace(paramObj.SignStr))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeID = paramObj.OutNo;
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
            string SignStr = paramObj.Result + paramObj.Alias + paramObj.OrderNo + paramObj.OutNo + AppSecret;
            var sign = PubClass.MD5(SignStr).ToUpper();
            if (sign != paramObj.SignStr)
            {
                MultiBus.Log.Factory.LogInstance.Loger.Write(new LogEntity { ExtendType = "提交参数：", LogMsg = "签名不正确,实体：" + JsonHelper.GetJson(paramObj) });
                result.IsSuccess = false;
                result.Msg = "签名不正确";
                result.ResponseData = result.Msg;
                return result;
            };
            if (paramObj.Result == "0")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "回调通：充值成功";
            }
            else if (paramObj.Result == "1")
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChargeMeessage = "回调：充值失败;描述：" + paramObj.ResultDesc;
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
        public class MGXW_HandleData
        {
            public string Result { get; set; }
            public string ResultDesc { get; set; }
            public string Alias { get; set; }
            public string OrderNo { get; set; }
            public string OutNo { get; set; }
            public string SignStr { get; set; }


        }
    }
}