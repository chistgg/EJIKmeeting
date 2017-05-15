using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meeting.Helpers
{
    public static class MessageStatusHelper
    {
        public static bool IsMessageInProtocol(string status)
        {
            return status[3] == '0' ? false : true;
        }
    }
}