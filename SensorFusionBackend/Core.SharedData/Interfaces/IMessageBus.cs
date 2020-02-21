using System;
using System.Threading.Tasks;
using Core.Common.RabbitMessageBus;

namespace Core.Common.Interfaces
{
    public interface IMessageBus
    {
        IDisposable Subscribe(string id, Action<Message> messageAction);
        IDisposable Subscribe(string id, Action<RequestMessage> messageAction);
        IDisposable SubscribeAsync(string id, Func<RequestMessage, Task> messageAction);
        IDisposable SubscribeToTopic(string id, Action<RequestMessage> messageAction, string topic);

        void Publish(RequestMessage message);
        Task PublishAsync(RequestMessage message);      

        IDisposable Response(Func<RequestMessage, ResponseMessage> f);
        IDisposable ResponseAsync(Func<RequestMessage, Task<ResponseMessage>> f);

        ResponseMessage Request(RequestMessage requestMessage);
        Task<ResponseMessage> RequestAsync(RequestMessage requestMessage);

        bool IsMessageBusAlive();
        void DestroyBus();
    }
}
