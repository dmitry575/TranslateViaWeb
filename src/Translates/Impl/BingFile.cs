using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Elements;

namespace TranslateViaWeb.Translates.Impl
{
    class BingFile : BaseTranslateFile
    {
        private readonly Random _random = new Random(1231);
        private readonly Dictionary<string, string> _mappingLanguagesTo = new Dictionary<string, string>
        {
            {"fr", "fr"},
            {"en", "en"},
            {"es", "es"},
            {"pt", "pt"},
            {"it", "it"},
            {"du", "nl"},
            {"nl", "nl"},
            {"po", "pl"},
            {"ru", "ru"},
            {"de","de"},
            {"zh", "zh"},
            {"ja", "ja"}
        };

        private readonly Dictionary<string, string> _mappingLanguagesFrom = new Dictionary<string, string>
        {
            {"fr", "fr"},
            {"en", "en"},
            {"es", "es"},
            {"pt", "pt"},
            {"it", "it"},
            {"du", "nl"},
            {"nl", "nl"},
            {"po", "pl"},
            {"ru", "ru"},
            {"de","de"},
            {"zh", "zh"},
            {"ja", "ja"}
        };
        public BingFile(string filename, Configuration config) : base(filename, config)
        {
        }

        protected override (string, bool) Translating(string text)
        {
            // select language

            Logger.Info($"set lang from: {Config.FromLang}");
            new ButtonWaiteElement(Driver, "//*[@id=\"tta_srcsl\"]").Action();
            try
            {
                Thread.Sleep(_random.Next(2, 3) * 1000);
              new ButtonWaiteElement(Driver, "//*[@id=\"tta_srcsl\"]//option[@value='"+Config.FromLang.ToLower()+"']").Action();
            }
            catch (Exception e)
            {
                Logger.Warn($"error on click button select to language: {e}");
            }

            Logger.Info($"set lang to: {Config.ToLang}");
            new ButtonWaiteElement(Driver, "//*[@id=\"tta_tgtsl\"]").Action();

            try
            {
                Thread.Sleep(_random.Next(2, 3) * 1000);
                new ButtonWaiteElement(Driver, "//*[@id=\"tta_tgtsl\"]//option[@value='" + Config.ToLang.ToLower()+"']").Action();
                
            }
            catch (Exception e)
            {
                Logger.Warn($"error on click button select to language: {e}");
            }

            new InputElement(Driver, "//textarea[@id='tta_input_ta']", text).Action();

            Thread.Sleep(_random.Next(20, 25) * 1000);

            var resultElement = new InputElement(Driver, "//textarea[@id='tta_output_ta']", string.Empty);
            string result = resultElement.GetAttribute("value");

            if (string.IsNullOrEmpty(result))
            {
                Logger.Warn("not found result of translating");
                return (string.Empty, false);
            }

            return (result, true);
        }

        protected override bool LanguagesMapping()
        {
            if (!_mappingLanguagesFrom.ContainsKey(Config.FromLang.ToLower()))
            {
                Logger.Error($"translate bing.com do not support language: {Config.FromLang}");
                return false;
            }
            if (!_mappingLanguagesTo.ContainsKey(Config.ToLang.ToLower()))
            {
                Logger.Error($"translate bing.com do not support language: {Config.ToLang}");
                return false;
            }

            return true;
        }

        protected override int GetMaxSymbolsText()
        {
            return 1999;
        }

        protected override string GetUrlTranslate()
        {
            return $"https://www.bing.com/translator";
        }

        protected override int GetId() => 5;


        protected override bool IsNeedRecreateDriver() => true;
    }
}
