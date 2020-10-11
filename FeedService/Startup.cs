using FeedService.Infrastructure;
using FeedService.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


[assembly: FunctionsStartup(typeof(FeedService.Startup))]

namespace FeedService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IPiletService, PiletService>();
            builder.Services.AddSingleton<IStorageAccountRepository, StorageAccountRepository>();
        }
    }
}
