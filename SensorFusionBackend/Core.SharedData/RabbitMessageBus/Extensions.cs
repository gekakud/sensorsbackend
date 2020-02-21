using Autofac;
using Core.Common.Extensions;
using Core.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Core.Common.RabbitMessageBus
{
    public static class Extensions
    {
        public static void AddRabbitMq(this ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var options = configuration.GetOptions<BusConfig>("rabbit");

                return options;
            }).SingleInstance();

            builder.RegisterType<RabbitMessageBus>().As<IMessageBus>().SingleInstance();
        }
    }
}
