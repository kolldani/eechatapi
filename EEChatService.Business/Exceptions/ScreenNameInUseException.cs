using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEChatService.Business.Exceptions
{
    public class ScreenNameInUseException : ApplicationException
    {
        public ScreenNameInUseException() : base()
        {
        }

        public ScreenNameInUseException(string message) : base(message)
        {
        }
    }
}
