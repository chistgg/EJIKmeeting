using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meeting.Helpers.Mapping
{
   public interface ICustomMapper
    {
        object Map(object source, Type sourceType, Type destinationType);
    }
}
