using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EEChatService.Data.Models
{
    public class ChatMessage
    {
        /// <summary>
        /// Unique identifier of a chat message.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        ///  Id of the chat that this message is related to.
        /// </summary>
        public Guid ChatId { get; set; }

        /// <summary>
        ///  The Chat object that this message is related to.
        /// </summary>
        public Chat Chat { get; set; }

        /// <summary>
        /// The text of a chat message.
        /// </summary>
        public string MessageText { get; set; } = string.Empty;

        /// <summary>
        ///  The date and time when client sent the chat message.
        /// </summary>
        public DateTime SentDate { get; set; }

        /// <summary>
        ///  The date and time when server created the message.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
