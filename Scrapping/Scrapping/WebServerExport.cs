using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using Scrapping.Models;

namespace Scrapping
{
    public  class WebServerExport : IDisposable
    {

        private readonly SeleniumConfig _config;
        private readonly Actions _actions;
        private IWebDriver _driver;


        public WebServerExport(SeleniumConfig config)
        {

             _config = config;
             var optionsFf = new ChromeOptions();
             var chromeDriverService = ChromeDriverService.CreateDefaultService(AppContext.BaseDirectory);


            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;


            optionsFf.AddArgument("--start-maximized");
            optionsFf.AddArgument("--disable-gpu");
            optionsFf.AddArgument("--disable-extensions");
            optionsFf.AddArgument("--window-size=1920,1080");


            optionsFf.AddUserProfilePreference("download.default_directory", AppContext.BaseDirectory);


            if (_config.Headless == 1)
            {
                optionsFf.AddArgument("--headless");
            }

            _driver = new ChromeDriver(chromeDriverService, optionsFf);


            _driver.Navigate().GoToUrl(_config.UrlPage);

            _actions = new Actions(_driver);
        }

        public void FetchDataPage()
        {
            ClickFetch();

            GetInfos();
        }

        public void FetchDataAlertPage()
        {
            ClickFetchAlert();
            ClickNextPage(true);
            ClickPreviousPage(true);
        }

        public void FetchDataDownloadPage()
        {
            ClickFetchDownload();
            ClickDownload();
        }

        public void HomePage()
        {
            ClickHome();
        }

        public void CounterPage()
        {
            ClickCounter();

            for (var i = 0; i <= 10; i++)
            {
                XPathClick("//button[contains(text(),'Click me')]");
            }
        }

        public void InitExport()
        {
            FetchDataAlertPage();

            FetchDataDownloadPage();

            CounterPage();

            FetchDataPage();

            HomePage();

            Console.ReadLine();

        }


        public void Dispose()
        { 
            _driver.Quit();
            _driver = null;
        }


        public void ClickFetch()
        {
            XPathClick("//*[@id=\"app\"]/div/div/div[2]/nav/div[3]/a");

            ClickNextPage();
            ClickNextPage();
            ClickPreviousPage();
        }


        public void ClickFetchAlert()
        {
            XPathClick("//*[@id=\"app\"]/div/div/div[2]/nav/div[4]/a");
        }


        public void ClickAlert()
        {
            _driver.SwitchTo().Alert().Accept();
        }

        public void ClickHome()
        {
            XPathClick("//*[@id=\"app\"]/div/div/div[2]/nav/div[1]/a");
        }

        public void ClickCounter()
        {
            XPathClick("//*[@id=\"app\"]/div/div/div[2]/nav/div[2]/a");
        }

  
        public void ClickFetchDownload()
        {
            XPathClick("//*[@id=\"app\"]/div/div/div[2]/nav/div[5]/a");
        }


        public void ClickFetchLogin()
        {
            XPathClick("//*[@id=\"app\"]/div/div/div[2]/nav/div[6]/a");
        }

        public void ClickNextPage(bool hasAlert = false)
        {
            XPathClick("//button[contains(text(),'Next')]");
            if (hasAlert)
            {
                ClickAlert();
            }
        }


        public void ClickPreviousPage(bool hasAlert = false)
        {            
            XPathClick("//button[contains(text(),'Previous')]");

            if (hasAlert)
            { 
                ClickAlert();
            }
        }


        public void ClickDownload()
        {
            XPathClick("//*[@id=\"app\"]/div/main/article/a");
        }

        public void GetInfos()
        {
            var loop = true;
            const string buttonName = "Next";

            try
            {
                while (loop)
                {   
                    var table = _driver.FindElement(By.XPath("//*[@id=\"app\"]/div/main/article/table/tbody"), _config.Timeout);
                    var maxTr = table.FindElements(By.XPath("//tbody//tr")).Count;
                    
                    for (var j = 1; j <= maxTr; j++)
                    {
                        Console.WriteLine(
                              table.FindElement(By.XPath($"//tr[{j}]/td[1]")).Text + "\t" +
                              table.FindElement(By.XPath($"//tr[{j}]/td[2]")).Text + "\t" +
                              table.FindElement(By.XPath($"//tr[{j}]/td[3]")).Text + "\t" +
                              table.FindElement(By.XPath($"//tr[{j}]/td[4]")).Text);
                    }

                    if (!_driver.ExistElement(By.XPath($"//button[contains(text(),${buttonName})]"), _config.Timeout))
                    {
                        loop = false;
                    }

                    ClickNextPage();

                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Element ${buttonName} not found!");
            }            
        }


        public void XPathClick(string xpath,bool generateThrow = true)
        {
            try
            {
                _driver.FindElement(By.XPath(xpath), _config.Timeout).Click();
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"Element not found: ${xpath}");

                if (generateThrow)
                {
                    throw new TimeoutException($"Element not found: ${xpath}");
                }
            }
            _driver.WaitMs(_config.Timeout);
        }

    }
}
