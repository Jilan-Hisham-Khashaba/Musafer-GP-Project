using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GP.Services
{
    public class RecommendedService
    {
        //private readonly HttpClient _client;

        //public RecommendedService(HttpClient client)
        //{
        //    _client = client;
        //}
        //    public async Task<IDictionary<string, string>> GetRecommendationsAsync(CommentDTOFlask[] comments)
        //    {
        //        var jsonComments = JsonConvert.SerializeObject(comments);
        //        var requestContent = new StringContent(jsonComments, Encoding.UTF8, "application/json");

        //        var response = await _client.PostAsync("http://localhost:5000/recommend", requestContent);
        //        response.EnsureSuccessStatusCode();

        //        var responseBody = await response.Content.ReadAsStringAsync();
        //        var recommendations = JsonConvert.DeserializeObject<IDictionary<string, string>>(responseBody);

        //        return recommendations;
        //    }
        //}

    }
}





