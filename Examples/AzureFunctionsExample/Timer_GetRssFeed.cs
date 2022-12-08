using System;
using System.Xml;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Griffin
{
    public class Timer_GetRssFeed
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Timer_GetRssFeed(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Function("Timer_GetRssFeed")]
        [BlobOutput("output/consultwithgriff.xml")]
        public async Task<string> Run([TimerTrigger("*/30 * * * * *")] TimerInfo myTimer)
        {
            var url = "https://consultwithgriff.com/rss.xml";

            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var xml = await response.Content.ReadAsStringAsync();

            // write json to blob storage
            return xml;
        }
    }
}
