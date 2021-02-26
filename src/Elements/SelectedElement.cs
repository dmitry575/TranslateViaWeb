using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using TranslateViaWeb.Common;

namespace TranslateViaWeb.Elements
{
    public class SelectedElement : BaseElementAction
    {
        private readonly string _value;

        public SelectedElement(RemoteWebDriver driver, string xpath, string value) : base(driver, xpath)
        {
            _value = value;
        }
        public override void Action()
        {
            var select = Driver.FindElement(By.XPath(Xpath));

            if (select == null)
            {
                Logger.Warn($"Not found element by xpath: {Xpath}, {nameof(SelectedElement)}");
                return;
            }

            var selectElement = new SelectElement(select);

            try
            {
                selectElement.SelectByValue(_value);
            }
            catch (Exception e)
            {
                Logger.Error($"failed set value to {Xpath}, value: {_value}. {e}");
            }
        }
    }
}
