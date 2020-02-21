using Autofac;
using Core.Common.Extensions;
using Core.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Core.Common.Mongo
{
    public class MongoDbConfig
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public bool Seed { get; set; }
    }

    public static class Extensions
    {
        public static void AddMongo(this ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var options = configuration.GetOptions<MongoDbConfig>("mongo");

                return options;
            }).SingleInstance();

            builder.Register(context =>
            {
                var options = context.Resolve<MongoDbConfig>();

                return new MongoClient(options.ConnectionString);
            }).SingleInstance();

            builder.Register(context =>
            {
                var options = context.Resolve<MongoDbConfig>();
                var client = context.Resolve<MongoClient>();
                return client.GetDatabase(options.Database);

            }).InstancePerLifetimeScope();
        }

        public static void AddMongoRepository<TEntity>(this ContainerBuilder builder, string collectionName)
            where TEntity : IIdentifiable
        { 
            builder.Register(ctx => new DataRepository<TEntity>(ctx.Resolve<IMongoDatabase>(), collectionName))
                .As<IDataRepository<TEntity>>()
                .InstancePerLifetimeScope();
        }
    }
}
