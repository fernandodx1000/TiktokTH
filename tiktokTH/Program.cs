using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using tiktokTH;

internal class Program
    {
    private static async Task Main()
    {
        // Set up Chrome options to run in headless mode
        var chromeOptions = new ChromeOptions();
       // chromeOptions.AddArgument("--headless");

        IWebDriver driver = new ChromeDriver(chromeOptions);

        // Navigate to TikTok
        driver.Navigate().GoToUrl("https://www.tiktok.com/explore");

        // Wait for the page to load (you may need to adjust the wait time)
        System.Threading.Thread.Sleep(5000);

        // Create an instance of TikTokScraper and use it to retrieve videos
        TikTokScraper tikTokScraper = new TikTokScraper(driver);
        List<Videos> videosList = tikTokScraper.GetVideos();

        // Use the videosList as needed
        foreach (var video in videosList)
        {
            Console.WriteLine($"Username: {video.Username} \n Video URL: {video.VideoUrl}\n Video Title: {video.VideoTitle} \n");

            // Download the video
            string downloadUrl = await DownloadVideo(video.VideoUrl);
            if (!string.IsNullOrEmpty(downloadUrl))
            {
                Console.WriteLine($"Download URL: {downloadUrl}\n");
            }
            else
            {
                Console.WriteLine("Failed to retrieve download URL.\n");
            }
        }



        // Pause execution to observe the stored Videos instances
        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();

        // Close the browser
        driver.Quit();
    }

    static async Task<string> DownloadVideo(string tikTokUrl)
    {


        var handler = new HttpClientHandler();

        // If you are using .NET Core 3.0+ you can replace `~DecompressionMethods.None` to `DecompressionMethods.All`
        handler.AutomaticDecompression = ~DecompressionMethods.None;

        // In production code, don't destroy the HttpClient through using, but better use IHttpClientFactory factory or at least reuse an existing HttpClient instance
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests
        // https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        using (var httpClient = new HttpClient(handler))
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://ttdownloader.com/search/"))
            {
                request.Headers.TryAddWithoutValidation("authority", "ttdownloader.com");
                request.Headers.TryAddWithoutValidation("accept", "*/*");
                request.Headers.TryAddWithoutValidation("accept-language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
                request.Headers.TryAddWithoutValidation("cookie", "PHPSESSID=foggri00tgcb6vd397etrsu9s2; _gid=GA1.2.742261162.1702222463; _gat_gtag_UA_117413493_7=1; _ga_CXXL1WRV92=GS1.1.1702222463.1.0.1702222463.0.0.0; _ga=GA1.1.1549109859.1702222463; cf_clearance=ukFNdYQtssdVpIM5mKUebD05cX9mJTg3fkmaEiMOVrc-1702222462-0-1-ea9d65e2.55e0d13a.4987558a-0.2.1702222462; prefetchAd_4301805=true");
                request.Headers.TryAddWithoutValidation("dnt", "1");
                request.Headers.TryAddWithoutValidation("origin", "https://ttdownloader.com");
                request.Headers.TryAddWithoutValidation("referer", "https://ttdownloader.com/pt/");
                request.Headers.TryAddWithoutValidation("sec-ch-ua", "^^");
                request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                request.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "^^");
                request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                request.Headers.TryAddWithoutValidation("sec-fetch-site", "same-origin");
                request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
                request.Headers.TryAddWithoutValidation("x-requested-with", "XMLHttpRequest");

                request.Content = new StringContent($"url={tikTokUrl}&format=&token=a663492a0df457b8e6ebeb9962f167788682417c586f113d2decc63f7458a083", Encoding.UTF8);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded; charset=UTF-8");

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Extract the download URL (this is a sample extraction, adjust based on the actual response structure)
                    string downloadUrl = ExtractDownloadUrl(responseContent);

                    return downloadUrl;
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    return $"Request failed with status code: {response.StatusCode}";
                }
                return null;
            }
        }
         

    }
    static string ExtractDownloadUrl(string responseContent)
        {
            try
            {
                // Load the response content into HtmlAgilityPack
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(responseContent);

                // Select the first anchor tag with class "download-link"
                var downloadLink = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='download-link']");

                // Extract the href attribute value (download URL)
                string downloadUrl = downloadLink?.GetAttributeValue("href", null);

                return downloadUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting download URL: {ex.Message}");
                return null;
            }
        }

    }



