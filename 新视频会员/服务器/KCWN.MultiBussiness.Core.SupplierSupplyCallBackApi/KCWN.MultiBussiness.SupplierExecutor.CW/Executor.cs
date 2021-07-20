using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.ISupplierExecutor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KCWN.MultiBussiness.Core.OModel.SupplierSubmitResult;
using KCWN.MultiBussiness.Core.OModel.V_Model;
using KCWN.MultiBussiness.Core.OModel.EnumInfo;
using KabapayCommon.Log.Factory;
using KabapayCommon.Log.ILog.Model;
using KCWN.PublicClass;
using System.Threading;
using KCWN.MultiBussiness.Core.OModel.DBModel;
using System.Security.Cryptography;

namespace KCWN.MultiBussiness.SupplierExecutor.CW
{
    public class Executor : IQueryOrder<SupplierQueryOrderParam>, ISubmitOrder<SupplierSubmitParam>
    {
        private string AllAttrKey = "SubmitUrl|QueryUrl";//检查的所有扩展属性
        private string ExecutorName = "橙券";
        private static readonly Dictionary<string, string> SubmitFalureResultCode = new Dictionary<string, string>
        {
            { "7001", "请求参数错误	" },
            { "7002", "请求超时" },
            { "7003", "商户账号不存在" },
            { "7004	", "商户账号状态暂停或禁用" },
            { "7005", "签名错误" },
            { "7006", "请求IP有误" },
            { "7008", "查询不到产品" },
            { "7009", "产品已下架" },
            { "7010", "查询不到商户密价" },
            { "7011", "商户密价状态暂停" },
            { "7014", "商户账号余额不足" },
            { "7019", "产品库存不足" },
        };
        private bool CheckSupplierSupplyAttr(List<T_SupplierSupplyAttr> list, out string msg)
        {
            msg = "";
            List<string> keyList = AllAttrKey.Split('|').ToList();
            list = list.Where(m => String.IsNullOrWhiteSpace(m.Value) == false).ToList();
            bool isOK = true;
            for (int i = 0; i < keyList.Count; i++)
            {
                if (!list.Exists(m => m.Name == keyList[i]))
                {
                    isOK = false;
                    msg = keyList[i];
                    return isOK;
                }
            }
            return isOK;
        }
        private Dictionary<string, T_SupplierSupplyAttr> GetAttrList(List<T_SupplierSupplyAttr> list)
        {
            Dictionary<string, T_SupplierSupplyAttr> attrList = new Dictionary<string, T_SupplierSupplyAttr>();
            list.ForEach(m =>
            {
                attrList.Add(m.Name, m);
            });
            return attrList;

        }

