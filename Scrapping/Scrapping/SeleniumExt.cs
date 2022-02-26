using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Scrapping
{
    public static class SeleniumExt
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds <= 0)
            {
                return driver.FindElement(by);
            }

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));

            var element = wait.Until(drv =>
            {
                try
                {
                    return drv.FindElement(by);
                }
                catch (Exception)
                {
                    return null;
                }
            });

            return element!;
        }

        public static void WaitMs(this IWebDriver driver, double delay)
        {
            var now = DateTime.Now;
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(delay))
            {
                PollingInterval = TimeSpan.FromMilliseconds(delay)
            };
            wait.Until(_ => (DateTime.Now - now) - TimeSpan.FromMilliseconds(delay) > TimeSpan.Zero);
        }

        public static bool ExistElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            try
            {
                if (timeoutInSeconds <= 0)
                {
                    return false;
                }

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv =>
                {
                    try
                    {
                        return drv.FindElement(by, timeoutInSeconds).Displayed;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
                
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
