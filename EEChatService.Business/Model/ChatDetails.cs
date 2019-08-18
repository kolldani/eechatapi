using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEChatService.Business.Model
{
    /// <summary>
    /// This class represents details of a chat.
    /// Some detail comes from messages  related to a chat.
    /// </summary>
    public class ChatDetails
    {
        /// <summary>
        /// The unique identifier of a chat.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// A human readable screen name for the user who started the chat.
        /// As users are anonymous we identify them via this screen name.
        /// It must be unique.
        /// </summary>
        public string UserScreenName { get; set; } = string.Empty;

        /// <summary>
        /// The date and time when last message was added to this chat.
        /// Comes from ChatMessage.CreatedDateTime property.
        /// </summary>
        public DateTime LastMessageCreatedDateTime { get; set; }

        /// <summary>
        /// Shows whether a chat has  already been answered or not.
        /// It derives from ChatMessage.UserType property: if the last message in a chat was sent by an authenticated user (operator), the chat is regarded as answered, in other case as unanswered.
        /// </summary>
        public bool IsAnswered { get; set; }
    }
}
