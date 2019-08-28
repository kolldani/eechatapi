using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EEChatService.Models.DTO
{
    public class ChatMessageDispatch
    {
        [Required]
        public string MessageText { get; set; }

        public DateTime SentDate { get; set; }
    }
}