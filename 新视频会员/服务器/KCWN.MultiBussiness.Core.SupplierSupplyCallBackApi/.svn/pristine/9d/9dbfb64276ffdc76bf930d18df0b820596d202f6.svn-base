using MultiBus.ILog.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Filter
{
    public class ProgramResultFilter : IResultFilter
    {
        private const string Key = "action";
        private bool _IsDebugLog = true;
        private Guid? gid;
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {

        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            string controlName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string item = "结果返回过滤器";
            var f = filterContext.RequestContext.HttpContext.Response;

            var request = System.Web.HttpContext.Current.Request;
            var keys = request.Form.AllKeys;

            //filterContext.HttpContext.Request.ge
            //using (HttpWebResponse response = (HttpWebResponse)System.Web.HttpContext.Current.Request.GetResponse())
            //{
            //    if (response.StatusCode == HttpStatusCode.OK)
            //    {
            //        Stream resStream = response.GetResponseStream();
            //        System.IO.StreamReader streamReader = new StreamReader(resStream, Encoding.GetEncoding(encodingName));
            //        ReturnString = streamReader.ReadToEnd();
            //        streamReader.Close();
            //        resStream.Close();
            //        httpObj.IsSuccess = true;
            //        httpObj.ResponseData = ReturnString;
            //        httpObj.LogMsg = string.Format("请求成功返回：({0})", ReturnString);
            //        endTime = DateTime.Now;
            //        httpObj.UseTime = (endTime - startTime).TotalSeconds;
            //        return httpObj;
            //    }
            //}

            //Stopwatch stopWatch = filterContext.Request.Properties[Key] as Stopwatch
            var c = filterContext.Result;
            //filterContext.Response.Content.ReadAsStringAsync().Result


            MultiBus.Log.Factory.LogInstance.Loger.Write(new LogEntity { ExtendType = "【结果返回过滤器】", LogMsg = $"执行方法：{actionName}，响应：{""}" });
        }
    }
}