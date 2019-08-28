using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EEChatService.Data.Models
{
    public class EEChatDbInitializer : DropCreateDatabaseIfModelChanges<EEChatDataContext>
    {
        protected override void Seed(EEChatDataContext context)
        {
            base.Seed(context);

            // Creating seed data
            var chat = new Chat()
            {
                UserScreenName = "Test User from seed data",
            };

            context.Chats.Add(chat);

            var chatMessages = new List<ChatMessage>(){
                new ChatMessage()
            {
                SenderName = "Anonymous user",
                SenderType = UserType.AnonymousUser,
                SentDate = DateTime.Now,
                                MessageText = "Some text.",
                ChatId = chat.Id,
            },
                new ChatMessage()
            {
                    SenderName = "Operator",
                    SenderType = UserType.AnonymousUser,
                    SentDate = DateTime.Now,
                MessageText = "Some other text.",
                ChatId = chat.Id,
        }
                };

            context.ChatMessages.AddRange(chatMessages);

            // Chat already has messages set it active.
            chat.IsActive = true;
            chat.LastMessageCreatedDateTime = chatMessages[chatMessages.Count - 1].CreatedDate;

            context.SaveChanges();
        }
    }
}
