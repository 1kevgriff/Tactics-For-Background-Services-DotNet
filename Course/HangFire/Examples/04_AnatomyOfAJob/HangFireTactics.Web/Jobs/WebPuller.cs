using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Xml;

namespace HangFireTactics.Web.Jobs;

// ReSharper disable once ClassNeverInstantiated.Global
public class WebPuller
{
    private readonly ILogger<WebPuller> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public WebPuller(ILogger<WebPuller> logger, IHttpClientFactory httpClientFactory )
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task GetRssItemUrlsAsync(string rssFeedUrl, string filename)
    {
        // check if filename directory exists
        var directory = Path.GetDirectoryName(filename);
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        using var client = _httpClientFactory.CreateClient();
        var rssContent = await client.GetStringAsync(rssFeedUrl);

        using var xmlReader = XmlReader.Create(new StringReader(rssContent));
        var feed = SyndicationFeed.Load(xmlReader);

        var rssItemUrls = feed.Items.Select(item => item.Links.FirstOrDefault()?.Uri.AbsoluteUri).ToList();

        var json = JsonSerializer.Serialize(rssItemUrls);
        await File.WriteAllTextAsync(filename, json);
    }

    public async Task DownloadFileFromUrl(string url, string filePath)
    {
        using var client = _httpClientFactory.CreateClient();

        using (_logger.BeginScope("DownloadFileFromUrl({url}, {filePath})", url, filePath))
        {
            try
            {
                _logger.LogInformation("Downloading file from {url}...", url);
                using var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Writing file to {filePath}...", filePath);
                await using var stream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                await stream.CopyToAsync(fileStream);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Error downloading file from {url}", url);
            }
        }
    }
}