using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEChatService.Business.Exceptions
{
    public class ChatNotFoundException : ApplicationException
    {
        public ChatNotFoundException() : base()
        {
        }

        public ChatNotFoundException(string message) : base(message)
        {
        }
    }
}
