using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using KCWN.PublicClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    public class JiaNuocallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            var paramObj = entity as RequestEntityData;
            ResultObj result = new ResultObj();
            if (paramObj == null || string.IsNullOrWhiteSpace(paramObj.UserId) || string.IsNullOrWhiteSpace(paramObj.OrderNo) || string.IsNullOrWhiteSpace(paramObj.OrderStatus) || string.IsNullOrWhiteSpace(paramObj.AccountVal) || string.IsNullOrWhiteSpace(paramObj.Time) || string.IsNullOrWhiteSpace(paramObj.BizType) || string.IsNullOrWhiteSpace(paramObj.Sign))
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.Code = "-1";
                result.ResponseData = result.Msg;
                return result;
            }

            try
            {

                string chargeID = paramObj.OrderNo;
                var chargeOrder = QueryOrdersChargeRecordByID(chargeID);
                if (chargeOrder == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "订单无法识别或不存在";
                    result.Code = "-1";
                    result.ResponseData = result.Msg;
                    return result;
                }
                if (chargeOrder.ChargeState != (int)ChargeState.Wait)
                {
                    result.IsSuccess = false;
                    result.Msg = "订单已有充值结果拒绝接受通知";
                    result.Code = "-1";
                    result.ResponseData = result.Msg;
                    return result;
                }

                var supplierInfo = QuerySupplyInfo(chargeOrder.SupplierSupplyID);
                if (supplierInfo == null || supplierInfo.SupplierSupply == null || String.IsNullOrWhiteSpace(supplierInfo.SupplierSupply.AppKey) || !paramObj.UserId.Equals(supplierInfo.SupplierSupply.AppKey))
                {
                    result.IsSuccess = false;
                    result.Msg = "订单渠道信息识别失败";
                    result.Code = "-1";
                    result.ResponseData = result.Msg;
                    return result;
                }

                string AppSecret = supplierInfo.SupplierSupply.AppSecret;
                //判断签名是否有误
                string Sign = "AccountVal" + paramObj.AccountVal + "BizType" + paramObj.BizType + "OrderNo" + paramObj.OrderNo + "OrderStatus" + paramObj.OrderStatus;
                if (!string.IsNullOrWhiteSpace(paramObj.ProductData))
                {
                    Sign += "ProductData" + paramObj.ProductData;
                }
                Sign += "Time" + paramObj.Time + "UserId" + paramObj.UserId + AppSecret;
                string sgin = PubClass.MD5(Sign, "UTF-8", false);
                if (!sgin.Equals(paramObj.Sign))
                {
                    result.IsSuccess = false;
                    result.Msg = "签名验证不匹配";
                    result.Code = "-1";
                    result.ResponseData = result.Msg;
                    return result;
                }

                if (paramObj.OrderStatus.Equals("SUCCESS"))
                {
                    chargeResult.ChargeState = ChargeState.Success;
                    chargeResult.ChargeMeessage = "回调:充值成功";

                    #region 卡密解析(V3.0新加)
                    var productObj = QueryProduct(chargeOrder.OrderId);
                    if (productObj != null && productObj.BusinessType == 2 && String.IsNullOrWhiteSpace(paramObj.ProductData) == false)
                    {
                        List<ProductData> cardList = JsonHelper.FromJson<List<ProductData>>(paramObj.ProductData);
                        chargeResult.CardInfoModels = new List<OModel.V_Model.CardInfoModel>();
                        cardList.ForEach(m =>
                        {
                            chargeResult.CardInfoModels.Add(new OModel.V_Model.CardInfoModel() { CardNumber = m.code, CardPwd = m.key, CardLink = m.url, CardDeadline = m.effend });
                        });
                    }
                    #endregion
                }
                else if (paramObj.OrderStatus.Equals("FAILED"))
                {
                    chargeResult.ChargeState = ChargeState.Fail;
                    chargeResult.ChargeMeessage = "回调:充值失败";
                }
                else if (paramObj.OrderStatus.Equals("UNDERWAY"))
                {
                    chargeResult.ChargeState = ChargeState.Wait;
                    chargeResult.ChargeMeessage = "回调:下单成功，充值中";
                }
                else
                {
                    chargeResult.ChargeState = ChargeState.Unkown;
                    chargeResult.ChargeMeessage = "回调:充值结果未知";
                }


                if (chargeResult.ChargeState == ChargeState.Wait)
                {
                    result.IsSuccess = false;
                    result.Msg = chargeResult.ChargeMeessage;
                    result.ResponseData = result.Msg;
                    return result;
                }

                //同步订单
                bool isOk = SaveChargeResult(chargeOrder, chargeResult);
                result.IsSuccess = isOk;
                result.Msg = isOk ? "同步成功" : "同步失败";
                result.Code = isOk ? "0" : "-1";
                result.ResponseData = isOk ? "OK" : "同步失败";
            }
            catch (Exception)
            {
                result.IsSuccess = false;
                result.Msg = "Data格式错误";
                result.Code = "-1";
                result.ResponseData = result.Msg;

            }
            return result;
        }


        public class RequestEntityData
        {
            /// <summary>
            /// 账号
            /// </summary>
            public string UserId { get; set; }
            /// <summary>
            /// 业务类型
            /// </summary>
            public string BizType { get; set; }
            /// <summary>
            /// 订单号
            /// </summary>
            public string OrderNo { get; set; }
            /// <summary>
            /// 充值账号
            /// </summary>
            public string AccountVal { get; set; }
            /// <summary>
            /// 订单状态
            /// </summary>
            public string OrderStatus { get; set; }
            /// <summary>
            /// 订单产品数据
            /// </summary>
            public string ProductData { get; set; }
            /// <summary>
            /// 时间戳
            /// </summary>
            public string Time { get; set; }
            /// <summary>
            /// 签名值
            /// </summary>
            public string Sign { get; set; }
        }

        public class ProductData
        {
            public string type { get; set; }
            public string code { get; set; }
            public string key { get; set; }
            public string url { get; set; }
            public string effstart { get; set; }
            public string effend { get; set; }
            public object extinfo { get; set; }

        }
    }
}