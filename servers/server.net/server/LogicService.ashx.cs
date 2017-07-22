using cornerstone.integrator.exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cornerstone.server
{
    /// <summary>
    /// Summary description for LogicService
    /// </summary>
    public class LogicService : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            String ResData = "";
            String PostData = "";
            try
            {
                cornerstone.integrator.Integrator.Init();
                PostData = System.Text.Encoding.UTF8.GetString(context.Request.BinaryRead(context.Request.ContentLength));
                if (PostData == "") return;
                ResData = cornerstone.integrator.Integrator.Process(PostData);
            }
            catch (System.Exception ex)
            {
                global::haxe.lang.Exceptions.exception = ex;
                object pos = new { className = "LogicService", methodName = "ProcessRequest", fileName = "LogicService.ashx", lineNumber = 0 };
                ResData = ExceptionManager.HandleException(global::Exception.wrap(ex, pos), PostData, null, null, null, null, null);
            }

            context.Response.ContentType = "text/plain";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.Write(ResData);
            context.Response.Flush();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}