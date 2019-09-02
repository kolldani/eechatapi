using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EEChatService.Controllers;
using EEChatService.Models.DTO;
using EEChatService.Business.Service;
using EEChatService.Data.Models;

namespace EEChatService.Tests.Controllers
{
    [TestClass]
    public class ChatsControllerIntegrationTests
    {
        private static string ConnectionString = string.Empty;
        private static IChatService ChatService;


        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EEChatDbConnection"].ConnectionString;
            ChatService = new ChatService(ConnectionString);
        }


        [TestMethod]
        public void ChatsControllerTest_CreateChat_HTTP200()
        {
            // Arrange
            var controller = new ChatsController(ChatService);
            string uniqueUserScreenName = "unique user screen name" + Guid.NewGuid().ToString();

            // act
            IHttpActionResult result = controller.CreateChat(uniqueUserScreenName);
            var createdResult = result as CreatedNegotiatedContentResult<ChatDTO>;

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(new Uri($"/api/chats/{createdResult.Content.Id}", UriKind.Relative), createdResult.Location);
            Assert.AreEqual(uniqueUserScreenName, createdResult.Content.UserScreenName);
            Assert.IsTrue(DateTime.Now.Subtract(createdResult.Content.CreatedDateTime).TotalMilliseconds <= 5000);

            // Db test
            //Test data is persisted into database as expected.
            var expected = new Chat()
            {
                Id = createdResult.Content.Id,
                UserScreenName = createdResult.Content.UserScreenName,
            };

            Chat actual = null;
            using (var context = new EEChatDataContext(ConnectionString))
            {
                actual = context.Chats.Find(createdResult.Content.Id);

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected, actual);
                context.Chats.Remove(actual);
            }
        }

        [TestMethod]
        public void ChatsControllerTest_CreateChat_HTTP409_conflict()
        {
            // Arrange
            var controller = new ChatsController(ChatService);
            string uniqueUserScreenName = "unique user screen name" + Guid.NewGuid().ToString();
            Chat chat = null;

            using (var context = new EEChatDataContext(ConnectionString))
            {
                // Create a chat
                chat = new Chat()
                {
                    UserScreenName = uniqueUserScreenName,
                };

                context.Chats.Add(chat);
                context.SaveChanges();

                // act
                // We pass controller action the same user screen name we with which created a chat above.
                IHttpActionResult result = controller.CreateChat(uniqueUserScreenName);
                var conflictResult = result as NegotiatedContentResult<string>;

                // Assert
                Assert.IsNotNull(conflictResult);
                Assert.AreEqual(HttpStatusCode.Conflict, conflictResult.StatusCode);

                context.Chats.Remove(chat);
            }
        }

        [TestMethod]
        public void ChatsControllerTest_CreateChat_HTTP400_bad_request()
        {
            // Arrange
            var controller = new ChatsController(ChatService);

            // act
            // We pass controller action a null as userScreenName parameter to get a HTTP 400 bad request.
            IHttpActionResult result = controller.CreateChat(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ChatsControllerTest_SendMessageIntoChat_HTTP200()
        {
            // Arrange
            var controller = new ChatsController(ChatService);
            string testMessage = "Test message text";
            var messageDispatch = new ChatMessageDispatch()
            {
                MessageText = testMessage,
                SentDate = DateTime.Now,
            };

            string uniqueUserScreenName = "unique user screen name" + Guid.NewGuid().ToString();
            Chat chat = null;

            using (var context = new EEChatDataContext(ConnectionString))
            {
                // Create a chat
                chat = new Chat()
                {
                    UserScreenName = uniqueUserScreenName,
                };

                context.Chats.Add(chat);
                context.SaveChanges();
            }

            // act
            IHttpActionResult result = controller.SendMessageIntoChat(chat.Id, messageDispatch);
            var okResult = result as OkNegotiatedContentResult<ChatMessageDTO>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(messageDispatch.MessageText, okResult.Content.MessageText);
            Assert.AreEqual(messageDispatch.SentDate, okResult.Content.SentDate);
            Assert.IsTrue(DateTime.Now.Subtract(okResult.Content.CreatedDate).TotalMilliseconds <= 5000);

            // Db test
            //Test data is persisted into database as expected.
            var expected = new ChatMessage()
            {
                Id = okResult.Content.Id,
                MessageText = okResult.Content.MessageText,
                ChatId = chat.Id,
            };
            using (var context = new EEChatDataContext(ConnectionString))
            {
                var actual = context.ChatMessages.Find(okResult.Content.Id);

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected, actual);
                // Test whether related chat has become active.
                var chatRelated = context.Chats.Find(actual.ChatId);
                Assert.IsNotNull(chatRelated);
                Assert.IsTrue(chatRelated.IsActive);

                context.Chats.Remove(chatRelated);
            }
        }

        [TestMethod]
        public void ChatsControllerTest_SendMessageIntoChat_HTTP404()
        {
            // Arrange
            var controller = new ChatsController(ChatService);
            string testMessage = "Test message text";
            var messageDispatch = new ChatMessageDispatch()
            {
                MessageText = testMessage,
                SentDate = DateTime.Now,
            };

            // act
            // We pass controller action a random chat ID, that we suppose not to be found.
            IHttpActionResult result = controller.SendMessageIntoChat(Guid.NewGuid(), messageDispatch);
            var notFoundResult = result as NegotiatedContentResult<string>;

            // Assert
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(HttpStatusCode.NotFound, notFoundResult.StatusCode);
        }

        [TestMethod]
        public void ChatsControllerTest_SendMessageIntoChat_HTTP409_bad_request()
        {
            // Arrange
            var controller = new ChatsController(ChatService);
            string testMessage = null;
            var messageDispatch = new ChatMessageDispatch()
            {
                MessageText = testMessage,
                SentDate = DateTime.Now,
            };

            // act
            // We pass controller action an invalid model.
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
            controller.Validate(messageDispatch);
            IHttpActionResult result = controller.SendMessageIntoChat(Guid.NewGuid(), messageDispatch);

            // Assert
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }
    }
}
