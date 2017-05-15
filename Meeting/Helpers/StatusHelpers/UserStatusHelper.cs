using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meeting.Helpers
{
    public static class UserStatusHelper
    {
        public static bool IsAdmin(string status)
        {
            return (status[0] == '2') ? true : false;
        }
        public static bool IsConfirmProtocol(string status)
        {
            return (status[3] == '1') ? true : false;
        }

        public static bool IsRejectProtocol(string status)
        {
            return (status[3] == '2') ? true : false;
        }

        public static bool DidVoteForProtocol(string status)
        {
            return (status[3] != '0') ? true : false;
        }

        public static bool HasSendedMessages(string status)
        {
            return (status[1] == '2') ? true : false;
        }

        public static bool WasInChat(string status)
        {
            return (status[1] != '0') ? true : false;
        }

        public static byte GetProtocolConfirmatin(string status)
        {
            return byte.Parse(status[3].ToString());
        }

        public static string GetProtocolConfirmatinString(string status)
        {
            switch (status[3])
            {
                case '1':
                    return "Утвердил протокол";
                case '2':
                    return "Отклонил протокол";
                case '0':
                default:
                    return " ";
            }
        }
    }
}