﻿using System;
using System.Collections.Generic;
using System.Threading;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Elements;

namespace TranslateViaWeb.Translates.Impl
{
    /// <summary>
    /// Class for translate one file
    /// open url, upload file, download new file
    /// </summary>
    public class DeeplTranslateFile : BaseTranslateFile
    {
        private readonly Random _random = new(1231);

        private readonly Dictionary<string, string> _mappingLanguages = new()
        {
            { "fr", "fr-FR" },
            { "en", "en-GB" },
            { "sp", "es-ES" },
            { "es", "es-ES" },
            { "pt", "pt-PT" },
            { "br", "pt-BR" },
            { "it", "it-IT" },
            { "du", "nl-NL" },
            { "po", "pl-PL" },
            { "ru", "ru-RU" },
            { "de", "de-DE" },
            { "zh", "zh-ZH" },
            { "ja", "ja-JA" },
        };

        public DeeplTranslateFile(string filename, Configuration config) : base(filename, config)
        {
        }

        public override (string, bool) Translating(string text)
        {
            // select language

            Logger.Info($"set lang from: {Config.FromLang}");
            new ButtonWaiteElement(Driver, "//button[@dl-test='translator-source-lang-btn']").Action();
            try
            {
                Thread.Sleep(_random.Next(2, 3) * 1000);
                new ButtonWaiteElement(Driver, "//div[@dl-test='translator-source-lang-list']//button[@dl-test='translator-lang-option-" + Config.FromLang.ToLower() + "']").Action();
            }
            catch (Exception e)
            {
                Logger.Warn($"error on click button select to language: {e}");
            }


            Logger.Info($"set lang to: {Config.ToLang}");
            new ButtonWaiteElement(Driver, "//section[@dl-test='translator-target']//button[@dl-test='translator-target-lang-btn']").Action();

            try
            {
                Thread.Sleep(_random.Next(2, 3) * 1000);
                new ButtonWaiteElement(Driver, "//div[@dl-test='translator-target-lang-list']//button[@dl-test='translator-lang-option-" + _mappingLanguages[Config.ToLang.ToLower()] + "']").Action();
            }
            catch (Exception e)
            {
                Logger.Warn($"error on click button select to language: {e}");
            }

            // set translating text
            var textArea = new InputElement(Driver, "//textarea[@dl-test='translator-source-input']", text);
            textArea.Action();
            textArea.SendKey("\n");

            Thread.Sleep(_random.Next(20, 25) * 1000);

            var resultElement = new InputElement(Driver, "//section[@dl-test='translator-target']//textarea[@dl-test='translator-target-input']", string.Empty);
            var result = resultElement.GetAttribute("value");

            if (string.IsNullOrEmpty(result))
            {
                Logger.Warn("not found result of translating");
                return (string.Empty, false);
            }

            return (result.Trim(), true);
        }

        protected override bool LanguagesMapping()
        {
            if (!_mappingLanguages.ContainsKey(Config.FromLang.ToLower()))
            {
                Logger.Error($"translate deepl do not support language: {Config.FromLang}");
                return false;
            }

            if (!_mappingLanguages.ContainsKey(Config.ToLang.ToLower()))
            {
                Logger.Error($"translate deepl do not support language: {Config.ToLang}");
                return false;
            }

            return true;
        }

        protected override int GetMaxSymbolsText()
        {
            return 4999;
        }

        protected override string GetUrlTranslate(string text = "")
        {
            return $"https://www.deepl.com/{Config.FromLang}/translator";
        }

        protected override int GetId() => 0;


        protected override bool IsNeedRecreateDriver() => true;
    }
}