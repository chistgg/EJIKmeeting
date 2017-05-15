using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Meeting.Models.Repository
{
    public static class FileRepository
    {
        public static File GetFileByID(Guid fileID, MeetingContainer model)
        {
            return (from f in model.FileSet
                    where f.ID == fileID
                    select f).First();
        }

        public static File GetFileByHashedID(string hashedID, MeetingContainer model)
        {
            try
            {
                var files = model.FileSet;

                foreach (var f in files)
                {
                    if (Crypto.VerifyHashedPassword(hashedID, f.ID.ToString()))
                        return f;
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}