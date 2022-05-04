using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TranslateViaWeb.Configs;

namespace TranslateViaWeb.Translates
{
    public abstract class BaseTranslateFile : ITranslateFile, IDisposable
    {
        /// <summary>
        /// Minimal seconds for pause between action on form
        /// </summary>
        private const int MinPauseSeconds = 10;

        /// <summary>
        /// Maximal seconds for pause between action on form
        /// </summary>
        private const int MaxPauseSeconds = 20;

        /// <summary>
        /// Split in sentences
        /// </summary>
        private const string Delimitary = ".!?()-:;,";


        /// <summary>
        /// How many seconds need wait a load website
        /// </summary>
        private readonly int _maxSecondsWaiting;

        /// <summary>
        /// Filename fro translate
        /// </summary>
        private readonly string _filename;

        private bool _isOpenUrl = false;

        /// <summary>
        /// Configuration
        /// </summary>
        protected readonly Configuration Config;

        protected static readonly ILog Logger = LogManager.GetLogger(typeof(BaseTranslateFile));

        /// <summary>
        /// Selenium driver, use Chrome
        /// </summary>
        protected ChromeDriver Driver;

        /// <summary>
        /// Get random seconds for pause
        /// </summary>
        private readonly Random _random = new Random();

        /// <summary>
        /// Translate file
        /// </summary>
        /// <param name="filename">Filename for translate</param>
        /// <param name="config">Configuration translate</param>
        public BaseTranslateFile(string filename, Configuration config)
        {
            _filename = filename;
            Config = config;
            _maxSecondsWaiting = config.Timeout;
        }
        protected TimeSpan GetPause()
        {
            return TimeSpan.FromSeconds(_random.Next(MinPauseSeconds, MaxPauseSeconds));
        }

        /// <summary>
        /// Translate the file and download to new path
        /// </summary>
        public virtual bool Translate()
        {
            // check may be file already translated
            if (FileTranslatedExists())
            {
                Logger.Info($"translated file already exists: {_filename}");
                return true;
            }

            if (!LanguagesMapping())
            {
                return false;
            }

            Logger.Info($"starting translate file: {_filename}");

            try
            {
                // text maybe a big so need to split
                var content = GetContent();
                var texts = GetTexts(content, GetMaxSymbolsText());

                if (texts == null || texts.Count <= 0)
                {
                    Logger.Warn($"text for translating is empty");
                    return false;
                }
                var readyText = new StringBuilder(content.Length);

                // create browser
                CreateDriver();

                int countTexts = texts.Count;
                int i = 0;
                foreach (var text in texts)
                {
                    string ready;
                    OpenUrl(text);
                    try
                    {
                        (ready, var isTranslate) = Translating(text);
                        if (!isTranslate)
                        {
                            Logger.Warn($"file not translate: {_filename}");
                            return false;
                        }

                        Logger.Info($"path of file translated: {_filename}, {i} of {text.Length}");
                    }
                    catch (Exception e)
                    {
                        Logger.Warn($"Translate {i} block failed: {e}");
                        break;
                    }
                    
                    if (string.IsNullOrEmpty(ready) || ready == text)
                    {
                        Logger.Warn("Translate is empty or same");
                        continue;
                    }

                    readyText.Append(ready);
                    Logger.Info($"append to cache: {ready.Length} bytes");

                    if (countTexts > 1 && i < countTexts)
                    {
                        var secWait = new Random().Next(10, 20) * 1000;
                        Logger.Info($"sleep before next chunk: {secWait} ms");
                        Thread.Sleep(secWait);

                        if (IsNeedRecreateDriver())
                        {
                            Dispose();
                            CreateDriver();
                        }
                    }

                    i++;
                }

                if (readyText.Length > 0)
                {
                    // save result translate
                    var fileSaved = GetFileNameSave();

                    Save(fileSaved, readyText.ToString());

                    Logger.Info($"file translate and saved: {fileSaved}, {readyText.Length} bytes");
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Info($"unexpected exception in translate process: {e}");
                return false;
            }
        }

        public abstract (string, bool) Translating(string text);

        protected abstract bool LanguagesMapping();
        protected abstract bool IsNeedRecreateDriver();

        /// <summary>
        /// Save to file data
        /// </summary>
        /// <param name="fileName">Full filename</param>
        /// <param name="text">text for saving</param>
        protected virtual void Save(string fileName, string text)
        {
            try
            {
                if (!Directory.Exists(Config.DirOutput))
                {
                    Directory.CreateDirectory(Config.DirOutput);
                    Logger.Info($"directory created: {Config.DirOutput}");
                }
                
                File.WriteAllText(fileName, text);
            }
            catch (Exception e)
            {
                Logger.Error($"write to file failed: {fileName}, {e}");
            }
        }

        /// <summary>
        /// Get content for transalet from file
        /// </summary>
        /// <returns></returns>
        private string GetContent()
        {
            if (File.Exists(_filename))
            {
                return File.ReadAllText(_filename);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get text need to translating
        /// </summary>
        private List<string> GetTexts(string text, int maxLength)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrWhiteSpace(text))
            {
                return result;
            }

            int pos = 0;
            while (pos < text.Length)
            {
                // check is edge of text
                if ((text.Length - pos) < maxLength)
                {
                    result.Add(text.Substring(pos));
                    break;
                }

                // send end of text
                int end = pos + maxLength;
                for (; end > pos; end--)
                {
                    if (Delimitary.IndexOf(text[end]) > -1)
                        break;
                }
                // if do not find any split, set spit as space
                if (pos == end)
                {
                    end = pos + maxLength;
                    for (; end > pos; end--)
                    {
                        if (text[end] == ' ')
                            break;
                    }
                }

                if (pos == end)
                {
                    end = pos + maxLength;
                }
                result.Add(text.Substring(pos, end - pos + 1));
                pos = end + 1;
            }
            return result;

        }

        /// <summary>
        /// Max symbols to send to translate system
        /// </summary>
        protected abstract int GetMaxSymbolsText();

        /// <summary>
        /// Get url start tranlate
        /// </summary>
        protected abstract string GetUrlTranslate(string text = "");

        /// <summary>
        /// Get Id of translate
        /// </summary>
        protected abstract int GetId();

        /// <summary>
        /// Creating webrowser
        /// </summary>
        public virtual void CreateDriver()
        {
            var options = GetOptions(Config.DirOutput);
            // options.Profile = profile;
            Driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromSeconds(_maxSecondsWaiting));

            Driver.Manage().Window.Maximize();

            _isOpenUrl = false;
        }

