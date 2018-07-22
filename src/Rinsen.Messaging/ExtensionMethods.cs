using Microsoft.Extensions.DependencyInjection;

namespace Rinsen.Messaging
{
    public static class ExtensionMethods
    {
        public static void AddRinsenMessaging(this IServiceCollection services)
        {
            services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();
            //services.AddScoped<MessageDbContext, MessageDbContext>();
            services.AddScoped<MessageProcessor, MessageProcessor>();
            services.AddScoped<SendGridMessageHandler, SendGridMessageHandler>();
        }
    }
}
