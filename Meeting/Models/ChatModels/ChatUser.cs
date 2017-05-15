using Meeting.Models.Repository;
using System;

namespace Meeting.Models.ChatModels
{
    public class ChatUser
    {


        public string ConnectionID { get; set; }
        public Guid UserID { get; set; }
        public Guid ChatID { get; set; }
        public string Status {get; set;}
        public string Nick { get; set; }

        public ChatUser(string connectionID, Guid userID)
        {
            MeetingContainer Model = new MeetingContainer();
            var user = UserRepository.GetUserByID(userID, Model);

            ConnectionID = connectionID;
            UserID = userID;
            ChatID = user.Chat.ID;
            Status = user.Status;
            Nick = user.Nick;
        }    
    }
}