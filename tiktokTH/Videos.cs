using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Videos(string videoUrl)
        {
            VideoUrl = videoUrl;
            Username = ExtractUsernameFromUrl(videoUrl);
            VideoTitle = GetVideoTitle(videoUrl);
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

        private string ExtractUsernameFromUrl(string videoUrl)
        {
            // Use regex to extract the username from the TikTok video URL
            var match = Regex.Match(videoUrl, @"\/@([^\/]+)\/video\/");
            return match.Success ? $"@{match.Groups[1].Value}" : string.Empty;
        }
    }
}
