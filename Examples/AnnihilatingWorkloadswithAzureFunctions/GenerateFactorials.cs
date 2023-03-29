using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AnnihilatingWorkloadsWithAzureFunctions
{
    public class GenerateFactorials
    {
        private readonly ILogger _logger;
        private const string QueueName = "factorial-request";
        private const string BlobName = "factorial-done";

        public GenerateFactorials(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GenerateFactorials>();
        }

        [Function("GenerateFactorials")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var max = 10000;

            var queueConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            QueueClient factorialBuilderQueue = new QueueClient(queueConnectionString, QueueName, 
                new QueueClientOptions() { MessageEncoding = QueueMessageEncoding.Base64 });

            await factorialBuilderQueue.CreateIfNotExistsAsync();

            var tasks = new Task[max];

            for (int x = 0; x < max; x++)
            {
                tasks[x] = factorialBuilderQueue.SendMessageAsync(x.ToString(), TimeSpan.FromMinutes(1));
            }

            Task.WaitAll(tasks);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Generated!");

            return response;
        }

        [Function("CalculateFactorials")]
        public async Task CalculateFactorials([QueueTrigger(QueueName)]string item)
        {
            // parse item into long
            var factorial = long.Parse(item);
            _logger.LogInformation("Calculating {factorial}!", factorial);

            var result = Factorial(factorial);

            _logger.LogInformation("{factorial}! == {result}", factorial, result);

            var blobConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            BlobContainerClient blobContainerClient = new BlobContainerClient(blobConnectionString, BlobName);
            await blobContainerClient.CreateIfNotExistsAsync();

            // filename should be 5 padded zeros
            var filename = $"{factorial.ToString().PadLeft(10, '0')}.txt";
            BlobClient client = blobContainerClient.GetBlobClient(filename);

            // write result to stream
            using var stream = await client.OpenWriteAsync(true);
            using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(result.ToString());
            await writer.FlushAsync();
            writer.Close();
        }

        public static BigInteger Factorial(long x)
        {
            if (x < 0)
            {
                throw new ArgumentException("Factorial is not defined for negative integers.");
            }
            BigInteger result = 1;
            for (int i = 2; i <= x; i++)
            {
                result *= i;
            }
            return result;
        }

    }


}
