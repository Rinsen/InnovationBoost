using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Rinsen.Messaging
{
    public class Class1
    {
        private readonly IConfiguration _configuration;

        public Class1(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task SendMessage(EmailMessage emailMessage)
        {
            var apiKey = _configuration["Rinsen.SendGrid.ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "Example User");
            var to = new EmailAddress("test@example.com", "Example User");
            var subject = emailMessage.Subject;
            var plainTextContent = emailMessage.PlainTextContent;
            var htmlContent = emailMessage.HtmlContent;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                // Failure
                emailMessage.Sent = DateTimeOffset.Now;
            }
        }
    }

    public class EmailMessage
    {
        public int Id { get; set; }

        public Guid FromIdentityId { get; set; }

        public Guid ToIdentityId { get; set; }

        public string Subject { get; set; }

        public string PlainTextContent { get; set; }

        public string HtmlContent { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Sent { get; set; }

    }
}
