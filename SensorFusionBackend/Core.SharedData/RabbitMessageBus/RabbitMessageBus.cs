using System;
using System.Threading.Tasks;
using Core.Common.Interfaces;
using EasyNetQ;

namespace Core.Common.RabbitMessageBus
{
    /// <summary>
    /// RabbitMQ message bus implementation with EasyNetQ
    /// </summary>
    public class RabbitMessageBus:IMessageBus
    {
        private readonly IBus _bus;

        /// <summary>
        /// http://localhost:15672
        /// guest
        /// guest
        /// </summary>
        /// <param name="config"></param>
        public RabbitMessageBus(BusConfig config)
        {
            _bus = RabbitHutch.CreateBus("host=" + config.HostUrl);
            _bus.Advanced.Container.Resolve<IConventions>().QueueNamingConvention = (type, id) => type.Name + "_Queue_" + id;
            _bus.Advanced.Container.Resolve<IConventions>().ExchangeNamingConvention = type => type.Name + "_Exchange";
        }

        public IDisposable SubscribeAsync(string id, Func<RequestMessage, Task> messageAction)
        {
            return _bus.SubscribeAsync(id, messageAction).ConsumerCancellation;
        }

        public IDisposable Subscribe(string id, Action<Message> messageAction)
        {
            return _bus.Subscribe(id, messageAction).ConsumerCancellation;
        }

        public IDisposable Subscribe(string id, Action<RequestMessage> messageAction)
        {
            return _bus.Subscribe(id, messageAction).ConsumerCancellation;
        }

        public IDisposable SubscribeToTopic(string id, Action<RequestMessage> messageAction, string topic)
        {
            return _bus.Subscribe(id, messageAction, configuration => configuration.WithTopic(topic)).ConsumerCancellation;
        }

        public Task PublishAsync(RequestMessage message)
        {
            return _bus.PublishAsync(message);
        }

        public void Publish(RequestMessage message)
        {
            _bus.Publish(message);
        }

        public Task<ResponseMessage> RequestAsync(RequestMessage requestMessage)
        {
            return _bus.RequestAsync<RequestMessage, ResponseMessage>(requestMessage);
        }

        public ResponseMessage Request(RequestMessage requestMessage)
        {
            return _bus.Request<RequestMessage, ResponseMessage>(requestMessage);
        }

        public IDisposable ResponseAsync(Func<RequestMessage, Task<ResponseMessage>> f)
        {
            return _bus.RespondAsync(f);
        }

        public IDisposable Response(Func<RequestMessage, ResponseMessage> f)
        {
            return _bus.Respond(f);
        }

        public bool IsMessageBusAlive()
        {
            return _bus.IsConnected;
        }

        public void DestroyBus()
        {
            _bus?.Dispose();
        }
    }
}
