using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace tiktokTH
{
    internal class Videos
    {
        public string Username { get; set; }
        public string VideoUrl { get; set; }
        public string VideoTitle { get; set; }
        public string VideoHastags { get; set; }
        public string DownloadUrl { get; set; }

        public Videos(string videoUrl)
        {
            VideoUrl = videoUrl;
            Username = ExtractUsernameFromUrl(videoUrl);
        }

        public static async Task<Videos> CreateAsync(string videoUrl)
        {
            var videos = new Videos(videoUrl);
            await videos.InitializeAsync();
            return videos;
        }

        public async Task InitializeAsync()
        {
            VideoTitle = await GetVideoTitleAsync();
            DownloadUrl = await GetDownloadUrlAsync();
            Console.WriteLine(DownloadUrl);
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

        private async Task<string> GetDownloadUrlAsync()
        {
            using (var client = new HttpClient())
            {
                string responseBody;
                var url = "https://ttdownloader.com/search/";
                var postData = $"url={VideoUrl}&format=&token=2004495bf50688664cda56e71eaa431502bfd26c0e70412ebf26e732952a99bf";

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
