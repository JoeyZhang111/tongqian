using MultiBus.ILog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Filter
{
    /// <summary>
    /// 异常过滤器
    /// </summary>
    public class ProgramExceptionFilter : HandleErrorAttribute, IExceptionFilter
    {
        public override void OnException(ExceptionContext filterContext)
        {
            string controlName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            Exception exception = filterContext.Exception;
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            HttpException http = new HttpException(null, exception);
            string Message = filterContext.Exception.Message;
            var resultData= new { ErrorMsg = "系统异常，请人工核实" }; 
            MultiBus.Log.Factory.LogInstance.Loger.Write(new LogEntity { ExtendType = $"【异常：{controlName}/{actionName}】", LogMsg = $"异常原因：{Message},异常详情：{ filterContext.Exception.StackTrace},异常响应：{new JavaScriptSerializer().Serialize(resultData)}" });
            var jsonResult = filterContext.Result as JsonResult;
            if (jsonResult == null)
            {
                jsonResult = new JsonResult();
                jsonResult.Data = resultData;
                filterContext.Result = jsonResult;
            }
            /* 
             * 设置自定义异常已经处理,避免其他过滤器异常覆盖
              */
            filterContext.ExceptionHandled = true;
            /* 
             * 在派生类重写时,设置或者重写一个值该值指定是否禁用ISS7.0中自定义错误
              */
            //filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}