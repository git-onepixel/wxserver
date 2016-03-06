using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class WxConfig
    {
        public string appId { get; set; }

        public string signature { get; set; }

        public string nonceStr { get; set; }

        public long timestamp { get; set; }

        public string ticket { get; set; }

        public string access_token { get; set; }

    }
}
