using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Demo.WebAPI.Infrastructure.Security
{
    public class SecurityConfig
    {
        [JsonPropertyName("EnbableAPISecurity")]
        public bool EnbableAPISecurity { get; set; }

        public byte[] Aes_Key { get; set; }

        public byte[] Aes_Iv { get; set; }

        public List<ActionToIgnoreSecurity> ActionToIgnoreSecurity { get; set; }

        public bool ActionDontNeedSecure(string Action,string Controller)
        {
            if(string.IsNullOrEmpty(Action) || string.IsNullOrEmpty(Controller))
            {
                return true;
            }
            if (!ActionToIgnoreSecurity.Contains(new ActionToIgnoreSecurity { Action = Action, Controller = Controller }))
            {
                return false;
            }
            return true;
        }
    }

    public class ActionToIgnoreSecurity
    {
        public string Action { get; set; }

        public string Controller { get; set; }
    }
}
