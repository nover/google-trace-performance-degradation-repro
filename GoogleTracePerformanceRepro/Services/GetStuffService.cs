using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GoogleTracePerformanceRepro.Services
{
    public class GetStuffService : IGetStuffService
    {
        private readonly HttpClient _httpClient;

        public GetStuffService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetStuffAsync()
        {
            var uri = "https://skarp.dk";

            var responseString = await _httpClient.GetStringAsync(uri);
            return responseString;
        }
    }

    public interface IGetStuffService
    {
        Task<string> GetStuffAsync();
    }
}