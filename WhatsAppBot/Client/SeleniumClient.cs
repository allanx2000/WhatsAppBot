using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppBot.Client
{
    public abstract class SeleniumClient
    {
        private readonly Properties.Settings Settings = Properties.Settings.Default;

        private ChromeDriver driver;
        
        protected ChromeDriver Driver
        {
            get
            {
                if (driver == null)
                {
                    string bin = Settings.ChromeBinary;
                    var fi = new FileInfo(bin);
                    var service = ChromeDriverService.CreateDefaultService(fi.DirectoryName);
                    service.HideCommandPromptWindow = true;

                    var options = new ChromeOptions()
                    {
                        AcceptInsecureCertificates = true,
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore,
                    };

                    if (!string.IsNullOrEmpty(bin))
                    {
                        options.BinaryLocation = bin;
                    }

                    if (Headless)
                        options.AddArgument("headless");

                    driver = new ChromeDriver(service, options);
                }

                return driver;
            }

        }

        private bool Headless = false;

        protected SeleniumClient(bool headless = false)
        {
            Headless = headless;
        }

        protected string GetCookieString(IWebDriver driver)
        {
            var cookies = driver.Manage().Cookies.AllCookies;
            return GetCookieString(cookies.ToList());
        }

        protected string GetCookieString(List<OpenQA.Selenium.Cookie> cookies)
        {
            return string.Join("; ", cookies.Select(c => string.Format("{0}={1}", c.Name, c.Value)));
        }

        public virtual void Close()
        {
            if (driver != null)
            {
                lock (driver)
                {
                    driver.Quit();
                }
            }
        }

    }
}
