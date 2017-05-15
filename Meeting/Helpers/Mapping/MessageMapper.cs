using AutoMapper;
using Meeting.Models;
using Meeting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meeting.Helpers.Mapping
{
    public class MessageMapper : ICustomMapper
    {
        public MessageMapper()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Message, MessageProtocolView>());//.ForMember("InProtocol", opt => opt.MapFrom(src => (src.Type % 10) == 0 ? false : true)));
            Mapper.Initialize(cfg => cfg.CreateMap<MessageProtocolView, Message>());//.ForMember("Type", opt => opt.MapFrom(src => src.InProtocol == true ? 1 : 0))); 

        }
        public object Map(object source, Type sourceType, Type destinationType)
        {
            return Mapper.Map(source, sourceType, destinationType);
        }

        public object MapCollection(object source, Type sourceType, Type destinationType)
        {
            return null;
           // return Mapper.Map<IEnumerable<>, List<MessageProtocolView>>(messages);
        }

        public List<MessageProtocolView> MessageMap(IEnumerable<Message> messages)
        {

            return Mapper.Map<IEnumerable<Message>, List<MessageProtocolView>>(messages);
        }
    }
}