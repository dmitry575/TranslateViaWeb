using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Elements;

namespace TranslateViaWeb.Translates.Impl
{
    /// <summary>
    /// Translate through  https://translate.systran.net/translationTools/text
    /// </summary>
    public class SystranNetFile : BaseTranslateFile
    {
        private readonly Random _random = new Random(456456);

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
        public SystranNetFile(string filename, Configuration config) : base(filename, config)
        {
        }

        protected override (string, bool) Translating(string text)
        {
            // select language

            try
            {
                Logger.Info($"set lang from: {Config.FromLang}");
                new ButtonWaiteElement(Driver, "//div[contains(@class,'sourceLanguageDiv')//div[contains(@class,'Select-arrow-zone')]").Action();

                Thread.Sleep(_random.Next(2, 3) * 1000);

                new ButtonWaiteElement(Driver, "//div[contains(@class,'targetLanguageDiv')]//div[contains(@class,'Select-arrow-zone')]").Action();

            }
            catch (Exception e)
            {
                Logger.Warn($"error on click button select to language: {e}");
            }

            new InputElement(Driver, "//textarea[@id='sourceText']", text).Action();

            try
            {
                Logger.Info($"set lang to: {Config.ToLang}");
                new ButtonWaiteElement(Driver, "//button[@data-id=\"rLang\"]").Action();

                Thread.Sleep(_random.Next(2, 3) * 1000);

                //new ButtonWaiteElement(Driver, "//div[contains(@class,'resultText')]//div[@class=\"dropdown-menu open\"]//li[@data-original-index=\"" + _mappingLanguagesTo[Config.ToLang.ToLower()] + "\"]").Action();
                new ButtonWaiteElement(Driver, "//div[contains(@class,'resultText')]//div[@class=\"dropdown-menu open\"]//li[contains(text(), '" + _mappingLanguagesTo[Config.ToLang.ToLower()] + "')]").Action();
                //buttonToLang.JavascriptExe("arguments[0].click();");
            }
            catch (Exception e)
            {
                Logger.Warn($"error on click button select to language: {e}");
            }

            new ButtonWaiteElement(Driver, "//a[@id='bTranslation']").Action();

            Thread.Sleep(_random.Next(20, 25) * 1000);

            var resultElement = new InputElement(Driver, "//textarea[@id='editResult_test']", string.Empty);
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
                Logger.Error($"translate translate.systran.net do not support language: {Config.FromLang}");
                return false;
            }
            if (!_mappingLanguagesTo.ContainsKey(Config.ToLang.ToLower()))
            {
                Logger.Error($"translate translate.systran.net do not support language: {Config.ToLang}");
                return false;
            }

            return true;
        }
        protected override bool IsNeedRecreateDriver() => true;

        protected override int GetMaxSymbolsText() => 1999;

        protected override string GetUrlTranslate() => "https://translate.systran.net/translationTools/text";

        protected override int GetId() => 4;
    }
}