        public virtual void OpenUrl(string text)
        {
            string url = GetUrlTranslate(text);

            if (_isOpenUrl)
            {
                Logger.Info($"url is already opened: {url}");
                return;
            }

            Logger.Info($"open url: {url}");
            try
            {
                Driver.Navigate().GoToUrl(url);
            }
            catch (WebDriverException e)
            {
                Logger.Error($"Open url failed: {e}");
            }
        }

        /// <summary>
        /// Checking translated file
        /// </summary>
        protected bool FileTranslatedExists()
        {
            return File.Exists(GetFileNameSave());
        }


        /// <summary>
        /// Get filename for saving transating text
        /// </summary>
        private string GetFileNameSave()
        {
            var fInfo = new FileInfo(_filename);
            // result translate file.from.to.extenstion

            var fileResultName =
                fInfo.Name.Substring(0, fInfo.Name.Length - fInfo.Extension.Length)
                + "."
                + Config.FromLang
                + "."
                + Config.ToLang
                + fInfo.Extension;

            return Path.Combine(Config.DirOutput, fileResultName);
        }

        /// <summary>
        /// Get options for FireFox
        /// </summary>
        /// <param name="dirOutput">Default directory for download files</param>
        private ChromeOptions GetOptions(string dirOutput)
        {
            var options = new ChromeOptions();
            options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36");
#if !DEBUG
            options.AddArgument("--headless");
#endif
            options.AddUserProfilePreference("download.default_directory", Path.GetFullPath(dirOutput));
            options.AddUserProfilePreference("intl.accept_languages", "en");
            options.AddUserProfilePreference("disable-popup-blocking", "true");
            if (!string.IsNullOrEmpty(Config.Proxy))
            {
                //Create a new proxy object
                var proxy = new Proxy
                {
                    //Set the http proxy value, host and port.
                    HttpProxy = Config.Proxy
                };

                //Set the proxy to the Chrome options
                options.Proxy = proxy;
            }

            return options;
        }


        /// <summary>
        /// Destroy
        /// </summary>
        public void Dispose()
        {
            if (Driver != null)
            {
                Driver.Close();
                Driver.Quit();
                Driver.Dispose();
            }
        }
    }
}
