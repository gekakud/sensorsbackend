using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Core.Common.Interfaces;
using Core.Common.Mongo;
using Core.Common.RabbitMessageBus;
using Core.Common.Redis;
using Core.Common.SharedDataObjects;
using Core.Common.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DataProcessorService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // ConfigureServices is where you register dependencies. This gets
        // called by the runtime before the Configure method, below.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(
                opt => { opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; });
            services.AddRedis();
            
            services.AddSwaggerDocs();

            // setup the Autofac container
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.AddRabbitMq();

            builder.AddMongo();
            //builder.AddMongoRepository<User>("Users");
            builder.AddMongoRepository<SensorData>("SensorData");
            builder.AddMongoRepository<GpsPoint>("GPSpoints");
            
            builder.RegisterType<RabbitMessageBus>().As<IMessageBus>().SingleInstance();

            //here we will have an access to all services
            //must be registered at the end, since depends on others
            builder.RegisterType<MessageHandlingService>().SingleInstance();
            var container = builder.Build();

            // return the IServiceProvider implementation
            return container.Resolve<IServiceProvider>();
        }

        // Inside of Configure method we set up middleware that handles
        // every HTTP request that comes to our application
        // Configure is where you add middleware. This is called after
        // ConfigureServices. You can use IApplicationBuilder.ApplicationServices
        // here if you need to resolve things from the container.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //initialize message handling
            app.ApplicationServices.GetService<MessageHandlingService>();
            
            app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            app.UseSwaggerDocs();
            app.UseMvc();
        }
    }
}
