using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using TechChallange.Api.Controllers.Region.Dto;
using TechChallange.Api.Response;
using TechChallange.Domain.Region.Entity;

namespace TechChallange.Test.IntegrationTests.Controllers
{
    public class RegionControllerTests : IClassFixture<TechChallangeApplicationFactory>
    {
        private readonly TechChallangeApplicationFactory _techChallangeApplicationFactory;

        public RegionControllerTests(TechChallangeApplicationFactory techChallangeApplicationFactory)
        {
            _techChallangeApplicationFactory = techChallangeApplicationFactory;
        }

        [Fact]
        public async Task Test()
        {
            var client = _techChallangeApplicationFactory.CreateClient();

            var response = await client.GetAsync("region/get-all?pageSize=10&page=1");

            var resp = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponsePagedDto<IEnumerable<RegionResponseDto>>>(resp,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result?.Data);
        }

        [Fact]
        public async Task Teste2()
        {
            var client = _techChallangeApplicationFactory.CreateClient();


            var response = await client.GetAsync("region/get-all?pageSize=10&page=1");

            var resp = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponsePagedDto<IEnumerable<RegionResponseDto>>>(resp,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            var responseid = await client.GetAsync($"region/get-by-id/{result?.Data?.FirstOrDefault()?.Id}");

            var respId = await responseid.Content.ReadAsStringAsync();

            var resultId = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(respId,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, responseid.StatusCode);
            Assert.Equal(result?.Data?.FirstOrDefault(), resultId?.Data);
        }

        [Fact]
        public async Task ShouldDeleteLogicalRegionById()
        {

            var client = _techChallangeApplicationFactory.CreateClient();

            var responses = await client.GetAsync("region/get-all?pageSize=10&page=1");

            var respa = await responses.Content.ReadAsStringAsync();

            var results = JsonSerializer.Deserialize<BaseResponsePagedDto<IEnumerable<RegionResponseDto>>>(respa,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var response = await client.DeleteAsync($"region/{results.Data.FirstOrDefault().Id}");

            var resp = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(resp,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //var regionDb = await _dbContext.Region.FirstOrDefaultAsync(r => r.Id == regionEntity.Id);


            var responseid = await client.GetAsync($"region/get-by-id/{results.Data.FirstOrDefault().Id}");

            var respId = await responseid.Content.ReadAsStringAsync();

            var resultId = JsonSerializer.Deserialize<BaseResponseDto<RegionResponseDto>>(respId,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });



            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
          //  Assert.True(regionDb.IsDeleted);
        }
    }
}
