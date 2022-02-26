using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Scrapping.Models;

namespace Scrapping
{
    public  class WebServerExport : IDisposable
    {

        private SeleniumConfig _config;
        private IWebDriver _driver;


        public WebServerExport(SeleniumConfig config)
        {

             this._config = config;
             ChromeOptions optionsFF = new ChromeOptions();
             var chromeDriverService = ChromeDriverService.CreateDefaultService(AppContext.BaseDirectory);


            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;


            optionsFF.AddArgument("--start-maximized");
            optionsFF.AddArgument("--disable-gpu");
            optionsFF.AddArgument("--disable-extensions");
            optionsFF.AddArgument("--window-size=1920,1080");


            optionsFF.AddUserProfilePreference("download.default_directory", AppContext.BaseDirectory);


            if (_config.Headless == 1)
            {
                optionsFF.AddArgument("--headless");
            }

            _driver = new ChromeDriver(chromeDriverService, optionsFF);


            _driver.Navigate().GoToUrl(_config.UrlPagina);
        }


        public void InitExport()
        {

            ClickFetch(); 


            ClickNextPage();
            ClickNextPage();
            ClickPreviousPage();            



            ClickFetchAlert();
            ClickNextPage(true);
            ClickPreviousPage(true);


            ClickFetchDownload();
            ClickDownload();


            ClickFetchLogin();


            SendAuthentication();
            
            Console.WriteLine("Press enter after resolve the captcha and do the login.");
            Console.ReadLine();
            ClickNextPage();
            ClickNextPage();
            ClickPreviousPage();



            ClickFetch();

            GetInfos();
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
        }


        public void ClickFetchAlert()
        {
            XPathClick("//*[@id=\"app\"]/div/div/div[2]/nav/div[4]/a");
        }


        public void ClickAlert()
        {
            _driver.SwitchTo().Alert().Accept();
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
            XPathClick("//button[contains(text(),'Proxima')]");
            if (hasAlert)
            {
                ClickAlert();
            }
        }


        public void ClickPreviousPage(bool hasAlert = false)
        {            
            XPathClick("//button[contains(text(),'Anterior')]");

            if (hasAlert)
            { 
                ClickAlert();
            }
        }


        public void ClickDownload()
        {
            XPathClick("//*[@id=\"app\"]/div/main/article/a");
        }


        public void SendAuthentication()
        {
            var email = _driver.FindElement(By.XPath("//*[@id=\"typeEmailX\"]"), _config.Timeout);
            var password = _driver.FindElement(By.XPath("//*[@id=\"typePasswordX\"]"), _config.Timeout);

            email.SendKeys(_config.Username);
            password.SendKeys(_config.Password);

        }


        public void GetInfos()
        {
            var loop = true;
            const string buttonName = "Proxima";

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
            _driver.WaitMS(_config.Timeout);
        }

    }
}
