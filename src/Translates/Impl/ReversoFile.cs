using System;
using System.Collections.Generic;
using System.Threading;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Elements;

namespace TranslateViaWeb.Translates.Impl;

public class ReversoFile : BaseTranslateFile
{
    private readonly Random _random = new(3412);

    private readonly Dictionary<string, string> _mappingLanguagesFrom = new()
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

    private readonly Dictionary<string, string> _mappingLanguagesTo = new()
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
        var inputText = new InputElement(Driver, "//div[@class=\"textarea__container\"]//textarea", text);
        inputText.Action();
        inputText.SendKey("\n");

        // select language

        Logger.Info($"set lang from: {Config.FromLang}");
        new ButtonWaiteElement(Driver, "//*[@class=\"selected-language\"]").Action();
        try
        {
            Thread.Sleep(_random.Next(2, 3) * 1000);
            new ButtonWaiteElement(Driver, "//*[@class=\"language-select-options\"]//span[contains(text(), '" + _mappingLanguagesFrom[Config.FromLang.ToLower()] + "')]").Action();
        }
        catch (Exception e)
        {
            Logger.Warn($"error on click button select to language: {e}");
        }

        try
        {
            Logger.Info($"set lang to: {Config.ToLang}");
            new ButtonWaiteElement(Driver, "//*[@class=\"selected-language\"]").Action(1);

            Thread.Sleep(_random.Next(2, 3) * 1000);

            new ButtonWaiteElement(Driver, "//*[@class=\"language-select-options\"]//span[contains(text(), '" + _mappingLanguagesTo[Config.ToLang.ToLower()] + "')]").Action(1);
        }
        catch (Exception e)
        {
            Logger.Warn($"error on click button select to language: {e}");
        }

        // waiting translate
        Thread.Sleep(_random.Next(30, 35) * 1000);

        var resultElement = new InputElement(Driver, "//*[contains(@class,'translation-input__result')]//span", string.Empty);
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
        if (!_mappingLanguagesFrom.ContainsKey(Config.FromLang.ToLower()))
        {
            Logger.Error($"translate Reverso do not support language: {Config.FromLang}");
            return false;
        }

        if (!_mappingLanguagesTo.ContainsKey(Config.ToLang.ToLower()))
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