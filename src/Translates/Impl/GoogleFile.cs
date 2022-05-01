
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Elements;

namespace TranslateViaWeb.Translates.Impl
{
    public class GoogleFile : BaseTranslateFile
    {
        private readonly Random _random = new Random(1331);

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
        public GoogleFile(string filename, Configuration config) : base(filename, config)
        {
        }

        public override (string, bool) Translating(string text)
        {
            // select language

            Thread.Sleep(_random.Next(30, 35) * 1000);

            new ButtonWaiteElement(Driver, "//span[@data-language-to-translate-into='{Config.FromLang.ToLower()}']").Action();

            var resultElement = new InputElement(Driver, "//textarea[@id='tta_output_ta']", string.Empty);
            string result = resultElement.GetAttribute("value");

            if (string.IsNullOrEmpty(result) || result.Length <= 4 || result == text)
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
                Logger.Error($"translate {GetUrlTranslate()} do not support language: {Config.FromLang}");
                return false;
            }
            if (!_mappingLanguagesTo.ContainsKey(Config.ToLang.ToLower()))
            {
                Logger.Error($"translate {GetUrlTranslate()} do not support language: {Config.ToLang}");
                return false;
            }

            return true;
        }

        protected override bool IsNeedRecreateDriver() => true;
        protected override int GetMaxSymbolsText() => 1999;

        protected override string GetUrlTranslate(string text="")
        {
            return $"https://translate.google.com/?sl={Config.FromLang.ToLower()}&tl={Config.ToLang.ToLower()}&text={HttpUtility.HtmlEncode(text)}&op=translate";
        }

        protected override int GetId() => 6;
    }
}
