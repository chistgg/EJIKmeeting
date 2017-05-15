using Meeting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meeting.Authentication
{
    public interface IUserProvider
    {
        User User { get; set; }
    }
}