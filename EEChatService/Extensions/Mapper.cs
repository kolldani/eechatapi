using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EEChatService.Models.DTO;
using EEChatService.Data.Models;

namespace EEChatService.Extensions
{
    /// <summary>
    /// Extension methods for map between DTOs and model classes
    /// </summary>
    public static class Mapper
    {
        public static ChatMessage MapToChatMessage(this ChatMessageDispatch messageDispatch)
        {
            return new ChatMessage()
            {
                MessageText = messageDispatch.MessageText,
                SentDate = messageDispatch.SentDate,
            };
        }

        public static ChatDTO MapToChatDTO(this Chat chat)
        {
            return new ChatDTO()
            {
                Id = chat.Id,
                UserScreenName = chat.UserScreenName,
                CreatedDateTime = chat.CreatedDateTime,
                LastMessageCreatedDateTime = chat.LastMessageCreatedDateTime ?? DateTime.MinValue,
            };
        }

        public static ChatWithMessagesDTO MapToChatWithMessagesDTO(this Chat chat)
        {
            var chatWithMessagesDTO = new ChatWithMessagesDTO()
            {
                Id = chat.Id,
                UserScreenName = chat.UserScreenName,
                CreatedDateTime = chat.CreatedDateTime,
                LastMessageCreatedDateTime = chat.LastMessageCreatedDateTime ?? DateTime.MinValue,
            };

            foreach (var message in chat.Messages)
            {
                chatWithMessagesDTO.Messages.Add(message.MapToChatMessageDTO());
            }

            return chatWithMessagesDTO;
        }


        private static ChatMessageDTO MapToChatMessageDTO(this ChatMessage message)
        {
            return new ChatMessageDTO()
            {
                Id = message.Id,
                SenderName = message.SenderName,
                SentDate = message.SentDate,
                MessageText = message.MessageText,
                CreatedDate = message.CreatedDate,
            };
        }
    }
}