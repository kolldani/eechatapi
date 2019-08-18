using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EEChatService.Data.Models
{
    /// <summary>
    /// This class represents a chat in our data model.
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// The unique identifier of a chat.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// A human readable screen name for the user who started the chat.
        /// As users are anonymous we identify them via this screen name.
        /// It must be unique.
        /// </summary>
        [Required, StringLength(100)]
        public string UserScreenName { get; set; } = string.Empty;

        /// <summary>
        /// The date and time when chat was created.
        /// </summary>
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Shows whether a chat is active or not.
        /// A chat becomes active when least one message is already added to it. 
        /// </summary>
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Messages contained by our chat.
        /// </summary>
        public IList<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
