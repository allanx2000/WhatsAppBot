using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace WhatsAppBot.Client
{
    public class WhatsAppClient : SeleniumClient
    {
        private IWebElement textBox;

        public WhatsAppClient()
        {
        }

        public const string BaseUrl = "https://web.whatsapp.com/";

        public void OpenLogin()
        {
            Driver.Navigate().GoToUrl(BaseUrl);
        }

        public void HookupUI()
        {
            textBox = Driver.FindElementByXPath("//div[@id='main']//div[contains(@class,'copyable-text selectable-text')]");

            if (textBox != null)
                Initialized = true;
        }

        private readonly Random random = new Random();

        public bool Initialized { get; private set; }

        //32-125
        public void SendRandomMessage()
        {
            StringBuilder sb = new StringBuilder();
            var length = random.Next(5, 100);
            for (int i = 0; i < length; i++)
            {
                sb.Append((char) random.Next(32, 126));
            }

            SendMessage(sb.ToString());
        }

        public void SendMessage(string message)
        {
            if (Initialized)
            {
                textBox.SendKeys(message);
                textBox.SendKeys(Keys.Return);
            }
        }
    }
}
