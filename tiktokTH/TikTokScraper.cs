using HtmlAgilityPack;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


            var hrefAttributes = htmlDoc.DocumentNode.SelectNodes("//a[contains(@href, '/video/')]/@href");

            if (hrefAttributes != null)
            {

                int totalCount = hrefAttributes.Count;
                int processedCount = 0;

                foreach (var hrefAttribute in hrefAttributes)
                {
                    string hrefValue = hrefAttribute.GetAttributeValue("href", "");

                    // Extract text from elements with class "css-dennn6-DivTimeTag"
                    var anchorElement = htmlDoc.DocumentNode.SelectSingleNode($"//a[@href='{hrefValue}']");
                    var date = "xx/xx/xx";

                    if (anchorElement != null)
                    {
                        // Print information about the anchorElement
                        Console.WriteLine($"Found anchorElement with href '{hrefValue}':");

                        // Print information about all inner nodes of the anchorElement
                        foreach (var innerNode in anchorElement.ChildNodes)
                        {
                           // Console.WriteLine($"  InnerNode: {innerNode.OuterHtml}");
                            // You can extract specific information from innerNode as needed
                        }

                        // Use a relative XPath based on the context of anchorElement
                        var nestedElement = anchorElement.SelectSingleNode(".//div/div[2]/div");

                        if (nestedElement != null)
                        {
                            var nestedText = nestedElement.InnerText;
                            date = nestedText;

                            // Print information about the nestedElement
                            Console.WriteLine($"  Found nestedElement with inner text '{nestedText}'");
                        }
                        else
                        {
                            Console.WriteLine("  No nestedElement found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No anchorElement found for href '{hrefValue}'.");
                    }

                    try
                    {
                        videosList.Add(await Videos.CreateAsync(hrefValue, date));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.Message}"); 
                    }
                    // Report progress for each video creation
                    processedCount++;
                    int percentage = (int)((double)processedCount / totalCount * 100);
                    overallProgress?.Report(percentage);
                }
               

               
            }

            return videosList;
        } 
        public async Task<List<Videos>> GetVideosAsync()
        {
            List<Videos> videosList = new List<Videos>();

            // Get the page source
            string pageSource = _driver.PageSource;

            // Load the page source into HtmlAgilityPack
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);


            var hrefAttributes = htmlDoc.DocumentNode.SelectNodes("//a[contains(@href, '/video/')]/@href");

            if (hrefAttributes != null)
            {

                int totalCount = hrefAttributes.Count;
                int processedCount = 0;

                foreach (var hrefAttribute in hrefAttributes)
                {
                    string hrefValue = hrefAttribute.GetAttributeValue("href", "");

                    // Extract text from elements with class "css-dennn6-DivTimeTag"
                    var anchorElement = htmlDoc.DocumentNode.SelectSingleNode($"//a[@href='{hrefValue}']");
                    var date = "xx/xx/xx";

                    if (anchorElement != null)
                    {
                        // Print information about the anchorElement
                        Console.WriteLine($"Found anchorElement with href '{hrefValue}':");

                        // Print information about all inner nodes of the anchorElement
                        foreach (var innerNode in anchorElement.ChildNodes)
                        {
                           // Console.WriteLine($"  InnerNode: {innerNode.OuterHtml}");
                            // You can extract specific information from innerNode as needed
                        }

                        // Use a relative XPath based on the context of anchorElement
                        var nestedElement = anchorElement.SelectSingleNode(".//div/div[2]/div");

                        if (nestedElement != null)
                        {
                            var nestedText = nestedElement.InnerText;
                            date = nestedText;

                            // Print information about the nestedElement
                            Console.WriteLine($"  Found nestedElement with inner text '{nestedText}'");
                        }
                        else
                        {
                            Console.WriteLine("  No nestedElement found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No anchorElement found for href '{hrefValue}'.");
                    }

                    try
                    {
                        videosList.Add(await Videos.CreateAsync(hrefValue, date));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.Message}"); 
                    }
                    // Report progress for each video creation
                    processedCount++;
                    int percentage = (int)((double)processedCount / totalCount * 100);
                    //overallProgress?.Report(percentage);
                }
               

               
            }

            return videosList;
        } 
        public async Task<List<Videos>> GetVideosAsync_old(IProgress<int> overallProgress = null)
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

                Console.WriteLine("hrefAttributes.Count: "+ hrefAttributes.Count);

                foreach (var hrefValue in hrefAttributes)
                {

                    // Extract text from elements with class "css-dennn6-DivTimeTag"
                    var anchorElement = htmlDoc.DocumentNode.SelectSingleNode($"//a[@href='{hrefValue}']");
                    var date = "xx/xx/xx";

                    if (anchorElement != null)
                    {
                        var nestedElement = anchorElement.SelectSingleNode(".//div/div[2]/div");


                        if (nestedElement != null)
                        {
                            var nestedText = nestedElement.InnerText;
                            date = nestedText;
                        }                      
                    }

                    videosList.Add(await Videos.CreateAsync(hrefValue.GetAttributeValue("href", ""),date));

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
