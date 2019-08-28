using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EEChatService.Data.Models;

namespace EEChatService.Business.Service
{
    public interface IChatService
    {
        /// <summary>
        /// Creates a chat.
        /// </summary>
        /// <param name="screenName">An unique screen name for anonymous user ordered to the created chat, that will identify the user for operators.</param>
        /// <returns>The unique identifier of chat.</returns>
        Guid CreateChat(string screenName);

        /// <summary>
        /// Adds a new message to the specified chat.
        /// </summary>
        /// <param name="chatId">Specifies a chat that the message wil be added to.</param>
        /// <param name="message">A ChatMessage object that contains the new message.</param>
        void AddMessageToChat(Guid chatId, ChatMessage message);

        /// <summary>
        /// Gets list of chats which are active (contain messages) and unanswered (the last message sent by an anonymous user).
        /// </summary>
        /// <returns>A list of Chat objects containing details of each chat.</returns>
        IList<Chat> GetChatList();

        /// <summary>
        /// Gets a chat with its related messages for a specified chat identifier.
        /// </summary>
        /// <param name="chatId">Id of a chat.</param>
        /// <param name="from">Optional. If specified related messages will be filtered. The creation date of related messages will be greater than ghe given from DateTime.</param>
        /// <returns>A Chat object containing related ChatMessage objects.</returns>
        Chat GetChat(Guid chatId, DateTime? from = null);

        /// <summary>
        /// Gets anonymous user's screenname ordered for a chat.
        /// </summary>
        /// <param name="chatId">Identifies a chat.</param>
        /// <returns>User's screen name.</returns>
        string GetScreenName(Guid chatId);
    }
}
