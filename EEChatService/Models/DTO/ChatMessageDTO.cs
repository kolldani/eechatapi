using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EEChatService.Models.DTO
{
    public class ChatMessageDTO
    {
        public Guid Id { get; set; }
        public string SenderName { get; set; }
        public DateTime SentDate { get; set; }
        public string MessageText { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}