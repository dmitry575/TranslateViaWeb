using System;
using log4net;
using OpenQA.Selenium;

namespace TranslateViaWeb.Elements
{
    public abstract class BaseElementAction : IElementAction
    {
        protected readonly IWebDriver Driver;
        protected readonly string Xpath;
        protected readonly ILog Logger = LogManager.GetLogger(typeof(BaseElementAction));


        protected BaseElementAction(IWebDriver driver, string xpath)
        {
            Driver = driver;
            Xpath = xpath;
        }
        public abstract void Action(int number = 0);

        public virtual string GetAttribute(string name)
        {
            try
            {
                var element = Driver.FindElement(By.XPath(Xpath));
                if (element == null)
                {
                    Logger.Warn($"Not found element by xpath: {Xpath}, {nameof(SelectedElement)}");
                    return string.Empty;
                }

                return element.GetAttribute(name);

            }
            catch (Exception e)
            {
                Logger.Error($"failed get attribute {Xpath}. {e}");
            }
            return string.Empty;
        }

        public virtual void JavascriptExe(string script)
        {
            var element = Driver.FindElement(By.XPath(Xpath));
            ((IJavaScriptExecutor)Driver).ExecuteScript(script, element);
        }

        public virtual string GetInnerText()
        {
            var element = Driver.FindElement(By.XPath(Xpath));
            if (element != null)
            {
                return element.Text.Trim();
            }
            return string.Empty;
        }
    }
}
