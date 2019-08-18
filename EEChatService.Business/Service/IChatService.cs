using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EEChatService.Business.Model;
using EEChatService.Data.Models;

namespace EEChatService.Business.Service
{
    interface IChatService
    {
        /// <summary>
        /// Creates a chat.
        /// </summary>
        /// <param name="screenName"></param>
        /// An unique screen name for anonymous user ordered to the created chat, that will identify the user for operators.
        /// <returns>The unique identifier of chat.</returns>
        Guid CreateChat(string screenName);

        /// <summary>
        /// Adds a new message to the specified chat.
        /// </summary>
        /// <param name="chatId"></param>
        /// Specifies a chat that the message wil be added to.
        /// <param name="message"></param>
        /// The text of the new message.
        void AddMessageToChat(Guid chatId, string message);

        /// <summary>
        /// Gets list of chats which are active (contain messages) and unanswered (the last message sent by an anonymous user).
        /// </summary>
        /// <returns>A list of ChatDetails objects containing details of each chat.</returns>
        IList<ChatDetails> GetChatList();

        /// <summary>
        /// Gets a list of messages for a specified chat identifier.
        /// </summary>
        /// <param name="chatId"></param>
        /// Id of a chat.
        /// <param name="from"></param>
        /// Optional. If specified the creation date of returned messages will be greater than ghe given from value.
        /// <returns>A list of ChatMessage objects.</returns>
        IList<ChatMessage> GetChatMessages(Guid chatId, DateTime? from = null);
    }
}
