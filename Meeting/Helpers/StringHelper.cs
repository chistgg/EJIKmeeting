using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Xml.Linq;

namespace Meeting.Helpers
{
    public static class StringHelper
    {
        private static string UrlFile = HttpContext.Current.Request.MapPath(@"\Resources\XML") + @"\Url.xml";

        public static Guid StrToGuidOrEmptGuid(string s)
        {
            Guid g = Guid.Empty;

            Guid.TryParse(s, out g);

            return g;
        }

        public static string ReplaceCharInString(string str, int index, char newSymb)
        {
            return str.Remove(index, 1).Insert(index, newSymb.ToString());
        }

        public static string ReplaceCharInString(ref string str, int index, char newSymb)
        {
            return str.Remove(index, 1).Insert(index, newSymb.ToString());
        }

        public static string DateTimeToString(DateTime dt)
        {
            return dt.ToShortDateString() + "  " + dt.ToShortTimeString();
        }

        public static string GetFileDirectoryForChat(Guid chatID)
        {
            return GetFullApplicationPath + @"/Content/" + chatID.ToString() + "/";
        }

        static public string GetFullApplicationPath
        {
            get
            {
                //Return variable declaration
                var appPath = string.Empty;

                //Getting the current context of HTTP request
                var context = HttpContext.Current;

                //Checking the current context content
                if (context != null)
                {
                    //Formatting the fully qualified website url/name
                    appPath = string.Format("{0}://{1}{2}{3}",
                                            context.Request.Url.Scheme,
                                            context.Request.Url.Host,
                                            context.Request.Url.Port == 80
                                                ? string.Empty
                                                : ":" + context.Request.Url.Port,
                                            context.Request.ApplicationPath);
                }

                if (!appPath.EndsWith("/"))
                    appPath += "/";

                return appPath;
            }
        }

        public static string GetHashedString(string source)
        {
            return Crypto.HashPassword(source);
        }

        public static string GetCurrentHost()
        {
            try
            {
                XDocument xDoc = XDocument.Load(UrlFile);
                var xLinks = xDoc.Element("links").Elements();
                foreach (var link in xLinks)
                {
                    string hostName = link.Attribute("name").Value;
                    if (hostName == "ejmeeting")
                        return link.Value;
                }

                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string GetPublicDownloadURL(Guid fileID)
        {
            return GetCurrentHost() + "/Base/Download?Lnk=" + HttpUtility.UrlEncode(GetHashedString(fileID.ToString()));
        }
    }
}