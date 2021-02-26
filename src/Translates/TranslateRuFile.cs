using System;
using System.Collections.Generic;
using System.Threading;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Elements;

namespace TranslateViaWeb.Translates
{
    public class TranslateRuFile : BaseTranslateFile
    {
        private readonly Random _random = new Random(1231);
        private readonly Dictionary<string, string> _mappingLanguages = new Dictionary<string, string>
        {
            {"fr", "20"},
            {"en", "2"},
            {"sp", "6"},
            {"es", "6"},
            {"pt", "12"},
            {"it", "7"},
            {"ru", "13"},
            {"de", "11"},
            {"zh", "9"},
            {"ja", "22"},
        };

        private readonly int _categoryId = 3;
        public TranslateRuFile(string filename, Configuration config) : base(filename, config)
        {
        }
        protected override (string, bool) Translating(string text)
        {
            if (!_mappingLanguages.ContainsKey(Config.FromLang.ToLower()))
            {
                Logger.Error($"translate Translate.Ru do not support language: {Config.FromLang}");
                return (string.Empty, false);
            }
            if (!_mappingLanguages.ContainsKey(Config.ToLang.ToLower()))
            {
                Logger.Error($"translate Translate.Ru do not support language: {Config.ToLang}");
                return (string.Empty, false);
            }

            Logger.Info($"set category: Computer");
            new ButtonWaiteElement(Driver, "//*[@data-id=\"selDivTmpl\"]").Action();
            Thread.Sleep(_random.Next(2, 3) * 1000);

            new ButtonWaiteElement(Driver, "//div[@class=\"dropdown-menu open\"]//li[@data-original-index=\"" + _categoryId + "\"]").Action();

            // select language

            try
            {
                Logger.Info($"set lang from: {Config.FromLang}");
                new ButtonWaiteElement(Driver, "//*[@data-id=\"sLang\"]").Action();

                Thread.Sleep(_random.Next(2, 3) * 1000);
                new ButtonWaiteElement(Driver, "//div[@class=\"dropdown-menu open\"]//li[@data-original-index=\"" + _mappingLanguages[Config.FromLang.ToLower()] + "\"]//a").Action();
                //buttonToLang.JavascriptExe("arguments[0].click();");
            }
            catch (Exception e)
            {
                Logger.Warn($"error on click button select to language: {e}");
            }

            new InputElement(Driver, "//textarea[@id='sourceText']", text).Action();

            try
            {
                Logger.Info($"set lang to: {Config.ToLang}");
                new ButtonWaiteElement(Driver, "//button[@id=\"rLang\"]").Action();
                
                Thread.Sleep(_random.Next(2, 3) * 1000);

                new ButtonWaiteElement(Driver, "//div[@class=\"dropdown-menu open\"]//li[@data-original-index=\"" + _mappingLanguages[Config.ToLang.ToLower()] + "\"]//a").Action();
                //buttonToLang.JavascriptExe("arguments[0].click();");
            }
            catch (Exception e)
            {
                Logger.Warn($"error on click button select to language: {e}");
            }

            new ButtonWaiteElement(Driver, "//a[@id='bTranslation']").Action();

            Thread.Sleep(_random.Next(20, 25) * 1000);

            var resultElement = new InputElement(Driver, "//textarea[@id='editResult_test']", string.Empty);
            string result = resultElement.GetInnerText();

            if (string.IsNullOrEmpty(result))
            {
                Logger.Warn("not found result of translating");
                return (string.Empty, false);
            }

            return (result, true);
        }

        protected override int GetMaxSymbolsText()
        {
            return 2999;
        }

        protected override string GetUrlTranslate()
        {
            return $"https://www.translate.ru/";
        }

        protected override int GetId() => 2;

        protected override bool IsNeedRecreateDriver() => true;

    }
}
