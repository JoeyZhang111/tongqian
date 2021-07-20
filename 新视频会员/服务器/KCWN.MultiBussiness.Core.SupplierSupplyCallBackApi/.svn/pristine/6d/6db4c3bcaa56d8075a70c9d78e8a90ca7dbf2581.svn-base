using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using System;
using ChargeState = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState;
using KCWN.PublicClass;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class TestCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as HandleData;
            ResultObj result = new ResultObj();
            if (paramObj == null)
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = result.Msg;
                return result;
            }
            string chargeID = paramObj.ChargeID;
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
            //进行验签


            //chargeResult.ResultType = re.ChargeState==3?ResultType.BalanceInsufficient: ResultType.ChargeSuccess;//充值结果描述
            //chargeResult.ChargeMeessage = $"测试供货商回调结果描述：{re.ChargeState}";

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
        public class HandleData
        {
            public string ChargeID { get; set; }
            public int? ChargeState { get; set; }

        }
    }
    


}