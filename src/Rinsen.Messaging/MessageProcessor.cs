using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.Messaging
{
    public class MessageProcessor
    {
        private readonly IMessageHandlerFactory _messageHandlerFactory;

        private readonly MessageDbContext _messageDbContext;

        public MessageProcessor(IMessageHandlerFactory messageHandlerFactory, MessageDbContext messageDbContext)
        {
            _messageHandlerFactory = messageHandlerFactory;
            _messageDbContext = messageDbContext;
        }

        public async Task Process(Guid senderId, Guid receiverId, string subjectText, string messageText)
        {
            var handledMessages = new List<HandledMessage>();

            // Create and save message
            var message = new Message
            {
                Created = DateTimeOffset.Now,
                ReceiverId = receiverId,
                SenderId = senderId,
                Subject = subjectText,
                Text = messageText
            };

            await _messageDbContext.AddAsync(message);

            await _messageDbContext.SaveChangesAsync();

            // Call handlers
            var preferedMessageTypes = await _messageDbContext.PreferedMessageTypes.Where(m => m.IdentityId == receiverId).ToListAsync();

            foreach (var messageHandler in _messageHandlerFactory.CreateHandlers())
            {
                if (!preferedMessageTypes.Any() || 
                    preferedMessageTypes.Any(mt => mt.Type == messageHandler.Type))
                {
                    var messageResult = await messageHandler.HandledMessage(message);

                    handledMessages.Add(new HandledMessage
                    {
                        MessageId = message.Id,
                        Sent = messageResult.Sent,
                        Type = messageHandler.Type
                    });
                }
            }

            // Save result
            await _messageDbContext.AddRangeAsync(handledMessages);
            await _messageDbContext.SaveChangesAsync();
        }
    }
}
