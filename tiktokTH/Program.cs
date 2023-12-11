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
        chromeOptions.AddArgument("--headless");

        IWebDriver driver = new ChromeDriver(chromeOptions);

        // Navigate to TikTok
        driver.Navigate().GoToUrl("https://www.tiktok.com/explore");

        // Wait for the page to load (you may need to adjust the wait time)
        System.Threading.Thread.Sleep(5000);

        // Create an instance of TikTokScraper and use it to retrieve videos
        TikTokScraper tikTokScraper = new TikTokScraper(driver);
        List<Videos> videosList = tikTokScraper.GetVideos();


        // Asynchronously initialize the DownloadUrl for each video
        var initializeTasks = videosList.Select((Videos video) => Task.FromResult(video.DownloadUrl));

        // Track the total number of tasks
        int totalTasks = initializeTasks.Count();

        // Counter for completed tasks
        int completedTasks = 0;

        // Wait for all initialization tasks to complete
        await Task.WhenAll(initializeTasks.Select(async task =>
        {
            // Increment the completed tasks counter
            Interlocked.Increment(ref completedTasks);

            // Simulate some asynchronous work
            //await task;

            // Update the text-based progress bar for each task
            Console.WriteLine($"Initialization Progress for video {completedTasks}/{totalTasks}: [{new string('#', completedTasks)}{new string('-', totalTasks - completedTasks)}]");
        }));


        // Use the videosList as needed
        /*
        foreach (var video in videosList)
        {
            Console.WriteLine($"Username: {video.Username} \n" +
                $"Video URL: {video.VideoUrl}\n" +
                $"Video Title: {video.VideoTitle} \n" +
                $"Video Download Link: {video.DownloadUrl} \n");
        }
        */
        // Pause execution to observe the stored Videos instances
        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();

        // Close the browser
        driver.Quit();
    }

}



