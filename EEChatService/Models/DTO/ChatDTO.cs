using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EEChatService.Models.DTO
{
    public class ChatDTO
    {
        public Guid Id { get; set; }
        public string UserScreenName { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastMessageCreatedDateTime { get; set; }


        public ChatDTO() { }
    }
}