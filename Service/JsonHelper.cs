using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace Service
{
    /// <summary>
    /// JSON辅助类
    /// </summary>
    public class JsonHelper
    {
        private HttpContext context;

        public JsonHelper(HttpContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 处理成功
        /// </summary>
        public void Success(string msg,object data=null) 
        {
            Message message = new Message();

            message.status = 0;
            message.msg = msg;
            message.data = data == null ? new Array[0] : data;
            message.updateTime = DateTime.Now;
            context.Response.Write(SerializeObject(message));
        }

        /// <summary>
        /// 处理失败
        /// </summary>
        public void Failure(string msg, object data = null)
        {
            Message message = new Message();

            message.status = -1;
            message.msg = msg;
            message.data = data == null?new Array[0]:data;
            message.updateTime = DateTime.Now;
            context.Response.Write(SerializeObject(message));
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public string SerializeObject(object obj) 
        {
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, timeFormat);
        }
        
    }
}
