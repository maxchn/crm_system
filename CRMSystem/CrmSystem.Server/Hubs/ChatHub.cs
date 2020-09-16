using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CrmSystem.Server.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("AllowSpecificOrigin")]
    public class ChatHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ChatHub(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Вход во все чаты в которых состоит текущий пользователь
        /// </summary>
        /// <param name="companyUrl"></param>
        /// <returns></returns>

        public async Task JoinToChats(int companyId)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            var company = await _unitOfWork.Company.FindById(companyId);

            if (user != null && company != null)
            {
                List<Chat> rawChats = (await _unitOfWork.Chat.Find(c => c.Company.CompanyId == company.CompanyId))?.ToList();
                List<Chat> chats = rawChats.FindAll(c => c.ChatParticipants.Exists(p => p.Id.Equals(user.Id)));

                if (chats != null)
                {
                    foreach (var chat in chats)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, chat.ChatId.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Метод для добавления участника(-ов) к чату
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="participants"></param>
        /// <returns></returns>

        public async Task AddParticipantTоChat(int chatId, List<ChatParticipant> participants)
        {
            bool result = true;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Chat.AddNewParticipant(chatId, participants);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                result = false;
            }

            await Clients.Caller.SendAsync("AddParticipantTоChatResult", result);
        }

        /// <summary>
        /// Отправка сообщения в чате
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="companyId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(int chatId, int companyId, string message)
        {
            await _unitOfWork.BeginTransactionAsync();
            _unitOfWork.EnabledDisabledAutoDetectChanges(false);

            var user = await _userManager.GetUserAsync(Context.User);

            if (user != null)
            {
                var chat = (await _unitOfWork.Chat.Find(c => c.ChatId == chatId && c.Company.CompanyId == companyId))?.FirstOrDefault();
                chat = chat.ChatParticipants.FirstOrDefault(p => p.Id.Equals(user.Id)) != null ? chat : null;

                if (chat != null)
                {
                    var chatMessage = new ChatMessage
                    {
                        Text = message,
                        DispatchTime = DateTime.Now,
                        OwnerId = user.Id
                    };


                    try
                    {
                        await _unitOfWork.ChatMessage.Insert(chatMessage);
                        await _unitOfWork.SaveChangesAsync();

                        int chatMessageParticipantId = 0;
                        foreach (var participant in chat.ChatParticipants)
                        {
                            ChatMessageParticipant chatMessageParticipant = new ChatMessageParticipant
                            {
                                MessageId = chatMessage.ChatMessageId,
                                ChatId = chatId,
                                Id = participant.Id
                            };

                            await _unitOfWork.ChatMessageParticipant.Insert(chatMessageParticipant);

                            if (participant.Id.Equals(user.Id))
                            {
                                chatMessageParticipantId = chatMessageParticipant.ChatMessageParticipantId;
                            }
                        }

                        await _unitOfWork.SaveChangesAsync();
                        _unitOfWork.Commit();

                        var fullMessage = new
                        {
                            id = chatMessageParticipantId,
                            chatId = chat.ChatId,
                            dateTime = chatMessage.DispatchTime,
                            text = message,
                            senderId = user.Id,
                            sender = $"{user.FirstName} {user.LastName} {user.Patronymic}"
                        };

                        await Clients.OthersInGroup(chat.ChatId.ToString()).SendAsync("ReceiveChatMessage", JsonConvert.SerializeObject(fullMessage));
                        await Clients.Caller.SendAsync("ReceiveChatMessageAsSender", JsonConvert.SerializeObject(fullMessage));
                    }
                    catch (Exception)
                    {
                        _unitOfWork.Rollback();
                    }
                }
            }

            _unitOfWork.EnabledDisabledAutoDetectChanges(true);
        }

        /// <summary>
        /// Загрузка сообщений чата
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task LoadChatMessages(int chatId)
        {
            var user = await _userManager.GetUserAsync(Context.User);

            if (user != null)
            {
                var messages = await _unitOfWork.ChatMessageParticipant.Find(t => t.Id.Equals(user.Id) && t.ChatId == chatId);

                if (messages != null)
                {
                    if (messages.Count() > 0)
                    {
                        foreach (var message in messages)
                        {
                            // Если ИД-шники совпадает то это сообщение принадлежит текущему пользователю
                            var fullMessage = new
                            {
                                id = message.ChatMessageParticipantId,
                                chatId = message.Chat.ChatId,
                                dateTime = message.Message.DispatchTime,
                                text = message.Message.Text,
                                sender = $"{message.Message.Owner.FirstName} {message.Message.Owner.LastName} {message.Message.Owner.Patronymic}",
                                senderId = message.Message.Owner.Id
                            };

                            await Clients.Caller.SendAsync(
                                message.Message.Owner.Id.Equals(user.Id) ? "ReceiveChatMessageAsSender" : "ReceiveChatMessage",
                                JsonConvert.SerializeObject(fullMessage));
                        }
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("ReceiveChatMessageAsSender", JsonConvert.SerializeObject(new List<ChatMessage>()));
                    }
                }
                else
                    await Clients.Caller.SendAsync("ReceiveChatMessageAsSender", JsonConvert.SerializeObject(new List<ChatMessage>()));
            }
        }

        /// <summary>
        /// Изменение название чата
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="newChatName"></param>
        /// <returns></returns>
        public async Task UpdateChatName(int chatId, string newChatName)
        {
            if (!string.IsNullOrEmpty(newChatName))
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await _unitOfWork.Chat.UpdateChatName(chatId, newChatName);
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();

                    await ResultOfChatNameUpdate(chatId, newChatName, true, "Название чата было успешно обновленно!");
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                    await ResultOfChatNameUpdate(null, null, false, "Во время изменения названия произошла ошибка! Повторите попытку!");
                }
            }
            else
                await ResultOfChatNameUpdate(null, null, false, "Название чата не может быть пустым!");
        }

        private async Task ResultOfChatNameUpdate(int? chatId, string newChatName, bool status, string message)
        {
            var result = new
            {
                chatId = chatId,
                newChatName = newChatName,
                status = status,
                message = message
            };

            if (status)
                await Clients.OthersInGroup(chatId.ToString()).SendAsync("UpdateChatNameResult", JsonConvert.SerializeObject(result));

            await Clients.Caller.SendAsync("UpdateChatNameResult", JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// Загрузка списка участников чата
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task LoadChatParticipant(int chatId)
        {
            List<ApplicationUser> participants = (await _unitOfWork.Chat.GetChatParticipants(chatId)).ToList();

            if (participants is null)
                await Clients.Caller.SendAsync("LoadChatParticipantResult", JsonConvert.SerializeObject(null));
            else
                await Clients.Caller.SendAsync("LoadChatParticipantResult", JsonConvert.SerializeObject(participants));
        }

        /// <summary>
        /// Удаление участника из чата
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="participantId"></param>
        /// <returns></returns>
        public async Task RemoveParticipantFromChat(int chatId, string participantId)
        {
            var user = await _userManager.GetUserAsync(Context.User);

            if (user != null)
            {
                try
                {
                    var chat = (await _unitOfWork.Chat.Find(c => c.ChatId == chatId)).FirstOrDefault();

                    // если текущий пользователь является владельцем чата
                    if (user.Id.Equals(chat.Owner.Id))
                    {
                        await _unitOfWork.BeginTransactionAsync();

                        try
                        {
                            await _unitOfWork.Chat.RemoveParticipantFromChat(chatId, participantId);
                            await _unitOfWork.SaveChangesAsync();

                            List<ApplicationUser> participants = (await _unitOfWork.Chat.GetChatParticipants(chatId)).ToList();

                            // если был удален последний участник чата то
                            if (participants.Count <= 0)
                            {
                                IEnumerable<ChatMessageParticipant> messages = await _unitOfWork.ChatMessageParticipant.Find(c => c.ChatId == chatId);

                                foreach (var message in messages)
                                {
                                    await _unitOfWork.ChatMessage.Delete(message.MessageId);
                                }

                                // удаляем чат
                                await _unitOfWork.Chat.Delete(chatId);
                                // отправляем на клиент что чат был удален
                                await Clients.Caller.SendAsync("LeaveСhatResult", JsonConvert.SerializeObject(new { status = true, chat = chatId }));
                            }

                            await _unitOfWork.SaveChangesAsync();
                            _unitOfWork.Commit();

                            await SendResultDeletingParticipant(true, "Учасник был успешно удален из чата!", chatId, participantId);
                        }
                        catch (Exception)
                        {
                            _unitOfWork.Rollback();
                        }
                    }
                    else
                    {
                        await SendResultDeletingParticipant(false, "Удалять участников чата может только владелец чата!!!", null, null);
                    }
                }
                catch (Exception)
                {
                    await SendResultDeletingParticipant(false, "Учасник не был удален из чата!", null, null);
                }
            }
            else
                await SendResultDeletingParticipant(false, "При удалении участника произошла неизвестная ошибка!", null, null);
        }

        private async Task SendResultDeletingParticipant(bool status, string message, int? chatId, string participantId)
        {
            var result = new
            {
                status,
                message,
                chatId,
                participantId
            };

            await Clients.Caller.SendAsync("RemoveParticipantFromChatResult", JsonConvert.SerializeObject(result));

            if (status)
                await Clients.Group(chatId.ToString()).SendAsync("RemoveParticipantFromList", JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// Метод для выхода из чата
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task LeaveСhat(int chatId)
        {
            var user = await _userManager.GetUserAsync(Context.User);

            if (user != null)
            {
                await _unitOfWork.BeginTransactionAsync();

                var status = LeaveStatus.NotLeave;
                try
                {
                    status = await _unitOfWork.Chat.LeaveChat(chatId, user.Id);
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                    status = LeaveStatus.NotLeave;
                }

                switch (status)
                {
                    case LeaveStatus.Leave:
                        {
                            await Clients.Caller.SendAsync("LeaveСhatResult", JsonConvert.SerializeObject(new { status = true, chat = chatId }));

                            var result = new
                            {
                                status = true,
                                message = "",
                                chatId,
                                participantId = user.Id
                            };

                            await Clients.Group(chatId.ToString()).SendAsync("RemoveParticipantFromList", JsonConvert.SerializeObject(result));
                        }
                        break;
                    case LeaveStatus.NotLeave:
                        await Clients.Caller.SendAsync("LeaveСhatResult", JsonConvert.SerializeObject(new { status = false }));
                        break;
                    case LeaveStatus.ChatRemoved:
                        await Clients.Caller.SendAsync("LeaveСhatResult", JsonConvert.SerializeObject(new { status = true, chat = chatId }));
                        break;
                }
            }
        }

        /// <summary>
        /// Удаление доступных пользователю сообщений в чате
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messagesIds"></param>
        /// <returns></returns>
        public async Task RemoveMessageFromMe(int chatId, List<ChatMessageParticipant> messages)
        {
            var user = await _userManager.GetUserAsync(Context.User);

            if (user != null)
            {
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    var chat = await _unitOfWork.Chat.FindById(chatId);

                    // если текущий пользователь является владельцем чата
                    if (chat.ChatParticipants.FirstOrDefault(t => t.Id.Equals(user.Id)) != null)
                    {
                        foreach (var message in messages)
                        {
                            var searchMessage = (await _unitOfWork.ChatMessageParticipant.Find(m => m.ChatId == chatId &&
                                    m.Id.Equals(user.Id) &&
                                    m.ChatMessageParticipantId == message.ChatMessageParticipantId)).FirstOrDefault();

                            if (searchMessage != null)
                            {
                                int countMessages = (await _unitOfWork.ChatMessageParticipant.Find(m => m.MessageId == searchMessage.MessageId)).Count();

                                await _unitOfWork.ChatMessageParticipant.Delete(searchMessage.ChatMessageParticipantId);

                                if (countMessages <= 1)
                                {
                                    await _unitOfWork.ChatMessage.Delete(searchMessage.MessageId);
                                }

                                await _unitOfWork.SaveChangesAsync();

                                await Clients.Caller.SendAsync("RemoveMessageFromMeResult", JsonConvert.SerializeObject(new { id = searchMessage.ChatMessageParticipantId }));
                            }
                        }
                    }

                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("RemoveMessageFromMe Error\r\n" + ex.Message);
                    _unitOfWork.Rollback();
                }
            }
        }
    }
}