using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Formatting;
using System.Text.Json;
using TechChallange.Api.Controllers.Region.Dto;
using TechChallange.Api.Response;
using TechChallange.Domain.Cache;
using TechChallange.Domain.Region.Entity;
using TechChallange.Infrastructure.Cache;
using TechChallange.Infrastructure.Context;

namespace TechChallange.Test.IntegrationTests.Controllers
{
    public class RegionControllerTests : IClassFixture<MsSqlContainerFixture>//: IClassFixture<TechChallangeApplicationFactory>
    {
        private readonly TechChallangeApplicationFactory _techChallangeApplicationFactory;

        private readonly HttpClient _client;

        public RegionControllerTests(MsSqlContainerFixture fixture)
        {
            var factory = new WebApplicationFactory<Program>()
           .WithWebHostBuilder(builder =>
           {
               builder.ConfigureServices(services =>
               {
                   // Remova a configuração existente e use o banco do TestContainers
                   var connectionString = fixture.MsSqlContainer.GetConnectionString();
                   services.ConfigureDbContext(connectionString);

                   // Configurar Redis
                   var redisConnectionString = fixture.RedisContainer.GetConnectionString();
                   services.ConfigureRedis(redisConnectionString);

                   services.AddScoped<ICacheRepository, CacheRepository>();
                   services.AddScoped<ICacheWrapper, CacheWrapper>();


                   // Criar um escopo para aplicar as migrations
                   using var scope = services.BuildServiceProvider().CreateScope();
                   var dbContext = scope.ServiceProvider.GetRequiredService<TechChallangeContext>();
                   dbContext.Database.Migrate(); // Aplica as migrations automaticamente

               });
           });

            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Test()
        {
          //  var client = _techChallangeApplicationFactory.CreateClient();

            var response = await _client.GetAsync("region/get-all?pageSize=10&page=1");

            var resp = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponsePagedDto<IEnumerable<RegionResponseDto>>>(resp,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result?.Data);
        }

        [Fact]
        public async Task Teste2()
        {
            //var client = _techChallangeApplicationFactory.CreateClient();



            var regioncreate = new RegionCreateDto
            {
                Name = "SP",
                Ddd = "11"
            };

            await _client.PostAsync("region", regioncreate, new JsonMediaTypeFormatter());

            var response = await _client.GetAsync("region/get-all?pageSize=10&page=1");

            var resp = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponsePagedDto<IEnumerable<RegionResponseDto>>>(resp,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            var responseid = await _client.GetAsync($"region/get-by-id/{result?.Data?.FirstOrDefault()?.Id}");

            var respId = await responseid.Content.ReadAsStringAsync();

            var resultId = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(respId,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, responseid.StatusCode);
            Assert.Equal(result?.Data?.FirstOrDefault(), resultId?.Data);
        }

        [Fact]
        public async Task ShouldDeleteLogicalRegionById()
        {
            var regioncreate = new RegionCreateDto
            {
                Name = "SP",
                Ddd = "11"
            };

            await _client.PostAsync("region", regioncreate, new JsonMediaTypeFormatter());

            //var client = _techChallangeApplicationFactory.CreateClient();

            var responses = await _client.GetAsync("region/get-all?pageSize=10&page=1");

            var respa = await responses.Content.ReadAsStringAsync();

            var results = JsonSerializer.Deserialize<BaseResponsePagedDto<IEnumerable<RegionResponseDto>>>(respa,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var response = await _client.DeleteAsync($"region/{results.Data.FirstOrDefault().Id}");

            var resp = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(resp,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //var regionDb = await _dbContext.Region.FirstOrDefaultAsync(r => r.Id == regionEntity.Id);


            var responseid = await _client.GetAsync($"region/get-by-id/{results.Data.FirstOrDefault().Id}");

            var respId = await responseid.Content.ReadAsStringAsync();

            var resultId = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(respId,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });



            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
          //  Assert.True(regionDb.IsDeleted);
        }
    }
}
