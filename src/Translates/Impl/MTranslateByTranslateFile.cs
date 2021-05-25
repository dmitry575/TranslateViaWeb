using System;
using System.Collections.Generic;
using System.Threading;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Elements;

namespace TranslateViaWeb.Translates.Impl
{
    public class MTranslateByTranslateFile : BaseTranslateFile
    {
        private readonly Random _random = new Random(56789);

        private List<string> _webSites = new List<string>
        {
            "https://www.m-translate.by/#",
            "https://www.m-translate.com.ua/#",
            //"https://www.m-translate.com/#",
            "https://www.m-translate.ru/#"

        };

        public MTranslateByTranslateFile(string filename, Configuration config) : base(filename, config)
        {
        }

        protected override (string, bool) Translating(string text)
        {

            // insert text for translate
            new InputElement(Driver, "//div[@id='trn_from_cnt']//textarea[@id='text']", text).Action();

            Logger.Info($"set lang from: {Config.FromLang}");
            new ButtonWaiteElement(Driver, "//div[@id='trn_from_cnt']//div[@id='from_btn']/span").Action();
            Thread.Sleep(_random.Next(1, 2) * 1000);
            new ButtonWaiteElement(Driver, "//div[@id='jbox_1']//span[@class='select2-selection__arrow']").Action();
            Thread.Sleep(_random.Next(1, 2) * 1000);
            new ButtonWaiteElement(Driver, "//span[contains(@class,'select2-container--open')]//li[contains(@id,'-" + Config.FromLang.ToLower() + "')]").Action();

            Logger.Info($"set lang to: {Config.ToLang}");
            new ButtonWaiteElement(Driver, "//div[@id='trn_from_cnt']//div[@id='to_btn']/span").Action();

            Thread.Sleep(_random.Next(1, 2) * 1000);
            new ButtonWaiteElement(Driver, "//div[@id='jbox_2']//span[@class='select2-selection__arrow']").Action();

            Thread.Sleep(_random.Next(1, 2) * 1000);
            new ButtonWaiteElement(Driver, "//ul[@id='select2-translate_to-results']//li[contains(@id,'-" + Config.ToLang.ToLower() + "')]").Action();

            Thread.Sleep(_random.Next(1, 2) * 1000);
            new ButtonWaiteElement(Driver, "//div[@id='trn_from_cnt']//div[@id='go_btn']").Action();
            Thread.Sleep(_random.Next(10, 20) * 1000);

            var resultElement = new InputElement(Driver, "//div[contains(@class,'translate_from_cnt')]//textarea[@id='text_out']", string.Empty);
            string result = resultElement.GetAttribute("value");

            if (string.IsNullOrEmpty(result))
            {
                Logger.Warn("not found result of translating");
                return (string.Empty, false);
            }

            return (result, true);
        }

        protected override bool LanguagesMapping() => true;

        protected override int GetMaxSymbolsText()
        {
            return 4999;
        }

        protected override string GetUrlTranslate(string text = "")
        {
            return _webSites[_random.Next(0, _webSites.Count - 1)];
        }

        protected override int GetId() => 1;

        protected override bool IsNeedRecreateDriver() => false;
    }
}
