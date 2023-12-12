using HtmlAgilityPack;
using OpenQA.Selenium;
using System.Threading.Tasks;

namespace tiktokTH
{
    internal class TikTokScraper
    {
        private readonly IWebDriver _driver;

        public TikTokScraper(IWebDriver driver)
        {
            _driver = driver;
        }

        public async Task<List<Videos>> GetVideosAsync(IProgress<int> overallProgress = null)
        {
            List<Videos> videosList = new List<Videos>();

            // Get the page source
            string pageSource = _driver.PageSource;

            // Load the page source into HtmlAgilityPack
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

            // Select href attributes that contain "/video/"
            var hrefAttributes = htmlDoc.DocumentNode.SelectNodes("//a[contains(@href, '/video/')]/@href");

            if (hrefAttributes != null)
            {
                int totalCount = hrefAttributes.Count;
                int processedCount = 0;

                foreach (var hrefValue in hrefAttributes)
                {
                    videosList.Add(await Videos.CreateAsync(hrefValue.GetAttributeValue("href", "")));

                    // Report progress for each video creation
                    processedCount++;
                    int percentage = (int)((double)processedCount / totalCount * 100);
                    overallProgress?.Report(percentage);
                }
            }
            else
            {
                Console.WriteLine("No href attributes found in the document containing '/video/'.");
            }

            return videosList;
        }

    }
}
