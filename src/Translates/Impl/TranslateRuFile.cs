using System;
using System.Collections.Generic;
using System.Threading;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Elements;

namespace TranslateViaWeb.Translates.Impl
{
    public class TranslateRuFile : BaseTranslateFile
    {
        private readonly Random _random = new Random(1231);
        private readonly Dictionary<string, string> _mappingLanguagesTo = new Dictionary<string, string>
        {
            {"fr", "Французский"},
            {"en", "Английский"},
            {"sp", "Испанский"},
            {"es", "Испанский"},
            {"it", "Итальянский"},
            {"ru", "Русский"},
            {"de", "Немецкий"},
            {"zh", "Китайский"},
            {"ja", "Японский"},
        };

        private readonly Dictionary<string, string> _mappingLanguagesFrom = new Dictionary<string, string>
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

        public override (string, bool) Translating(string text)
        {
            
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

                new ButtonWaiteElement(Driver, "//div[@id='sourceTextBlock']//div[@class=\"dropdown-menu open\"][@role='combobox']//li[@data-original-index=\"" + _mappingLanguagesFrom[Config.FromLang.ToLower()] + "\"]").Action();
                
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

                new ButtonWaiteElement(Driver, "//div[contains(@class,'resultText')]//div[@class=\"dropdown-menu open\"]//li[contains(text(), '" + _mappingLanguagesTo[Config.ToLang.ToLower()] + "')]").Action();
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
                Logger.Error($"translate Translate.Ru do not support language: {Config.FromLang}");
                return false;
            }
            if (!_mappingLanguagesTo.ContainsKey(Config.ToLang.ToLower()))
            {
                Logger.Error($"translate Translate.Ru do not support language: {Config.ToLang}");
                return false;
            }

            return true;
        }

        protected override int GetMaxSymbolsText()
        {
            return 2999;
        }

        protected override string GetUrlTranslate(string text = "")
        {
            return $"https://www.translate.ru/";
        }

        protected override int GetId() => 2;

        protected override bool IsNeedRecreateDriver() => true;

    }
}
