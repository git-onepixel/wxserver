using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Service;
using System.Text;
using System.Web.Security;

namespace AlsServer.Handler
{
    /// <summary>
    /// 微信业务逻辑处理
    /// </summary>
    public class WxHandler : IHttpHandler
    {
        private WxService service = new WxService();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/json";

            var type = context.Request["type"];

            switch (type) 
            {
                case "share":
                    Share(context);
                    break;
                case "auth":
                    Auth(context);
                    break;
                case "openid":
                    OpenId(context);
                    break;
                default:
                    break;
            } 

            
        }

        /// <summary>
        /// 微信分享
        /// </summary>
        /// <param name="context"></param>
        private void Share(HttpContext context) 
        {
            JsonHelper jsonHelper = new JsonHelper(context);
            
            string url = context.Request["page_url"];
            Encoding utf8 = Encoding.UTF8;
            url = HttpUtility.UrlDecode(url, utf8);

            WxConfig config = service.GetShareConfig(url);

            jsonHelper.Success("查询成功", config);
        
        }

        /// <summary>
        /// 微信认证
        /// </summary>
        /// <param name="context"></param>
        private void Auth(HttpContext context)
        {
            string token = "alsgame";
            string echoStr = context.Request["echoStr"];
            string signature = context.Request["signature"];
            string timestamp = context.Request["timestamp"];
            string nonce = context.Request["nonce"];
            string[] ArrTmp = { token, timestamp, nonce };
            Array.Sort(ArrTmp);  //字典排序  
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();
            if (tmpStr == signature)
            {
                if (!string.IsNullOrEmpty(echoStr))
                {
                    context.Response.Write(echoStr);
                    context.Response.End();
                }
            }
        }

        /// <summary>
        /// 获取OpenId
        /// </summary>
        public void OpenId(HttpContext context)
        {
            JsonHelper jsonHelper = new JsonHelper(context);
            string code = context.Request["code"];
            
            var obj = service.GetOpenId(code);

            jsonHelper.Success("查询成功", obj);
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