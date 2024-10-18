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
    public class TestScenario3 : IDisposable
    {
        private static readonly string LT_USERNAME = Environment.GetEnvironmentVariable("LT_USERNAME") ?? "sharatnaik109";
        private static readonly string LT_ACCESS_KEY = Environment.GetEnvironmentVariable("LT_ACCESS_KEY") ?? "VVukNOCbsg66fAMjUvaktIm8wyu1kqAGUVpmnHrX2tKnaBHUY7";
        private static readonly string seleniumUri = "https://hub.lambdatest.com/wd/hub";

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
                { "build", "Test Scenario 3" },
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
            driver.Value = new RemoteWebDriver(new Uri(seleniumUri), browserOptions.ToCapabilities());
        }

        [Test, TestCaseSource(nameof(BrowserAndOS)), Timeout(30000), Parallelizable(ParallelScope.All)]
        public void InputForm(string browser, string os)
        {
            driver.Value.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");
            driver.Value.FindElement(By.LinkText("Input Form Submit")).Click();

            driver.Value.FindElement(By.XPath("//button[normalize-space()='Submit']")).Click();

            // Validate empty form submission
            WebDriverWait wait = new WebDriverWait(driver.Value, TimeSpan.FromSeconds(1));
            IWebElement errorMessageElement = wait.Until(d => d.FindElement(By.CssSelector("input:invalid")));
            string errorMessage = errorMessageElement.GetAttribute("validationMessage");
            Assert.That(errorMessage, Is.EqualTo("Please fill out this field."), "Error message not as expected.");

            // Fill in the form
            driver.Value.FindElement(By.Id("name")).SendKeys("Athik Rehaman");
            driver.Value.FindElement(By.Id("inputEmail4")).SendKeys("athikrehman65.ar@gmail.com");
            driver.Value.FindElement(By.Id("inputPassword4")).SendKeys("Athik123");
            driver.Value.FindElement(By.Id("company")).SendKeys("EGDK India");
            driver.Value.FindElement(By.Id("websitename")).SendKeys("Lambda.com");

            var countryDropdown = new SelectElement(driver.Value.FindElement(By.Name("country")));
            countryDropdown.SelectByText("India");

            driver.Value.FindElement(By.Id("inputCity")).SendKeys("Mangalore");
            driver.Value.FindElement(By.Id("inputAddress1")).SendKeys("Kapikad");
            driver.Value.FindElement(By.Id("inputAddress2")).SendKeys("Bejai");
            driver.Value.FindElement(By.Id("inputState")).SendKeys("Karnataka");
            driver.Value.FindElement(By.Id("inputZip")).SendKeys("576003");

            driver.Value.FindElement(By.XPath("//button[normalize-space()='Submit']")).Click();
            IWebElement successMessageElement = wait.Until(d => d.FindElement(By.XPath("//p[@class='success-msg hidden']")));
            string successMessage = successMessageElement.Text;
            Assert.That(successMessage, Is.EqualTo("Thanks for contacting us, we will get back to you shortly."), "Success message not as expected.");
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
                driver.Value?.Quit();
                driver.Value = null;
            }
        }

        public void Dispose()
        {
            driver.Value?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
