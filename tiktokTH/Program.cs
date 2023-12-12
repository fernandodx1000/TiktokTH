using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ShellProgressBar;
using tiktokTH;

internal class Program
{
    private static int completedTasks = 0;
    private static readonly object lockObject = new object();

    private static async Task Main()
    {
        // Set up Chrome options to run in headless mode
        var chromeOptions = new ChromeOptions();
        //chromeOptions.AddArgument("--headless");

        IWebDriver driver = new ChromeDriver(chromeOptions);

        // Navigate to TikTok
        driver.Navigate().GoToUrl("https://www.tiktok.com/explore");

        // Wait for the page to load (you may need to adjust the wait time)
        System.Threading.Thread.Sleep(5000);

        // Create an instance of TikTokScraper and use it to retrieve videos
        TikTokScraper tikTokScraper = new TikTokScraper(driver);
        // List<Videos> videosList =  tikTokScraper.GetVideos();
        //List<Videos> videosList = await tikTokScraper.GetVideosAsync();
        List<Videos> videosList;

        // Create a progress bar for overall progress
        var overallProgressOptions = new ProgressBarOptions
        {
            ProgressCharacter = '#',
            ProgressBarOnBottom = true
        };

        using (var overallProgressBar = new ProgressBar(100, "Overall progress...", overallProgressOptions))
        {
            // Subscribe to the overall progress event
            var overallProgress = new Progress<int>(percentage =>
            {
                // Update the overall progress bar
                overallProgressBar.Tick(percentage, $"Overall progress... {percentage}% complete");
            });

            // Retrieve videos with overall progress reporting
            videosList = await tikTokScraper.GetVideosAsync(overallProgress);
        }



        foreach (var video in videosList)
            {
                Console.WriteLine($"Username: {video.Username} \n" +
                    $"Video URL: {video.VideoUrl}\n" +
                    $"Video Title: {video.VideoTitle} \n" +
                    $"Video Download Link: {video.DownloadUrl} \n");
            }


            // Pause execution to observe the stored Videos instances
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            // Close the browser
            driver.Quit();
     }
    
}