        public SupplierQueryOrderResult Query(SupplierQueryOrderParam task)
        {
            string guid = Guid.NewGuid().ToString();
            string ExtendType = $"GUID:{guid},【{ExecutorName}=>查单】";
            LogInstance.Loger.Write(new LogEntity { ExtendType = ExtendType, LogMsg = $"【初始数据】，{JsonHelper.GetJson(task)}" });
            SupplierQueryOrderResult queryResultObj = new SupplierQueryOrderResult();
            queryResultObj.State = ChargeState.Wait;
            queryResultObj.ChargeMessage = "初始等待状态";
            try
            {
                var attrList = GetAttrList(task.SupplierSupplyAttr);
                var timestamp = PubClass.GetUnixTimestamp(DateTime.Now);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("app_id", task.SupplierInfo.AppKey);
                dic.Add("timestamp", timestamp.ToString());
                dic.Add("order_no", task.Orders.ChargeID);
                dic.Add("version", "1.1.0");
                string signStr = PubClass.DictionaryToString(dic, 1, "=", "&") + $"&key={task.SupplierInfo.AppSecret}";
                var sign = PubClass.MD5(signStr).ToUpper();
                dic.Add("sign", sign);
                var requestData = PubClass.DictionaryToString(dic);
                string queryUrl = attrList["QueryUrl"].Value;
                HttpHelper httpObj = PubClass.HttpRequest(queryUrl, requestData, true);
                LogInstance.Loger.Write(new LogEntity { ExtendType = ExtendType, LogMsg = $"【查单请求】，请求接口：{queryUrl},请求数据：{requestData}，请求响应：{(httpObj.IsSuccess ? httpObj.ResponseData : $"请求响应异常：{httpObj.ErrorMsg}")}" });
                if (!httpObj.IsSuccess)
                {
                    queryResultObj.State = ChargeState.Wait;
                    queryResultObj.ChargeMessage = "查单请求失败";
                    return queryResultObj;
                }
                QueryResponseData model = JsonHelper.FromJson<QueryResponseData>(httpObj.ResponseData);
                if (model != null && model.code == "7000" && model.data != null && model.data.order_no == task.Orders.ChargeID)
                {
                    if (model.data.state == "SUCCESS")
                    {
                        queryResultObj.State = ChargeState.Success;
                        queryResultObj.ChargeMessage = "充值成功";
                        return queryResultObj;
                    }
                    else if (model.data.state == "FAILURE")
                    {
                        queryResultObj.State = ChargeState.Success;
                        queryResultObj.ChargeMessage = "充值失败";
                        return queryResultObj;
                    }
                    else
                    {
                        queryResultObj.State = ChargeState.Wait;
                        queryResultObj.ChargeMessage = "充值中";
                        return queryResultObj;

                    };
                }
                return queryResultObj;
            }
            catch (Exception ex)
            {
                queryResultObj.State = ChargeState.Wait;
                queryResultObj.ChargeMessage = "执行器异常";
                LogInstance.Loger.Write(new LogEntity { ExtendType = ExtendType, LogMsg = $"【执行器异常】，异常：{ex.Message},详情：{ex.StackTrace}" });
                return queryResultObj;
            }

        }

