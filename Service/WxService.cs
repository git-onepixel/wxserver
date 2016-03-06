using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Web.Security;
using System.Configuration;

namespace Service
{
    /// <summary>
    /// 微信业务处理
    /// </summary>
    public class WxService
    {
        /// <summary>
        /// 微信公众号
        /// </summary>
        private static string appId = ConfigurationManager.AppSettings["APPID"].ToString();
        private static string secret = ConfigurationManager.AppSettings["SECRET"].ToString();

        /// <summary>
        /// 全局缓存
        /// </summary>
        private static string AccessToken = "";
        private static string JsApiTicket = "";

        /// <summary>
        /// 上次更新时间
        /// </summary>
        private static long LastAccessToken = 0;
        private static long LastJsApiTicket = 0;

        /// <summary>
        /// 获取OPENID
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetOpenId(string code)
        { 
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = String.Format(url,appId,secret,code);
            
            return HttpGet(url);
        }
        
        /// <summary>
        /// 获取微信配置信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public WxConfig GetShareConfig(string url)
        {
            string ticket =  GetJsApiTicket();
            string noncestr =  GetNonceStr();
            long timestamp =  GetTimeStamp();
            string signature = CreateSignature(ticket, noncestr, timestamp, url);

            WxConfig config = new WxConfig();
            config.appId = appId;
            config.ticket = ticket;
            config.nonceStr = noncestr;
            config.timestamp = timestamp;
            config.signature = signature;

            return config;
        }

        /// <summary>
        /// 生成签名算法
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string CreateSignature(string ticket,string nocestr,long timestamp,string url) 
        {
            string text = "jsapi_ticket=" + ticket;
            text += "&noncestr=" + nocestr;
            text += "&timestamp=" + timestamp;
            text += "&url=" + url;

            return SHA1(text); 
        }

        /// <summary>
        /// 获取Ticket
        /// </summary>
        /// <returns></returns>
        public string GetJsApiTicket()
        {
            if (GetTimeStamp() - LastJsApiTicket > 7000)
            {
                string accessToken = GetAccessToken();
                string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi";
                url = String.Format(url, accessToken);

                string result = HttpGet(url);
                WxConfig config = JsonConvert.DeserializeObject<WxConfig>(result);
                JsApiTicket = config.ticket;

                LastJsApiTicket = GetTimeStamp();
            }
            return JsApiTicket;
        }

        /// <summary>
        /// 获取Access_Token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public string GetAccessToken()
        {
            if (GetTimeStamp() - LastAccessToken > 7000)
            {
                string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
                url = String.Format(url, appId, secret);

                string result = HttpGet(url);
                WxConfig config = JsonConvert.DeserializeObject<WxConfig>(result);
                AccessToken = config.access_token;

                LastAccessToken = GetTimeStamp();
            }
            return AccessToken;
        }

        /// <summary>
        /// 获取TimeStamp
        /// </summary>
        /// <returns></returns>
        private long GetTimeStamp() 
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);  
        }

        /// <summary>
        /// 获取NoceStr
        /// </summary>
        /// <returns></returns>
        private string GetNonceStr() 
        {
            return System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Sha1加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string SHA1(string text)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(text, "SHA1"); 
        }

        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/json;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string result = streamReader.ReadToEnd();
            streamReader.Close();
            myResponseStream.Close();

            return result;
        }
        
    }
}
