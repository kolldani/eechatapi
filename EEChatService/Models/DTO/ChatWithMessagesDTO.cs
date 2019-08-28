using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EEChatService.Models.DTO
{
    public class ChatWithMessagesDTO : ChatDTO
    {
        public IList<ChatMessageDTO> Messages { get; set; }


        public ChatWithMessagesDTO() : base()
        {
            Messages = new List<ChatMessageDTO>();
        }
    }
}