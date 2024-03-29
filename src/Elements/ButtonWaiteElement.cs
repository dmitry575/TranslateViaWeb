﻿using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TranslateViaWeb.Common;

namespace TranslateViaWeb.Elements;

public class ButtonWaiteElement : BaseElementAction
{
    private const int MaxClick = 3;
    
    private readonly int _tryClick = 0;

    public ButtonWaiteElement(IWebDriver driver, string xpath) : base(driver, xpath)
    {
    }

    public override void Action(int number)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30))
        {
            PollingInterval = TimeSpan.FromSeconds(20)
        };

        var buttons = wait.Until(d => d.FindElements(By.XPath(Xpath)));
        Logger.Info($"try find xpath: {Xpath}, {nameof(ButtonWaiteElement)}");


        if (buttons == null || buttons.Count == 0)
        {
            buttons = Driver.FindElements(By.XPath(Xpath));
            if (buttons == null)
            {
                Logger.Warn($"Not found element by xpath: {Xpath}, {nameof(ButtonWaiteElement)}");
                return;
            }
        }

        var i = -1;
        foreach (var button in buttons)
        {
            i++;
            if (i != number)
            {
                continue;
            }

            var display = button.GetCssValue("display");
            Logger.Info($"button display: {display}");

            if ((button.Displayed && button.Enabled) || _tryClick > MaxClick)
            {
                // try to scroll to button element
                try
                {
                    Driver.ScrollToCenter(button);
                }
                catch (Exception e)
                {
                    Logger.Error($"failed scroll to button {Xpath}. {e}");
                }

                Thread.Sleep(5);

                button.Click();
                return;
            }
        }
    }
}