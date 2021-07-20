using KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ProgramActionFilter());//方法过滤器
            filters.Add(new ProgramExceptionFilter());//异常过滤器
           // filters.Add(new ProgramResultFilter());//结果返回过滤器
            //filters.Add(new ProgramAuthorityFilter());

        }
    }
}