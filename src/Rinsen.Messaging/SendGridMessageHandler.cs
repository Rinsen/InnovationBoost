using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Rinsen.Messaging
{
    public class SendGridMessageHandler : IMessageHandler
    {
        private readonly IConfiguration _configuration;
        private readonly MessageDbContext _messageDbContext;

        public SendGridMessageHandler(IConfiguration configuration, MessageDbContext messageDbContext)
        {
            _configuration = configuration;
            _messageDbContext = messageDbContext;
        }

        public MessageHandlerType Type => MessageHandlerType.SendGrid;

        public async Task<MessageResult> HandledMessage(Message message)
        {
            var apiKey = _configuration["Rinsen.SendGrid.ApiKey"];
            var client = new SendGridClient(apiKey);

            var sender = await _messageDbContext.FindAsync<MessageIdentity>(new { IdentityId = message.SenderId });
            var receiver = await _messageDbContext.FindAsync<MessageIdentity>(new { IdentityId = message.ReceiverId });
            var from = new EmailAddress(sender.Email, $"{sender.GivenName} {sender.Surname}");
            var to = new EmailAddress(receiver.Email, $"{receiver.GivenName} {receiver.Surname}");
            var subject = message.Subject;
            var plainTextContent = message.Text;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, string.Empty);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                return new MessageResult { Sent = DateTimeOffset.Now };
            }

            // Failure
            return new MessageResult { Sent = null };
        }
    }

    
}
