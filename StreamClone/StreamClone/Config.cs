using System;
using System.Collections.Generic;
using System.Text;

namespace StreamClone
{
    class Config
    {
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public string ClientID { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int CheckPeriod { get; set; }
    }
}
