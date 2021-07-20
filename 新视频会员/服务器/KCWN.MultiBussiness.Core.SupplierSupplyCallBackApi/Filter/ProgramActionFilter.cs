using MultiBus.ILog.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Filter
{
    public class ProgramActionFilter : FilterAttribute, IActionFilter
    {
        private Guid? uid;
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            uid = Guid.NewGuid();
            string controlName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string ip = GetRealIP();
            string requestData = GetHttpData();
            string contentType = filterContext.RequestContext.HttpContext.Request.ContentType;
            string method = filterContext.RequestContext.HttpContext.Request.HttpMethod;
            MultiBus.Log.Factory.LogInstance.Loger.Write(new LogEntity { ExtendType = $"【{controlName}/{actionName}】(uid:{uid})", LogMsg = $"请求IP:{ip},ContentType：{contentType},请求方式：{method},请求数据：{requestData}" });
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string controlName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string resultStr= new JavaScriptSerializer().Serialize(filterContext.Result);
            ActionResultData resultObj = new JavaScriptSerializer().Deserialize<ActionResultData>(resultStr);
            string result = "";
            if (resultObj!=null&& resultObj.Data != null) {
                result= new JavaScriptSerializer().Serialize(resultObj.Data);  
            }
            MultiBus.Log.Factory.LogInstance.Loger.Write(new LogEntity { ExtendType = $"【{controlName}/{actionName}】(uid:{uid})", LogMsg = $"响应数据：{result}" });
        }

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
                    //Stream resStream = System.Web.HttpContext.Current.Request.InputStream;
                    //StreamReader streamReader = new StreamReader(resStream, Encoding.GetEncoding(encod));
                    //http_data = streamReader.ReadToEnd();
                    //resStream.Position = 0;
                    //streamReader.Close(); 如果释放了在控制器中再次读取数据流会为空
                    //resStream.Close();
                    Stream resStream = System.Web.HttpContext.Current.Request.InputStream;
                    resStream.Position = 0;
                    resStream.Seek(0, SeekOrigin.Begin);
                    StreamReader streamReader = new StreamReader(resStream, Encoding.GetEncoding(encod));
                    http_data = streamReader.ReadToEnd();

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

        #region 获取真实IP
        public static string GetRealIP()
        {
            string result = String.Empty;
            try
            {
                result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                //可能有代理   
                if (!string.IsNullOrWhiteSpace(result))
                {
                    //没有"." 肯定是非IP格式  
                    if (result.IndexOf(".") == -1)
                    {
                        result = null;
                    }
                    else
                    {
                        //有","，估计多个代理。取第一个不是内网的IP。  
                        if (result.IndexOf(",") != -1)
                        {
                            result = result.Replace(" ", string.Empty).Replace("\"", string.Empty);

                            string[] temparyip = result.Split(",;".ToCharArray());

                            if (temparyip != null && temparyip.Length > 0)
                            {
                                for (int i = 0; i < temparyip.Length; i++)
                                {
                                    //找到不是内网的地址  
                                    if (IsIPAddress(temparyip[i]) && temparyip[i].Substring(0, 3) != "10." && temparyip[i].Substring(0, 7) != "192.168" && temparyip[i].Substring(0, 7) != "172.16.")
                                    {
                                        return temparyip[i];
                                    }
                                }
                            }
                        }

                        else if (IsIPAddress(result)) //代理即是IP格式  
                        {
                            return result;
                        }

                        else //代理中的内容非IP  
                        {
                            result = null;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    result = System.Web.HttpContext.Current.Request.UserHostAddress;
                }

            }
            catch (Exception ex)
            {

                //写异常：
            }
            return result;

        }
        public static bool IsIPAddress(string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length < 7 || str.Length > 15)
                return false;

            string regformat = @"^(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);

            return regex.IsMatch(str);
        }
        #endregion

    }
}
public class ActionResultData
{
    public Object ContentEncoding { get; set; }
    public Object ContentType { get; set; }
    public Object Data { get; set; }
    public int? JsonRequestBehavior { get; set; }
    public Object MaxJsonLength { get; set; }
}