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
}



