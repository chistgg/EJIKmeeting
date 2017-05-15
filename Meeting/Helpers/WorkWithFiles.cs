using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Meeting.Helpers
{
    public static class WorkWithFiles
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static string AllowedFileTypes = HttpContext.Current.Request.MapPath(@"\Resources\XML") + @"\AllowedTypes.xml";
        public static string GetFileExtentsion(string filePath)
        {
            var fileExtension = System.IO.Path.GetExtension(filePath).ToLower();
            try
            {
                XDocument xDoc = XDocument.Load(AllowedFileTypes);
                var xTypes = xDoc.Element("types").Elements();
                foreach (var type in xTypes)
                {       
                    string ext = type.Value;
                    if (fileExtension.Equals(ext))
                        return ext;
                }

                return "";
            }
            catch (Exception ex)
            {
                logger.Error("В методе IsAllowFile возникло исключение: " + ex.ToString());
                return "";
            }
        }


        public static Guid UploadFile(HttpPostedFileBase upload, string path)
        {

            if (upload != null && upload.ContentLength < 104857600)
            {
                var fileID = Guid.NewGuid();
                var fileExt = GetFileExtentsion(upload.FileName);
                if (fileExt == "")
                    return Guid.Empty;

                // string fileName = (upload.FileName.GetHashCode() + DateTime.Now.GetHashCode()).ToString();
                string url = path + fileID.ToString() ;
              
                upload.SaveAs(url);

                return fileID;
            }
            return Guid.Empty;

        }

        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public static void CreateDirectoryIfNotExists(string path)
        {

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }


    }
}