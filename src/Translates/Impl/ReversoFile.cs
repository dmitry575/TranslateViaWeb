using System;
using System.Collections.Generic;
using System.Threading;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Elements;

namespace TranslateViaWeb.Translates.Impl;

public class ReversoFile : BaseTranslateFile
{
    private readonly Random _random = new(3412);

    private readonly Dictionary<string, string> _mappingLanguages = new()
    {
        { "fr", "French" },
        { "en", "English" },
        { "sp", "Spanish" },
        { "es", "Spanish" },
        { "pt", "Portuguese" },
        { "it", "Italian" },
        { "du", "Dutch" },
        { "po", "Polish" },
        { "ru", "Russian" },
        { "de", "German" },
        { "zh", "Chinese" },
        { "cn", "Chinese" },
        { "ja", "Japanese" },
        { "ar", "Arabic" }
    };

    public ReversoFile(string filename, Configuration config) : base(filename, config)
    {
    }

    public override (string, bool) Translating(string text)
    {
        // select language

        Logger.Info($"set lang from: {Config.FromLang}");
        new ButtonWaiteElement(Driver, "//*[@class=\"language-switch\"]//div[@dl-test='translator-source']//button[@dl-test='translator-source-lang-btn']/div").Action(0);
        //new ButtonWaiteElement(_driver, "//*[@id=\"dl_translator\"]//div[@dl-test='translator-source']//button[@dl-test='translator-lang-option-" + _config.FromLang.ToLower() + "']").Action();
        try
        {
            Thread.Sleep(_random.Next(2, 3) * 1000);
            var buttonToLang = new ButtonWaiteElement(Driver,
                "//*[@id=\"dl_translator\"]//div[@dl-test='translator-source']//button[@dl-test='translator-lang-option-" + Config.FromLang.ToLower() + "']");
            buttonToLang.JavascriptExe("arguments[0].click();");
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

            //new ButtonWaiteElement(Driver, "//div[contains(@class,'resultText')]//div[@class=\"dropdown-menu open\"]//li[contains(text(), '" + _mappingLanguagesTo[Config.ToLang.ToLower()] + "')]").Action();
        }
        catch (Exception e)
        {
            Logger.Warn($"error on click button select to language: {e}");
        }

        return (null, false);
    }

    protected override bool LanguagesMapping()
    {
        if (!_mappingLanguages.ContainsKey(Config.FromLang.ToLower()))
        {
            Logger.Error($"translate Reverso do not support language: {Config.FromLang}");
            return false;
        }

        if (!_mappingLanguages.ContainsKey(Config.ToLang.ToLower()))
        {
            Logger.Error($"translate Reverso do not support language: {Config.ToLang}");
            return false;
        }

        return true;
    }

    protected override bool IsNeedRecreateDriver() => false;

    protected override int GetMaxSymbolsText()
    {
        return 1999;
    }

    protected override string GetUrlTranslate(string text = "")
    {
        return "https://www.reverso.net/text-translation#sl=eng";
    }

    protected override int GetId() => 8;
}