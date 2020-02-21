using Core.Common.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DataAnalyticsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseLogging()
                .UseUrls("http://*:6002");

    }
}
