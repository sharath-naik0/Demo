using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace SeleniumTestProject
{
    [TestFixture]
    public class TestScenario2 : IDisposable
    {
        public static string LT_USERNAME = Environment.GetEnvironmentVariable("LT_USERNAME") ?? "sharatnaik109";
        public static string LT_ACCESS_KEY = Environment.GetEnvironmentVariable("LT_ACCESS_KEY") ?? "VVukNOCbsg66fAMjUvaktIm8wyu1kqAGUVpmnHrX2tKnaBHUY7";
        public static string seleniumUri = "https://hub.lambdatest.com/wd/hub";

        private readonly ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();

        public static IEnumerable<TestCaseData> BrowserAndOS()
        {
            yield return new TestCaseData("Chrome", "Windows 10");
            yield return new TestCaseData("Firefox", "Windows 10");
        }

        [SetUp]
        public void Init()
        {
            var browserOptions = new ChromeOptions();
            var ltOptions = new Dictionary<string, object>
            {
                { "username", LT_USERNAME },
                { "accessKey", LT_ACCESS_KEY },
                { "visual", true },
                { "build", "Test Scenario 2" },
                { "project", "Test Proj" },
                { "name", "Test Program" },
                { "selenium_version", "4.0.0" },
                { "w3c", true },
                { "networkLogs", true },
                { "video", true },
                { "consoleLogs", true },
                { "screenshots", true }
            };

            browserOptions.AddAdditionalOption("LT:Options", ltOptions);
            driver.Value = new RemoteWebDriver(new Uri(seleniumUri), browserOptions.ToCapabilities(), TimeSpan.FromSeconds(600));
        }

        [Test, TestCaseSource(nameof(BrowserAndOS)), Timeout(20000), Parallelizable(ParallelScope.All)]
        public void SliderDemo(string browser, string os)
        {
            driver.Value.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");
            WebDriverWait wait = new WebDriverWait(driver.Value, TimeSpan.FromSeconds(2));
            IWebElement dragLink = wait.Until(d => d.FindElement(By.LinkText("Drag & Drop Sliders")));
            dragLink.Click();

            IWebElement slider = wait.Until(d => d.FindElement(By.XPath("//input[@value='15']")));
            Actions action = new Actions(driver.Value);
            action.ClickAndHold(slider).MoveByOffset(215, 0).Release().Perform();

            IWebElement rangeValue = wait.Until(d => d.FindElement(By.Id("rangeSuccess")));
            string rangeText = rangeValue.Text;
            Assert.That(rangeText, Is.EqualTo("95"), "The range value does not show 95.");
        }

        [TearDown]
        public void Cleanup()
        {
            bool passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
            try
            {
                if (driver.Value != null)
                {
                    ((IJavaScriptExecutor)driver.Value).ExecuteScript("lambda-status=" + (passed ? "passed" : "failed"));
                }
            }
            finally
            {
                if (driver.Value != null)
                {
                    driver.Value.Quit();
                    driver.Value = null;
                }
            }
        }

        public void Dispose()
        {
            driver.Value?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
