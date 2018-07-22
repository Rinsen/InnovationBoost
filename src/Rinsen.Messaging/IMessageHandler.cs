using System.Threading.Tasks;

namespace Rinsen.Messaging
{
    public interface IMessageHandler
    {
        Task<MessageResult> HandledMessage(Message message);

        MessageHandlerType Type { get; }
    }

    public enum MessageHandlerType
    {
        SendGrid = 0,
    }
}
