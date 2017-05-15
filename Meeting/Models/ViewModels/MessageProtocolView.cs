using Meeting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meeting.Models.ViewModels
{
    public class MessageProtocolView
    {
        public System.Guid ID { get; set; }
        public string Content { get; set; }
        public DateTime SendingTime { get; set; }
        public bool InProtocol { get; set; }
        public File File { get; set; }
        public User User { get; set; }


        //TODO перевести на automapper
        public MessageProtocolView(Message message)
        {
            ID = message.ID;
            Content = message.Content;
            SendingTime = message.SendingTime;
            File = message.File;
            User = message.User;
            InProtocol = MessageStatusHelper.IsMessageInProtocol(message.Status);
        }
    }
}