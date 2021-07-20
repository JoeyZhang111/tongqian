using KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib;
using KCWN.PublicClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using MultiBus.ILog.Model;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Controllers
{
    public class CallBackController : Controller
    {
        #region  老视频会员（直充回调）
        /// <summary>
        /// 老视频会员（直充回调）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult HYX(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.HYXCallBack.HYX_HandleData entity)
        {
            HYXCallBack item = new HYXCallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  老视频会员（卡密回调）
        /// <summary>
        /// 老视频会员（卡密回调）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult HYXKM(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.HYXCMCallBack.HYX_HandleData entity)
        {
            HYXCMCallBack item = new HYXCMCallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 娱尚互娱
        public JsonResult TOQI(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.TOQICallBack.TOQI_HandleData entity)
        {
            TOQICallBack item = new TOQICallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 安畅
        public JsonResult ANCA(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.ANCACallBack.ANCA_HandleData entity)
        {
            ANCACallBack item = new ANCACallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 蓝色兄弟
        public ActionResult LSXD(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.LSXDCallBack.LSXD_HandleData entity)
        {
            LSXDCallBack item = new LSXDCallBack();
            MyResult result = new MyResult();
            result.ContentType = "text/plain";
            result.ContentEncoding = System.Text.Encoding.UTF8;
            result.Data = item.Handle(entity).ResponseData.ToString();
            return result;
            //ResultObj resultObj = item.Handle(entity);
            //return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 瑞联
        public JsonResult RULI(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.RULICallBack.RULI_HandleData entity)
        {
            RULICallBack item = new RULICallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 易派
        public JsonResult YIPA(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.YIPACallBack.YIPA_HandleData entity)
        {
            YIPACallBack item = new YIPACallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 易点
        public JsonResult YD()
        {
            ResultObj result = null;
            try
            {
                var handle = new YDCallBack();
                string str = PubClass.GetHttpData("GBK");
                var param = JsonHelper.FromJson<YDCallBack.YD_HandleData>(str);
                result = handle.Handle(param);
            }
            catch (Exception)
            {
                result = new ResultObj
                {
                    IsSuccess = false,
                    Msg = "系统异常",
                    ResponseData = "系统异常"
                };
            }
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 江西拾兴
        public JsonResult SxQy(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.SxQyCallBack.SxQy_HandleData entity)
        {
            SxQyCallBack item = new SxQyCallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 湘悦
        public JsonResult HNXY(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.HNXYCallBack.HNXY_HandleData entity)
        {
            HNXYCallBack item = new HNXYCallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 净蓝
        public JsonResult JingLan(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.JingLanCallBack.JingLan_HandleData entity)
        {
            JingLanCallBack item = new JingLanCallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 福禄（直充）
        public JsonResult FULU(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.FULUCallBack.FULU_HandleData entity)
        {
            FULUCallBack item = new FULUCallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 聚通达
        public JsonResult JTD(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.JTDCallBack.JTD_HandleData entity)
        {
            JTDCallBack item = new JTDCallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 捷贝
        public ActionResult JB(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.JBCallBack.JB_HandleData entity)
        {
            MultiBus.Log.Factory.LogInstance.Loger.Write(new LogEntity { ExtendType = $"【请求实体】", LogMsg = (entity == null ? "为空" : JsonHelper.GetJson(entity)) });
            MyResult result = new MyResult();
            result.ContentType = "text/plain";
            result.ContentEncoding = System.Text.Encoding.UTF8;
            if (entity == null)
            {
                result.Data = "参数异常";
                return result;
            }
            JBCallBack item = new JBCallBack();
            result.Data = item.Handle(entity).ResponseData.ToString();
            return result;
        }
        #endregion

        #region 芒果秀玩
        public JsonResult MGXW(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.MGXWCallBack.MGXW_HandleData entity)
        {
            MGXWCallBack item = new MGXWCallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 佳诺直充
        public JsonResult JiaNuo(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.JiaNuocallBack.RequestEntityData entity)
        {
            JiaNuocallBack item = new JiaNuocallBack();
            ResultObj result = item.Handle(entity);
            if (result.Code == "0")
            {
                return Json(new { code = result.Code }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { code = result.Code, msg = result.Msg }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 聚合数据
        public JsonResult JHSJ(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.JHSJCallBack.JHSJ_HandleData entity)
        {
            JHSJCallBack item = new JHSJCallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 喜马拉雅
        public JsonResult XMLY(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.XMLYCallBack.XMLY_HandleData entity)
        {
            XMLYCallBack item = new XMLYCallBack();
            ResultObj result = item.Handle(entity);
            return Json(result.ResponseData, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 深圳米粒
        public ActionResult SZML(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.SZMLCallBack.SZML_HandleData entity)
        {
            SZMLCallBack item = new SZMLCallBack();
            MyResult result = new MyResult();
            result.ContentType = "text/plain";
            result.ContentEncoding = System.Text.Encoding.UTF8;
            result.Data = item.Handle(entity).ResponseData.ToString();
            return result;
        }
        #endregion

        #region 河北润普
        public ActionResult RP(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.RPCallBack.RP_HandleData entity)
        {
            RPCallBack item = new RPCallBack();
            MyResult result = new MyResult();
            result.ContentType = "text/plain";
            result.ContentEncoding = System.Text.Encoding.UTF8;
            result.Data = item.Handle(entity).ResponseData.ToString();
            return result;
        }
        #endregion

        #region 仁跃
        public ActionResult RY(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.RYCallBack.RY_HandleData entity)
        {
            RYCallBack item = new RYCallBack();
            MyResult result = new MyResult();
            result.ContentType = "text/plain";
            result.ContentEncoding = System.Text.Encoding.UTF8;
            result.Data = item.Handle(entity).ResponseData.ToString();
            return result;
        }
        #endregion
        #region 仁跃
        public ActionResult CW(KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Lib.CWCallBack.CW_HandleData entity)
        {
            CWCallBack item = new CWCallBack();
            MyResult result = new MyResult();
            result.ContentType = "text/plain";
            result.ContentEncoding = System.Text.Encoding.UTF8;
            result.Data = item.Handle(entity).ResponseData.ToString();
            return result;
        }
        #endregion

        

        public string Test()
        {
            Thread.Sleep(1000);
            return "ok";
        }

        #region 获取Http请求内容

        /// <summary>
        /// 获取Http请求内容
        /// <para>仅能使用Web环境</para>
        /// <para>此方法内部使用InputStream获取内容，所以此方法仅能使用一次</para>
        /// <para>请注意：因为http的标准性，此方法内部使用了InputStream，外部再次使用，则获取不到数据</para>
        /// </summary>
        /// <returns></returns>
        public static String GetHttpData(string encod = "utf-8")
        {
            try
            {
                string http_data = string.Empty;
                System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;
                if (request.HttpMethod.ToLower() == "post")
                {
                    Stream resStream = System.Web.HttpContext.Current.Request.InputStream;
                    resStream.Position = 0;
                    resStream.Seek(0, SeekOrigin.Begin);
                    StreamReader streamReader = new StreamReader(resStream, Encoding.GetEncoding(encod));
                    http_data = streamReader.ReadToEnd();
                    //Stream reqStream = System.Web.HttpContext.Current.Request.InputStream;
                    //reqStream.Position = 0;
                    //reqStream.Seek(0, SeekOrigin.Begin);
                    //byte[] buffer = new byte[(int)reqStream.Length];
                    //reqStream.Read(buffer, 0, (int)reqStream.Length);
                    //foreach (char a in buffer)
                    //{
                    //    http_data = http_data + a.ToString();
                    //}
                }
                else
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    System.Collections.Specialized.NameValueCollection qu = request.QueryString;
                    foreach (string key in qu)
                    {
                        if (key != null)
                        {
                            dic.Add(key, qu[key]);
                        }
                    }
                    http_data = DictionaryToString(dic);
                }
                return http_data.Trim();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        ///  键值集合以某种格式组合成字符串（排序）
        /// </summary>
        /// <param name="dic">键值集合</param>
        /// <param name="code">排序方式（0:默认,1:升序,2:降序）</param>
        /// <param name="connect">键和对应值的连接格式</param>
        /// <param name="spar">与相邻键值连接格式</param>
        /// <returns></returns>
        public static string DictionaryToString(Dictionary<string, string> dic, int code = 0, string connect = "=", string spar = "&", bool isIgnoreCase = false)
        {
            List<string> list_key = new List<string>();
            List<string> new_str = new List<string>();
            System.Collections.ArrayList arr_list = new System.Collections.ArrayList();
            arr_list.AddRange(dic.Keys);
            foreach (string item in dic.Keys)
            {
                list_key.Add(item);//默认顺序
            }
            if (code == 0)
            {
                new_str = list_key;
            }
            else
                if (code == 1 || code == 2)
            {
                List<string> li = new List<string>();
                if (isIgnoreCase)
                {
                    arr_list.Sort(StringComparer.Ordinal);
                }
                else
                {
                    arr_list.Sort();
                }
                if (code == 1)
                {
                    for (int i = 0; i < arr_list.Count; i++)
                    {
                        li.Add(arr_list[i].ToString());
                        Console.WriteLine(arr_list[i] + "：" + i);
                    }
                }
                else if (code == 2)
                {
                    for (int i = arr_list.Count - 1; i >= 0; i--)
                    {
                        li.Add(arr_list[i].ToString());
                        Console.WriteLine(arr_list[i] + "：" + i);
                    }
                }
                foreach (string item in li)
                {
                    new_str.Add(item);
                }
            }
            //将排序后组成字符串
            StringBuilder sb = new StringBuilder();
            string value = "";
            string key = "";
            //排序后的字符组合
            foreach (string item in new_str)
            {
                key = item;
                value = Convert.ToString(dic[key]);
                if (sb.ToString() != "")
                {
                    sb.Append(spar);
                }
                sb.Append(key + connect + value);
            }
            return sb.ToString();
        }
        #endregion



        public ActionResult Test2()
        {
            string http_data = string.Empty;
            System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;


            //Stream reqStream = System.Web.HttpContext.Current.Request.InputStream;
            //reqStream.Position = 0;
            //reqStream.Seek(0, SeekOrigin.Begin);
            //byte[] buffer = new byte[(int)reqStream.Length];
            //reqStream.Read(buffer, 0, (int)reqStream.Length);

            //string json = "";
            //foreach (char a in buffer)
            //{
            //    json = json + a.ToString();

            //}
            string f = GetHttpData();
            string g = GetHttpData();
            MultiBus.Log.Factory.LogInstance.Loger.Write(new LogEntity { ExtendType = $"【请求实体】", LogMsg = f });
            return new JsonResult();
        }

    }

    #region 自定响应类型
    public class MyResult : ContentResult
    {
        public MyResult()
        {

        }
        public object Data { get; set; }
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            HttpResponseBase response = context.HttpContext.Response;
            if (!String.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = this.ContentType;
            }
            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }
            if (this.Data != null)
            {
                response.Write(this.Data.ToString());
            }
            base.ExecuteResult(context);
        }
    }
    #endregion
}