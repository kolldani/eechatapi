using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EEChatService.Data.Models
{
    public class ChatMessage : IEquatable<ChatMessage>
    {
        /// <summary>
        /// Unique identifier of a chat message.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        ///  Screen name of sender.
        ///  In case of an authenticated user it comes from his/her ScreenName property, in case of an anonymous user it comes from the Chat.UserScreenName property.
        /// </summary>
        [Required, StringLength(100)]
        public string SenderName { get; set; } = string.Empty;

/// <summary>
/// The type of sender: anonymous user or an authenticated operator of EE inc.
/// </summary>
        public UserType SenderType { get; set; }

        /// <summary>
        ///  The date and time when client sent the chat message.
        /// </summary>
        public DateTime SentDate { get; set; }

        /// <summary>
        /// The text of a chat message.
        /// </summary>
        [Required]
                public string MessageText { get; set; } = string.Empty;

        /// <summary>
        ///  The date and time when server created the message.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        ///  Id of the chat that this message is related to.
        /// </summary>
        public Guid ChatId { get; set; }

        /// <summary>
        ///  The Chat object that this message is related to.
        /// </summary>
        public Chat Chat { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChatMessage);
        }

        public bool Equals(ChatMessage other)
        {
            if (object.ReferenceEquals(this, other))
                return true;

            return other != null &&
                   Id.Equals(other.Id) &&
                   MessageText == other.MessageText &&
                   ChatId.Equals(other.ChatId);
        }

        public override int GetHashCode()
        {
            var hashCode = -1391527914;
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MessageText);
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(ChatId);
            return hashCode;
        }
    }
}

public enum UserType
{
    AnonymousUser,
    Operator
}