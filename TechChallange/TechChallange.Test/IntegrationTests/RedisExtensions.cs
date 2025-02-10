using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechChallange.Test.IntegrationTests
{
    public static class RedisExtensions
    {
        public static IServiceCollection ConfigureRedis(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString));
            return services;
        }
    }
}
