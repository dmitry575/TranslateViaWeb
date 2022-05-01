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
        new ButtonWaiteElement(Driver, "//*[@class=\"selected-language\"]").Action();
        try
        {
            Thread.Sleep(_random.Next(2, 3) * 1000);
            new ButtonWaiteElement(Driver, "//*[@class=\"language-select-options\"]//div[contains(text(), '" + _mappingLanguagesFrom[Config.ToLang.ToLower()] + "')]").Action();
        }
        catch (Exception e)
        {
            Logger.Warn($"error on click button select to language: {e}");
        }

        new InputElement(Driver, "//div[@class=\"textarea__container\"]//textarea", text).Action();

        try
        {
            Logger.Info($"set lang to: {Config.ToLang}");
            new ButtonWaiteElement(Driver, "//button[@data-id=\"selected-language\"]").Action(1);

            Thread.Sleep(_random.Next(2, 3) * 1000);

            new ButtonWaiteElement(Driver, "//*[@class=\"language-select-options\"]//div[contains(text(), '" + _mappingLanguagesTo[Config.ToLang.ToLower()] + "')]").Action();
        }
        catch (Exception e)
        {
            Logger.Warn($"error on click button select to language: {e}");
        }

        // waiting translate
        Thread.Sleep(_random.Next(30, 35) * 1000);

        var resultElement = new InputElement(Driver, "//div[@class='sentence-wrapper sentence-wrapper_target ng-star-inserted']//span", string.Empty);
        string result = resultElement.GetInnerText();

        if (string.IsNullOrEmpty(result) || result.Length <= 4 || result == text)
        {
            Logger.Warn("not found result of translating");
            return (string.Empty, false);
        }

        return (result, true);
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