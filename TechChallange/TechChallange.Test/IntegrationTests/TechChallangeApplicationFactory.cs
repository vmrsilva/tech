using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;
using Testcontainers.Redis;
using Microsoft.EntityFrameworkCore;
using TechChallange.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient;
using TechChallange.Domain.Region.Entity;
using TechChallange.Infrastructure.Cache;
using TechChallange.Domain.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace TechChallange.Test.IntegrationTests
{
    

    public sealed  class TechChallangeApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder().Build();
        private readonly RedisContainer _redisContainer = new RedisBuilder().Build();

        private string? _connectionString;
        private string? _connectionStringRedis;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var settings = new Dictionary<string, string?>
            {
                { "ConnectionStrings:Database", _connectionString },
                { "ConnectionStrings:Cache", _connectionStringRedis }
            };

                config.AddInMemoryCollection(settings!);
            });
        }

        public async Task InitializeAsync()
        {
            await _sqlContainer.StartAsync();
            _connectionString = _sqlContainer.GetConnectionString();

            await _redisContainer.StartAsync();
            _connectionStringRedis = _redisContainer.GetConnectionString();

            await WaitForDatabaseAsync();

            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TechChallangeContext>();
            await context.Database.EnsureCreatedAsync();

            var region = new RegionEntity("SP", "11");
            context.Region.Add(region);
            await context.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            await _sqlContainer.StopAsync();
            await _redisContainer.StopAsync();
        }

        private async Task WaitForDatabaseAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    await connection.OpenAsync();
                    return;
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }
            throw new Exception("Database did not answer.");
        }
    }


}
