using Meeting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meeting.Protocol
{
    public interface IProtocolGenerate
    {
        void InitializeParam(Guid meetingID, string outFilePath);
        void GenerateProtocol();
    }
}
