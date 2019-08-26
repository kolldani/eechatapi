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
                Logger.Error($"{nameof(CreateChat)} threw an exception: {e.Message}");
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
                    if (chat.IsActive == false)
                    {
                        chat.IsActive = true;
                    }
                    chat.LastMessageCreatedDateTime = message.CreatedDate;
                    if (message.SenderType == UserType.Operator)
                        chat.IsAnswered = true;

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{nameof(AddMessageToChat)} threw an exception: {e.Message}");
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
                    chatList = context.Chats.Where(c => c.IsActive == true).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{nameof(GetChatList)} threw an exception: {e.Message}");
                throw e;
            }

            return chatList;
        }

        public IList<ChatMessage> GetChatMessages(Guid chatId, DateTime? from = null)
        {
            List<ChatMessage> chatMessages = new List<ChatMessage>();

            try
            {
                using (var context = new EEChatDataContext(ConnectionString))
                {
                    var chat = context.Chats.Find(chatId);
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

                    chatMessages = (List<ChatMessage>)chat.Messages;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{nameof(GetChatMessages)} threw an exception: {e.Message}");
                throw e;
            }

            return chatMessages;
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
                Logger.Error($"{nameof(GetScreenName)} threw an exception: {e.Message}");
                throw e;
            }

            return userScreenName;
        }
    }
}
