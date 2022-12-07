using System;
using System.Xml;
using Microsoft.Azure.Functions.Worker;
using  Microsoft.Azure.Functions.Worker.Extensions;
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
        public async Task Run([TimerTrigger("*/30 * * * * *")] TimerInfo myTimer, ILogger logger)
        {
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var url = "https://consultwithgriff.com/rss.xml";


            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // convert xml response to json
            var xml = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.SerializeXmlNode(new XmlDocument { InnerXml = xml });
            
            // write json to blob storage
            logger.LogTrace(json);
        }
    }
}
