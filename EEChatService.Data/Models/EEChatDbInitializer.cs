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

            context.SaveChanges();

            var chatMessages = new List<ChatMessage>(){
                new ChatMessage()
            {
                MessageText = "Some text.",
                Chat = chat,
            },
                new ChatMessage()
            {
                MessageText = "Some other text.",
                Chat = chat,
        }
                };

            context.ChatMessages.AddRange(chatMessages);

            context.SaveChanges();


        }
    }
}
