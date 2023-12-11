using HtmlAgilityPack;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public List<Videos> GetVideos()
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
                foreach (var hrefValue in hrefAttributes)
                {
                    videosList.Add(new Videos(hrefValue.GetAttributeValue("href", "")));
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
