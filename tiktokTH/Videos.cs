using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tiktokTH
{
    internal class Videos
    {
        public string Username;
        public string VideoUrl;
        public string VideoTitle;
        public string VideoHastags;
        public string DownloadUrl;




        public Videos(string videoUrl)
        {
            VideoUrl = videoUrl;
            Username = ExtractUsernameFromUrl(videoUrl);
            VideoTitle = GetVideoTitle(videoUrl);


            // Call the asynchronous method after the object has been constructed
            InitializeAsync(videoUrl);

        }
        // Asynchronous method to initialize additional properties
        private async void InitializeAsync(string videoUrl)
        {
            DownloadUrl = await GetDownloadUrl(videoUrl);
        }
        private string GetVideoTitle(string videoUrl)
        {
            // Use HttpClient to get the HTML content of the video page
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string videoPageContent = client.GetStringAsync(videoUrl).Result;

                    // Load the page content into HtmlAgilityPack
                    HtmlDocument videoHtmlDoc = new HtmlDocument();
                    videoHtmlDoc.LoadHtml(videoPageContent);

                    // Select the element with class="tiktok-j2a19r-SpanText efbd9f0"
                    var titleElement = videoHtmlDoc.DocumentNode.SelectSingleNode("//span[contains(@class, 'tiktok-j2a19r-SpanText efbd9f0')]");

                    return titleElement?.InnerText.Trim() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving video title: {ex.Message}");
                    return string.Empty;
                }
            }
        }

        private static async Task<string> GetDownloadUrl(string tikTokUrl)
        {
            string responseBody;

            using (HttpClient client = new HttpClient())
            {
                // URL and data
                string url = "https://ttdownloader.com/search/";
                string postData = $"url={tikTokUrl}&format=&token=419cbc2e380a5b2cb33c62922c9803ded89797b101aa1f6ebb01de87e3950170";

                // Creating the HttpRequestMessage
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("authority", "ttdownloader.com");
                request.Headers.Add("accept", "*/*");
                request.Headers.Add("accept-language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
                request.Headers.Add("cookie", "PHPSESSID=foggri00tgcb6vd397etrsu9s2; _gid=GA1.2.742261162.1702222463; cf_clearance=8rvKsKnf8I3C0KYrMunJNTiJHrxOFtAboID1YdMZcGs-1702295213-0-1-ea9d65e2.55e0d13a.4987558a-0.2.1702295213; _ga_CXXL1WRV92=GS1.1.1702295216.3.1.1702296979.0.0.0; _ga=GA1.2.1549109859.1702222463; _gat_gtag_UA_117413493_7=1");
                request.Headers.Add("dnt", "1");
                request.Headers.Add("origin", "https://ttdownloader.com");
                request.Headers.Add("referer", "https://ttdownloader.com/pt/");
                request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                request.Headers.Add("sec-fetch-dest", "empty");
                request.Headers.Add("sec-fetch-mode", "cors");
                request.Headers.Add("sec-fetch-site", "same-origin");
                request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
                request.Headers.Add("x-requested-with", "XMLHttpRequest");
                request.Content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");

                // Sending the request
                HttpResponseMessage response = await client.SendAsync(request);

                // Reading the response
                responseBody = await response.Content.ReadAsStringAsync();
               // Console.WriteLine(responseBody);
            }

            return ExtractDownloadUrl(responseBody);
        }

        private static string ExtractDownloadUrl(string responseContent)
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

        private string ExtractUsernameFromUrl(string videoUrl)
        {
            // Use regex to extract the username from the TikTok video URL
            var match = Regex.Match(videoUrl, @"\/@([^\/]+)\/video\/");
            return match.Success ? $"@{match.Groups[1].Value}" : string.Empty;
        }
    }
}
