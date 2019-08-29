using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using EEChatService.Models.DTO;
using EEChatService.Business.Service;
using EEChatService.Business.Exceptions;
using EEChatService.Data.Models;
using EEChatService.Extensions;
using NLog;

namespace EEChatService.Controllers
{
    [RoutePrefix("api/chats")]
    [Authorize]
    public class ChatsController : ApiController
    {
        private readonly IChatService ChatService;
        private readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        // ChatService is injected by Unity.
        public ChatsController(IChatService chatService)
        {
            ChatService = chatService;
        }

        /// <summary>
        /// Creates a chat.
        /// </summary>
        /// <param name="screenName">An unique screen name identifies the anonymous user in this chat.</param>
        /// <returns>The URI of the newly created chat.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(string))]
        public IHttpActionResult CreateChat([FromBody] string userScreenName)
        {
            if (string.IsNullOrEmpty(userScreenName))
                return BadRequest("'userScreenName' is a mandatory parameter.");

            Guid chatId = Guid.Empty;

            try
            {
                chatId = ChatService.CreateChat(userScreenName);
            }
            catch (ScreenNameInUseException e)
            {
                return Content(HttpStatusCode.Conflict, $"{e.Message}");
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(CreateChat)} threw an exception: {e.Message}");

                return InternalServerError();
            }

            return Created(new Uri($"/api/chats/{chatId.ToString()}", UriKind.Relative), $"The chat with ID '{chatId.ToString()}' is Created.");
        }

        /// <summary>
        /// Sends a message into a chat.
        /// </summary>
        /// <param name="id">Identifies the chat which the message will be added to.</param>
        /// <param name="model">Contains a new message.</param>
        [AllowAnonymous]
        [Route("{id}")]
        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult SendMessageIntoChat(Guid id, ChatMessageDispatch model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var chatMessage = model.MapToChatMessage();

            try
            {
                    if (User.Identity.IsAuthenticated) // User is an operator.
                {
                    chatMessage.SenderType = UserType.Operator;
                    var user = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
                    chatMessage.SenderName = user?.ScreenName;
                }
                else // User is anonymous
                {
                    chatMessage.SenderType = UserType.AnonymousUser;
                    chatMessage.SenderName = ChatService.GetScreenName(id);
                }
                // Add the message to our chat.
                ChatService.AddMessageToChat(id, chatMessage);
            }
            catch (ChatNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, $"{e.Message}");
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(SendMessageIntoChat)} threw an exception: {e.Message}");

                return InternalServerError();
            }

            return Ok();
        }

        /// <summary>
        /// Gets a list of chats, which are active (contain messages) and unanswered (last answer ffrom an anonymous user).
        /// </summary>
        /// <returns>A list of chats without their messages.</returns>
        [HttpGet]
        [ResponseType(typeof(IList<ChatDTO>))]
        public IHttpActionResult GetChatList()
        {
            IList<ChatDTO> chatDTOs = new List<ChatDTO>();

            try
            {
                var chats = ChatService.GetChatList();

                // Map model classes to DTOs
                foreach (var chat in chats)
                {
                    chatDTOs.Add(chat.MapToChatDTO());
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(GetChatList)} threw an exception: {e.Message}");

                return InternalServerError();
            }

            return Ok(chatDTOs);
        }

        /// <summary>
        /// Gets a chat with messages related to it.
        /// </summary>
        /// <param name="id">Identifies a chat.</param>
        /// <param name="from">Optional. If given, filters messages related to chat. CreatedDate of related messages will be greater than from DateTime.</param>
        /// <returns>A DTO containing information about the chat and its messages.</returns>
        [AllowAnonymous]
        [Route("{id}")]
        [HttpGet]
        [ResponseType(typeof(ChatWithMessagesDTO))]
        public IHttpActionResult GetChatWithMessages(Guid id, DateTime? from = null)
        {
            var chatWithMessagesDTO = new ChatWithMessagesDTO();

            try
            {
                var chat = ChatService.GetChat(id, from);

                // Map Chat to DTO.
                chatWithMessagesDTO = chat.MapToChatWithMessagesDTO();
            }
            catch (ChatNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, $"{e.Message}");
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(GetChatWithMessages)} threw an exception: {e.Message}");

                return InternalServerError();
            }

            return Ok(chatWithMessagesDTO);
        }
    }
}
