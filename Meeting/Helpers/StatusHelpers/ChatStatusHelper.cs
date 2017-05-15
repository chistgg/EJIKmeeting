using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meeting.Helpers
{
    public static class ChatStatusHelper
    {
        public static bool IsProtocolInConfirming(string status)
        {
            return (status[3] == '1') ? true : false;
        }

        public static bool IsProtocolConfirmed(string status)
        {
            return (status[3] == '2') ? true : false;
        }
    }
}