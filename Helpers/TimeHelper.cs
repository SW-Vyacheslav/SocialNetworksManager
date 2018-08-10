using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class TimeHelper
    {
        public static DateTime UnixTimeStampToDateTime(Int64 timestamp)
        {
            DateTime time = new DateTime(1970,1,1,0,0,0,0,DateTimeKind.Utc);
            return time.AddSeconds(timestamp).ToLocalTime();
        }
    }
}