using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class Message
    {
        /// <summary>
        /// 响应状态码
        /// </summary>
        public int status { get; set; }
        
        /// <summary>
        /// 响应消息
        /// </summary>
        public string msg { get; set; }
        
        /// <summary>
        /// 响应数据集
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 服务器时间
        /// </summary>
        public DateTime updateTime { get; set; }
    }
}
