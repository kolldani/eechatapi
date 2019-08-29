using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using EEChatService.Business.Exceptions;
using EEChatService.Data.Models;
using NLog;

namespace EEChatService.Business.Service
{
    public class ChatService : IChatService
    {
        private readonly string ConnectionString = string.Empty;
        private readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public ChatService(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public Guid CreateChat(string screenName)
        {
            var chat = new Chat()
            {
                UserScreenName = screenName,
            };

            try
            {
                using (var context = new EEChatDataContext(ConnectionString))
                {
                    // Before create the chat we ensure that the given screen name is not in use.
                    if (context.Chats.Any(c => c.UserScreenName == screenName))
                        throw new ScreenNameInUseException($"The following screen name is already in use: {screenName}");

                    context.Chats.Add(chat);

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(CreateChat)} threw an exception: {e.Message}");
                throw e;
            }

            return chat.Id;
        }

        public void AddMessageToChat(Guid chatId, ChatMessage message)
        {
            try
            {
                using (var context = new EEChatDataContext(ConnectionString))
                {
                    var chat = context.Chats.Find(chatId);
                    // If we don't find the chat by ID throw exception.
                    if (chat == null)
                        throw new ChatNotFoundException($"The chat wihh ID ${chatId} cannot be found.");

                    message.ChatId = chat.Id;
                    context.ChatMessages.Add(message);
                    // If chat is not active, yet, set it as we added a message to it.
                    if (chat.IsActive == false)
                    {
                        chat.IsActive = true;
                    }
                    chat.LastMessageCreatedDateTime = message.CreatedDate;
                    // If an operator sent the message the chat is regarded as answered, if an anonymous user, it's regarded as unanswered.
                    // So a chat may change unanswered to answered or vice versa depending on which type of user sent the message.
                    if (message.SenderType == UserType.Operator && chat.IsAnswered == false)
                        chat.IsAnswered = true;
                    else if (message.SenderType == UserType.AnonymousUser && chat.IsAnswered == true)
                        chat.IsAnswered = false;

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(AddMessageToChat)} threw an exception: {e.Message}");
                throw e;
            }
        }

        public IList<Chat> GetChatList()
        {
            List<Chat> chatList = new List<Chat>();

            try
            {
                using (var context = new EEChatDataContext(ConnectionString))
                {
                    chatList = context.Chats.
                        Where(c => (c.IsActive == true) && (c.IsAnswered == false))
                        .ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(GetChatList)} threw an exception: {e.Message}");
                throw e;
            }

            return chatList;
        }

        public Chat GetChat(Guid chatId, DateTime? from = null)
        {
            Chat chat = null;

            try
            {
                using (var context = new EEChatDataContext(ConnectionString))
                {
                    chat = context.Chats.Find(chatId);
                    // If we don't find the chat by ID throw exception.
                    if (chat == null)
                        throw new ChatNotFoundException($"The chat wihh ID ${chatId} cannot be found.");

                    var query = context.Entry(chat).Collection(c => c.Messages)
                                           .Query();

                    if (from != null) // Filter by CreatedDate
                        query.Where(m => m.CreatedDate > from)
                            .OrderBy(m => m.CreatedDate);
                    else
                        query.OrderBy(m => m.CreatedDate);
                    query.Load();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(GetChat)} threw an exception: {e.Message}");
                throw e;
            }

            return chat;
        }

        public string GetScreenName(Guid chatId)
        {
            string userScreenName = string.Empty;
            try
            {
                using (var context = new EEChatDataContext(ConnectionString))
                {
                    var chat = context.Chats.Find(chatId);
                    // If we don't find the chat by ID throw exception.
                    if (chat == null)
                        throw new ChatNotFoundException($"The chat wihh ID ${chatId} cannot be found.");

                    userScreenName = chat.UserScreenName;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(GetScreenName)} threw an exception: {e.Message}");
                throw e;
            }

            return userScreenName;
        }
    }
}
