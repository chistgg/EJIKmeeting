using AutoMapper;
using Meeting.Models;
using Meeting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meeting.Helpers.Mapping
{
    public class MessageMapProfile : Profile
    {
        protected override void Configure()
        {
            base.CreateMap<Message, MessageProtocolView>().ReverseMap();
        }
    }
}