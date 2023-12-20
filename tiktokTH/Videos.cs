using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium.DevTools.V118.Network;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace tiktokTH
{
    internal class Videos
    {
        public string Username { get; set; }
        public string VideoUrl { get; set; }
        public string VideoTitle { get; set; }
        public DateTime VideoDate { get; set; }
        public string VideoId { get; set; }
        public string VideoHastags { get; set; }
        public string DownloadUrl { get; set; }

        private VideoDateParser dateParser = new VideoDateParser();


        public Videos(string videoUrl)
        {

            VideoUrl = videoUrl;
            Username = ExtractUsernameFromUrl(videoUrl);

        }

        public static async Task<Videos> CreateAsync(string videoUrl, string Date)
        {
            var videos = new Videos(videoUrl);
            await videos.InitializeAsync(videoUrl, Date);
            return videos;
        }

        public async Task InitializeAsync(string videoUrl, string Date)
        {
            VideoTitle = "deu merda";// await GetVideoTitleAsync();
            VideoId = await GetVideoIdAsync(videoUrl);
            VideoDate = dateParser.TranslateVideoDate(Date);
            DownloadUrl = await GetDownloadUrlAsync(videoUrl);

        }
        private static async Task<string> GetVideoIdAsync(string videoUrl)
        {
            // Use regex to extract the video ID from the TikTok URL
            Match match = Regex.Match(videoUrl, @"/video/(\d+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }
        private string ExtractUsernameFromUrl(string videoUrl)
        {
            var match = Regex.Match(videoUrl, @"\/@([^\/]+)\/video\/");
            return match.Success ? $"@{match.Groups[1].Value}" : string.Empty;
        }

        private async Task<string> GetVideoTitleAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var videoPageContent = await client.GetStringAsync(VideoUrl);
                    var videoHtmlDoc = new HtmlDocument();
                    videoHtmlDoc.LoadHtml(videoPageContent);

                    var titleElement = videoHtmlDoc.DocumentNode.SelectSingleNode("//span[contains(@class, 'tiktok-j2a19r-SpanText efbd9f0')]");
                    return titleElement?.InnerText.Trim() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving video title: {ex.Message}");
                    return "null";
                }
            }
        }

        private async Task<string> GetDownloadUrlAsync(string videoUrl)
        {

            string tokenValue = "";

            using (HttpClient client = new HttpClient())
            {
                // Set headers
                client.DefaultRequestHeaders.Add("Host", "ttdownloader.com");
                client.DefaultRequestHeaders.Add("Cookie", "PHPSESSID=hc170bnsorlhk88n4qst7000s5; _gid=GA1.2.2075230462.1703078434; cf_clearance=jZlrUZOaVRAt3qEka5GI8qLQf4ng6Im0CGhRu0tXxkM-1703078430-0-2-c1bb6225.d2b6af7c.96ba3dc5-0.2.1703078430; _ga_CXXL1WRV92=GS1.1.1703078433.15.1.1703078629.0.0.0; _ga=GA1.1.1549109859.1702222463");
                client.DefaultRequestHeaders.Add("cache-control", "max-age=0");
                client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\", \"Google Chrome\";v=\"120\"");
                client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                client.DefaultRequestHeaders.Add("dnt", "1");
                client.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                client.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
                client.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
                client.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
                client.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
                client.DefaultRequestHeaders.Add("accept-language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");

                string url = "https://ttdownloader.com/pt/";


                // Make the GET request
                HttpResponseMessage response = await client.GetAsync(url);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read and parse the HTML content
                    string content = await response.Content.ReadAsStringAsync();

                    // Extract the value of //*[@id="token"]
                    tokenValue = ExtractTokenValue(content);

                    // Output the token value
                    Console.WriteLine($"Token Value: {tokenValue}");
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                }
            }


            using (var client = new HttpClient())
            {
                string responseBody;
                var url = "https://ttdownloader.com/search/";
                var postData = $"url={VideoUrl}&format=&token={tokenValue}";

                var request = new HttpRequestMessage(HttpMethod.Post, url);
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

                var response = await client.SendAsync(request);
                responseBody = await response.Content.ReadAsStringAsync();

                return ExtractDownloadUrl(responseBody);
            }
        }

        private static string ExtractTokenValue(string htmlContent)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // Use XPath to select the desired element
            HtmlNode tokenNode = doc.DocumentNode.SelectSingleNode("//*[@id='token']");

            // Check if the node is found
            if (tokenNode != null)
            {
                // Get the value of the 'value' attribute
                string tokenValue = tokenNode.GetAttributeValue("value", "");
                return tokenValue;
            }
            else
            {
                return null; // or handle the case where the node is not found
            }
        }

        private static string ExtractDownloadUrl(string responseContent)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(responseContent);

                var downloadLink = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='download-link']");
                return downloadLink?.GetAttributeValue("href", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting download URL: {ex.Message}");
                return null;
            }
        }
    }
}
