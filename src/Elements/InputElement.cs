using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TranslateViaWeb.Elements
{
    class InputElement : BaseElementAction
    {
        private readonly string _value;

        public InputElement(WebDriver driver, string xpath, string value) : base(driver, xpath)
        {
            _value = value;
        }
       
        public override void Action(int number)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30))
            {
                PollingInterval = TimeSpan.FromSeconds(20)
            };

            try
            {
                var element = wait.Until(d => d.FindElement(By.XPath(Xpath)));
                Logger.Info($"try find xpath: {Xpath}, {nameof(InputElement)}");

                if (element == null)
                {
                    Logger.Warn($"Not found element by xpath: {Xpath}, {nameof(SelectedElement)}");
                    return;
                }
                element.Click();
                Thread.Sleep(1000);
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].value = arguments[1]", element, _value);

            }
            catch (Exception e)
            {
                Logger.Error($"failed set value to {Xpath}, value: {_value}. {e}");
            }
        }
    }
}
