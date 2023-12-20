using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using ShellProgressBar;
using tiktokTH;

internal class Program
{
    private static int completedTasks = 0;
    private static readonly object lockObject = new object();



    private static async Task Main()
    {

        //getVideos();

        //test();
        test1();

    }

    private static void getVideos()
    {
       
        try
        {
            IWebDriver driver = StartBrowser();

            driver.Navigate().GoToUrl("https://www.tiktok.com/search/video?lang=pt-BR&q=cr7");


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    private static IWebDriver StartBrowser()
    {
        // Set up Firefox options and driver
        FirefoxOptions options = new FirefoxOptions();
        // options.AddArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);
        //options.AddArgument("--headless");

        // Avoiding detection
        options.AddArgument("--disable-blink-features=AutomationControlled");

        options.Profile = new FirefoxProfile("C:\\Users\\costa\\AppData\\Roaming\\Mozilla\\Firefox\\Profiles\\pywfd4gy.default-release");
        IWebDriver driver = new FirefoxDriver(options);
        // options.AddArgument("--headless");

        return driver;
    }

    private static async void test1()
    {

        // Set up Firefox options and driver
        FirefoxOptions options = new FirefoxOptions();
        options.AddArgument("--user-agent=Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");


        // Avoiding detection
        options.AddArgument("--disable-blink-features=AutomationControlled");

        options.AddAdditionalOption("useAutomationExtension", false);
        //options.AddArgument("--headless");

       
        options.Profile = new FirefoxProfile("C:\\Users\\costa\\AppData\\Roaming\\Mozilla\\Firefox\\Profiles\\pywfd4gy.default-release");
        IWebDriver driver = new FirefoxDriver(options);
        // options.AddArgument("--headless");

        // Navigate to the TikTok search page
        driver.Navigate().GoToUrl("https://www.tiktok.com/login");

        try
        {/*
            // Wait for the element to be present
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement element = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[1]/div/div[2]/div/div/div/div[6]")));

            // Click on the element
            element.Click();

            // Wait for the new window or tab to open
            wait.Until(driver => driver.WindowHandles.Count > 1);

            // Switch to the new window or tab
            string originalWindowHandle = driver.CurrentWindowHandle;
            foreach (string windowHandle in driver.WindowHandles)
            {
                if (windowHandle != originalWindowHandle)
                {
                    driver.SwitchTo().Window(windowHandle);
                    break;
                }
            }

            // Wait for the email input field to be present
            WebDriverWait wait2 = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement emailInput = wait2.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[2]/div/form/fieldset[1]/div[1]/input")));

            // Input the email address
            emailInput.SendKeys("costagoncalo9@gmail.com");

            // Wait for the password input field to be present
            IWebElement passwordInput = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[2]/div/form/fieldset[1]/div[2]/input")));

            // Input the password
            passwordInput.SendKeys("patoseguro98");

            // Wait for the login button to be present
            IWebElement loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[2]/div/form/fieldset[2]/input[1]")));

            // Click on the login button
            loginButton.Click();

            // Perform actions on the new page

            // ...

            // Switch back to the original window
            driver.SwitchTo().Window(originalWindowHandle);

            // Now you are back in the original window
           */
            
            driver.Navigate().GoToUrl("https://www.tiktok.com/search/video?lang=pt-BR&q=cr7");

            //driver.Navigate().GoToUrl("https://www.tiktok.com/explore");

            Thread.Sleep(5000);

            // Set a flag to track whether the element is found
            bool isElementFound = false;

            // Set up a loop with a maximum number of attempts (to prevent infinite looping)
            int maxAttempts = 100000;
            int currentAttempt = 0;

            while (currentAttempt < maxAttempts && !isElementFound)
            {
                try
                {
                    // Attempt to find the element
                    IWebElement element = driver.FindElement(By.XPath("//*[@id=\"tabs-0-panel-search_video\"]/div/div[2]"));

                    // If the element is found, set the flag to true and break out of the loop
                    isElementFound = true;

                    // Now you can interact with the element
                    Console.WriteLine("Element found: " + element.Text);

                    

                }
                catch (NoSuchElementException)
                {
                    // If the element is not found, increment the attempt counter and wait before the next attempt
                    currentAttempt++;
                    System.Threading.Thread.Sleep(1000);
                    // For example, scroll down using JavaScript to load more content
                    
                        IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                        jsExecutor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");


                    // Find all elements with a class containing "css-1soki6-DivItemContainerForSearch"
                    ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.CssSelector("[class*='css-1soki6-DivItemContainerForSearch']"));

                    // Count and output the number of matching elements
                    Console.WriteLine($"Number of elements with the specified class: {elements.Count}");

                    // Wait for 1 second before the next attempt
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());    
                }
            }

            TikTokScraper tikTokScraper = new TikTokScraper(driver);

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

            Console.WriteLine($"Recent Videos:");


            foreach (var video in videosList)
            {
                // Calculate the difference in days between the current date and the video's date
                int daysDifference = (DateTime.Now - video.VideoDate).Days;

                // Check if the video is from the last 5 days
                if (daysDifference <= 5)
                {
                    Console.WriteLine($"Video Url: {video.VideoUrl} \n" +
                        $"Video Date: {video.VideoDate} \n" +
                        $"Video ID: {video.VideoId} \n" +
                        $"Download Link: {video.DownloadUrl} \n");
                }
            }

        }
        catch(Exception ex) 
        {
            Console.WriteLine(ex);

        }
        finally
        {
            // Close the WebDriver instance
            //driver.Quit();
        }
        Console.ReadLine();
    }

    private static async void  test()
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



