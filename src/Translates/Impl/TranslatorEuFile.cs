using System;
using System.Collections.Generic;
using System.Threading;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Elements;

namespace TranslateViaWeb.Translates.Impl
{
    public class TranslatorEuFile : BaseTranslateFile
    {
        private readonly Random _random = new Random(1231);
        private readonly Dictionary<string, string> _mappingLanguagesTo = new Dictionary<string, string>
        {
            {"fr", "french"},
            {"en", "english"},
            {"sp", "spanish"},
            {"es", "spanish"},
            {"pt", "portuguese&nbsp;EU"},
            {"br", "portuguese&nbsp;BR"},
            {"it", "italian"},
            {"du", "dutch"},
            {"po", "polish"},
            {"ru", "russian"},
            {"de","german"},
            {"zh", "chinese"},
            {"ja", "japanese"},
        };

        private readonly Dictionary<string, string> _mappingLanguagesFrom = new Dictionary<string, string>
        {
            {"fr", "fr"},
            {"en", "en"},
            {"sp", "es-ES"},
            {"es", "es"},
            {"pt", "pt"},
            {"br", "pt-br"},
            {"it", "it"},
            {"du", "nl"},
            {"po", "pl"},
            {"ru", "ru"},
            {"de","de"},
            {"zh", "zh-CHT"},
            {"ja", "ja"},
        };

        public TranslatorEuFile(string filename, Configuration config) : base(filename, config)
        {
        }

        public override (string, bool) Translating(string text)
        {
            // select language

            try
            {
                Thread.Sleep(_random.Next(2, 6) * 1000);
                Logger.Info($"set lang from: {Config.FromLang}");
                new ButtonWaiteElement(Driver, "//*[@id=\"vyber_jazyk_0\"]").Action();

                Thread.Sleep(_random.Next(7, 9) * 1000);

                new ButtonWaiteElement(Driver, "//div[@id='rozvinuto_vyber_jazyk_0']//div[@data-lang=\"" + _mappingLanguagesFrom[Config.FromLang.ToLower()] + "\"]").Action();
                new ButtonWaiteElement(Driver, "//div[@id='rozvinuto_vyber_jazyk_0']//div[@data-lang=\"" + _mappingLanguagesFrom[Config.FromLang.ToLower()] + "\"]").Action();

            }
            catch (Exception e)
            {
                Logger.Warn($"error on click button select to language: {e}");
                return (string.Empty,false);
            }

            new InputElement(Driver, "//textarea[@id='vstup_textarea']", text).Action();

            try
            {
                Thread.Sleep(_random.Next(2, 6) * 1000);
                Logger.Info($"set lang to: {Config.ToLang}");
                new ButtonWaiteElement(Driver, "//div[@id=\"vyber_jazyk_1\"]").Action();

                Thread.Sleep(_random.Next(2, 3) * 1000);


                new ButtonWaiteElement(Driver, "//div[@id='rozvinuto_vyber_jazyk_1']//div[contains(text(), '" + _mappingLanguagesTo[Config.ToLang.ToLower()] + "')]").Action();
                new ButtonWaiteElement(Driver, "//div[@id='rozvinuto_vyber_jazyk_1']//div[contains(text(), '" + _mappingLanguagesTo[Config.ToLang.ToLower()] + "')]").Action();
            }
            catch (Exception e)
            {
                Logger.Warn($"error on click button select to language: {e}");
                return (string.Empty, false);
            }
            Thread.Sleep(_random.Next(2, 6) * 1000);
            new ButtonWaiteElement(Driver, "//div[@id='prelozit']").Action();

            Thread.Sleep(_random.Next(20, 25) * 1000);

            var resultElement = new InputElement(Driver, "//textarea[@id='vystup_textarea']", string.Empty);
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
            return 999;
        }

        protected override string GetUrlTranslate(string text = "")
        {
            return "https://www.translator.eu/english/";
        }

        protected override int GetId() => 7;

        protected override bool IsNeedRecreateDriver() => true;
    }
}
