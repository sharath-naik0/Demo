using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTestProject
{
    [TestFixture]
    public class TestScenario1 : IDisposable
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
                { "build", "Test Scenario 1" },
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
        public void SimpleForm(string browser, string os)
        {
            driver.Value.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");
            driver.Value.FindElement(By.LinkText("Simple Form Demo")).Click();
            Assert.IsTrue(driver.Value.Url.Contains("simple-form-demo"), "URL does not contain 'simple-form-demo'.");

            string message = "Welcome to LambdaTest";
            driver.Value.FindElement(By.Id("user-message")).SendKeys(message);
            driver.Value.FindElement(By.Id("showInput")).Click();

            string displayedMessage = driver.Value.FindElement(By.Id("message")).Text;
            Assert.That(displayedMessage, Is.EqualTo(message), "The displayed message does not match the input message.");
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
