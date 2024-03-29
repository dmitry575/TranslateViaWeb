﻿using System;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using TranslateViaWeb.Common;

namespace TranslateViaWeb.Elements
{
    public class DownloadElement : BaseElementAction
    {
        /// <summary>
        /// How many seconds need wait a download link
        /// </summary>
        private readonly int _maxSecondsWaiting;

        /// <summary>
        /// Path where will be save files
        /// </summary>
        private readonly string _downloadPath;

        /// <summary>
        /// Url downloaded
        /// </summary>
        public string UrlDownload { get; private set; }

        /// <summary>
        /// Fullname file of download
        /// </summary>
        public string FileDownload { get; private set; }

        public DownloadElement(FirefoxDriver driver, string xpath, string downloadPath, int timeout) : base(driver, xpath)
        {
            _downloadPath = downloadPath;
            _maxSecondsWaiting = timeout;
        }

        /// <summary>
        /// Get full path of downloaded file
        /// </summary>
        /// <param name="linkElement">Url of downloaded file</param>
        private string GetFileName(IWebElement linkElement)
        {
            string href = linkElement.GetAttribute("href");
            if (string.IsNullOrEmpty(href))
            {
                Logger.Warn($"download link has not 'href' attribute");
                return string.Empty;
            }

            var fInfo = new FileInfo(href);
            return Path.Combine(_downloadPath, fInfo.Name);
        }

        public override void Action(int number)
        {
            //waiting while translating
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(_maxSecondsWaiting));
            IWebElement blockDownload = null;
            try
            {
                wait.PollingInterval = TimeSpan.FromSeconds(20);
                blockDownload = wait.Until(d => d.FindElement(By.CssSelector(Xpath)));
            }
            catch (Exception e)
            {
                Logger.Error($"not found element in time: {Xpath}, {e.Message}");
            }

            blockDownload ??= Driver.FindElement(By.CssSelector(Xpath));

            if (blockDownload == null)
            {
                Logger.Error($"not found element for download file by xpath: {Xpath}, after wait {_maxSecondsWaiting} seconds");
                throw new ElementActionException($"not found element for download file by xpath: {Xpath}");
            }
            var linkElement = blockDownload.FindElements(By.TagName("a")).FirstOrDefault();
            if (linkElement == null)
            {
                throw new ElementActionException($"not found tag a, with download file url in xpath: {Xpath}");
            }

            UrlDownload = linkElement.GetAttribute("href");
            try
            {
                Driver.ScrollToCenter(linkElement);
            }
            catch (Exception e)
            {
                Logger.Warn($"windows scroll before download failed: {e}");
            }

            Logger.Info($"try to click download file: {Xpath}");
            linkElement.Click();

            // check file downloaded, and rename
            FileDownload = GetFileName(linkElement);
            Logger.Info($"Downloaded file {Xpath}, to {FileDownload}");
        }
    }
}