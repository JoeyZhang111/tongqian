using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using KCWN.MultiBussiness.Core.OModel.QueueModel;
using MultiBus.ILog.Model;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib
{
    /// <summary>
    /// 老视频会员系统
    /// </summary>
    public class HYXCMCallBack : ICallBack
    {
        public override ResultObj Handle<T>(T entity)
        {
            ResultObj result = new ResultObj();
            var re = entity as HYX_HandleData;
            if (re == null || re.MOrderID == null || re.State == null || re.Sign == null)
            {
                result.IsSuccess = false;
                result.Msg = "参数异常";
                result.ResponseData = new { Code = -1, Msg = result.Msg };
                return result;
            }
            string AppSecret = KCWN.PublicClass.Tools.GetConfigInfo("HYX_AppSecret");
            string mySign = KCWN.PublicClass.PubClass.MD5(re.AppKey + re.TimesTamp + AppSecret).ToUpper();
            if (mySign != re.Sign)
            {
                result.IsSuccess = false;
                result.Msg = "签名验证失败";
                result.ResponseData = new { Code = -1, Msg = result.Msg };
                return result;
            }

            var chargeOrder = QueryOrdersChargeRecordByID(re.MOrderID);

            if (chargeOrder == null)
            {
                result.IsSuccess = false;
                result.Msg = "订单无法识别或不存在";
                result.ResponseData = new { Code = -1, Msg = result.Msg };
                return result;
            }
            if (chargeOrder.ChargeState != (int)ChargeState.Wait)
            {
                result.IsSuccess = false;
                result.Msg = "订单已有充值结果拒绝接受通知";
                result.ResponseData = new { Code = -1, Msg = result.Msg };
                return result;
            }

            OrderChargeResult chargeResult = new OrderChargeResult();
            chargeResult.ChargeState = ChargeState.Wait;
            if (re.State == "2")
            {
                chargeResult.ChargeState = ChargeState.Success;
                chargeResult.ChargeMeessage = "充值成功";
                #region 解密卡密
                var productObj = QueryProduct(chargeOrder.OrderId);
                if (productObj != null && productObj.BusinessType == 2)
                {
                    if (re.ExtendParam != null && !String.IsNullOrEmpty(re.ExtendParam))
                    {
                        ExtendParamData cardObj = PublicClass.JsonHelper.FromJson<ExtendParamData>(re.ExtendParam);
                        if (cardObj != null && !String.IsNullOrEmpty(cardObj.CardPwd))
                        {
                            chargeResult.CardInfoModels = new List<OModel.V_Model.CardInfoModel>();
                            var memberKey = KCWN.PublicClass.Tools.GetConfigInfo("HYX_RSAPrivateKey");
                            var privateKeyByte = Convert.FromBase64String(memberKey);
                            AsymmetricKeyParameter privateKey = null;
                            privateKey = GetPrivateKey(privateKeyByte);
                            string cardNumber = null;
                            string cardPwd = null;
                            if (!String.IsNullOrEmpty(cardObj.CardNumber))
                            {
                                cardObj.CardNumber = cardObj.CardNumber.Replace(' ', '+');
                                var tem_cardNumber = Decrypt(privateKey, Convert.FromBase64String(cardObj.CardNumber));
                                cardNumber = Encoding.UTF8.GetString(tem_cardNumber);
                            }
                            if (!String.IsNullOrEmpty(cardObj.CardPwd))
                            {
                                cardObj.CardPwd = cardObj.CardPwd.Replace(' ', '+');
                                var tem_cardPwd = Decrypt(privateKey, Convert.FromBase64String(cardObj.CardPwd));
                                cardPwd = Encoding.UTF8.GetString(tem_cardPwd);
                            }
                            chargeResult.CardInfoModels.Add(new OModel.V_Model.CardInfoModel() { CardNumber = cardNumber, CardPwd = cardPwd, CardDeadline = cardObj.CardDeadline });
                            MultiBus.Log.Factory.LogInstance.Loger.Write(new LogEntity { ExtendType = $"【恒易讯】", LogMsg = $"卡密信息：{PublicClass.JsonHelper.GetJson(chargeResult.CardInfoModels)}" });
                        }
                    }
                }
                #endregion
            }
            else if (re.State == "3")
            {
                chargeResult.ChargeState = ChargeState.Fail;
                chargeResult.ChargeMeessage = "充值失败";
            }
            if (chargeResult.ChargeState == ChargeState.Wait)
            {
                result.IsSuccess = false;
                result.Msg = "订单状态非明确充值结果";
                result.ResponseData = new { Code = -1, Msg = result.Msg };
                return result;
            }
            if (chargeResult.ChargeState == ChargeState.Success && chargeResult.CardInfoModels == null)
            {
                result.IsSuccess = false;
                result.Msg = "卡密信息异常";
                result.ResponseData = new { Code = -1, Msg = result.Msg };
                return result;
            }
            //chargeResult.ResultType = ? ResultType.BalanceInsufficient : ResultType.ChargeSuccess;//充值结果描述
            bool isOk = SaveChargeResult(chargeOrder, chargeResult);
            result.IsSuccess = isOk;
            result.Msg = isOk ? "同步成功" : "同步失败";
            result.ResponseData = new { Code = 0 };
            return result;

        }
        public AsymmetricKeyParameter GetPrivateKey(byte[] privateKeyBtye)
        {
            return PrivateKeyFactory.CreateKey(privateKeyBtye);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public byte[] Decrypt(AsymmetricKeyParameter privateKey, byte[] inputData)
        {
            //非对称加密算法，加解密用 
            IAsymmetricBlockCipher engine = new RsaEngine();
            //私钥解密 
            engine.Init(false, privateKey);
            try
            {
                var returnData = engine.ProcessBlock(inputData, 0, inputData.Length);
                return returnData;
            }
            catch (Exception ex)
            {
                throw new Exception("解密失败:" + ex.Message);
            }
        }
        public class HYX_HandleData
        {
            public string AppKey { get; set; }
            public string TimesTamp { get; set; }
            public string Sign { get; set; }
            public string MOrderID { get; set; }
            public string OrderID { get; set; }
            public string State { get; set; }
            public string ChargeAccount { get; set; }
            public string ProductCode { get; set; }
            public string BuyCount { get; set; }
            public string ExtendParam { get; set; }

        }
        public class ExtendParamData
        {
            public string CardNumber { get; set; }
            public string CardPwd { get; set; }
            public string CardDeadline { get; set; }
        }
    }

}