        public SupplierSubmitResult Submit(SupplierSubmitParam task)
        {
            string guid = Guid.NewGuid().ToString();
            string ExtendType = $"GUID:{guid},【{ExecutorName}=>提单】";
            LogInstance.Loger.Write(new LogEntity { ExtendType = ExtendType, LogMsg = $"初始化数据：{JsonHelper.GetJson(task)}" });
            DateTime time = DateTime.Now;
            SupplierSubmitResult submitResult = new SupplierSubmitResult();
            submitResult.SubmitState = SupplierSubmitState.Unkown;
            submitResult.SubmitMessage = "初始未知状态";
            submitResult.SubmitTime = DateTime.Now.ToString("yyyyHHddHHmmss");
            try
            {
                string outMsg;
                if (!CheckSupplierSupplyAttr(task.SupplierSupplyAttr, out outMsg))
                {
                    submitResult.SubmitState = SupplierSubmitState.Fail;
                    submitResult.SubmitMessage = $"缺少自定义属性：{outMsg}";
                    submitResult.ResultType = ResultType.ExecuteParameterError;
                    return submitResult;
                }
                if (String.IsNullOrWhiteSpace(task.SupplierProductRelation.SupplierProductCode))
                {
                    submitResult.SubmitState = SupplierSubmitState.Fail;
                    submitResult.SubmitMessage = $"缺少渠道产品编号";
                    submitResult.ResultType = ResultType.ExecuteParameterError;
                    return submitResult;
                }
                var attrList = GetAttrList(task.SupplierSupplyAttr);
                var timestamp = PubClass.GetUnixTimestamp(DateTime.Now);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("app_id", task.SupplierInfo.AppKey);
                dic.Add("timestamp", timestamp.ToString());
                dic.Add("order_no", task.Orders.ChargeID);
                dic.Add("recharge_number", task.Orders.ChargeAccount);
                dic.Add("product_id", task.SupplierProductRelation.SupplierProductCode);
                dic.Add("amount", task.Orders.BuyCount.ToString());
                dic.Add("version", "1.1.0");
                string signStr = PubClass.DictionaryToString(dic, 1, "=", "&") + $"&key={task.SupplierInfo.AppSecret}";
                var sign = PubClass.MD5(signStr).ToUpper();
                dic.Add("sign", sign);
                var requestData = PubClass.DictionaryToString(dic);
                string submitUrl = attrList["SubmitUrl"].Value;
                HttpHelper httpObj = PubClass.HttpRequest(submitUrl, requestData, true);
                LogInstance.Loger.Write(new LogEntity { ExtendType = ExtendType, LogMsg = $"【提单请求】，请求接口：{submitUrl},请求数据：{requestData}，请求响应：{(httpObj.IsSuccess ? httpObj.ResponseData : $"请求响应异常：{httpObj.ErrorMsg}")}" });
                if (!httpObj.IsSuccess)
                {
                    submitResult.SubmitState = SupplierSubmitState.Unkown;
                    submitResult.SubmitMessage = $"提单请求异常：{httpObj.ErrorMsg}";
                    return submitResult;
                }
                SubmitResponseData model = JsonHelper.FromJson<SubmitResponseData>(httpObj.ResponseData);
                if (model == null || String.IsNullOrEmpty(model.code))
                {
                    submitResult.SubmitState = SupplierSubmitState.Unkown;
                    submitResult.SubmitMessage = $"提单返回数据异常";
                    return submitResult;
                }
                if (model.code == "7000")
                {
                    submitResult.SubmitState = SupplierSubmitState.Success;
                    submitResult.SubmitMessage = "提单成功";
                    if (model.data.state == "SUCCESS")
                    {
                        submitResult.ChargeResult = new SupplierHandleChargeResult()
                        {
                            State = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState.Success,
                            ChargeMessage = "充值成功"
                        };
                        return submitResult;
                    }
                    else if (model.data.state == "FAILURE")
                    {
                        submitResult.ChargeResult = new SupplierHandleChargeResult()
                        {
                            State = KCWN.MultiBussiness.Core.OModel.EnumInfo.ChargeState.Fail,
                            ChargeMessage = "充值失败"
                        };
                        return submitResult;
                    }
                    return submitResult;
                }
                else if (SubmitFalureResultCode.ContainsKey(model.code))
                {
                    submitResult.SubmitState = SupplierSubmitState.Fail;
                    submitResult.SubmitMessage = "提单失败";
                    return submitResult;
                }
                else
                {
                    submitResult.SubmitState = SupplierSubmitState.Unkown;
                    submitResult.SubmitMessage = $"状态未知：（{model.code}）";
                    return submitResult;
                }
            }
            catch (Exception ex)
            {
                submitResult.SubmitState = SupplierSubmitState.Unkown;
                submitResult.SubmitMessage = "执行器异常";
                submitResult.ResultType = ResultType.ExecuteSystemError;
                LogInstance.Loger.Write(new LogEntity { ExtendType = ExtendType, LogMsg = $"【执行器异常】，异常：{ex.Message},详情：{ex.StackTrace}" });
                return submitResult;
            }
        }

        public class SubmitResponseData
        {
            public string code { get; set; }
            public string message { get; set; }
            public OrderObj data { get; set; }
            public class OrderObj
            {
                public string app_id { get; set; }
                public string order_no { get; set; }
                public string recharge_number { get; set; }
                public string start_time { get; set; }
                public string end_time { get; set; }
                public string state { get; set; }
                public string consume_amount { get; set; }
            }
        }

        public class QueryResponseData
        {
            public string code { get; set; }
            public string message { get; set; }
            public OrderObj data { get; set; }
            public class OrderObj
            {
                public string app_id { get; set; }
                public string order_no { get; set; }
                public string recharge_number { get; set; }
                public string start_time { get; set; }
                public string end_time { get; set; }
                public string state { get; set; }
                public string consume_amount { get; set; }
            }
        }
    }
}
