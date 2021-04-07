using System;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Translates.Impl;

namespace TranslateViaWeb.Translates
{
    public static class TranslateFileFactory
    {
        public static BaseTranslateFile Create(Type type, string filename, Configuration config)
        {
            switch (type)
            {
                case { } intType when intType == typeof(DeeplTranslateFile):
                    return new DeeplTranslateFile(filename, config);

                case { } intType when intType == typeof(MTranslateByTranslateFile):
                    return new MTranslateByTranslateFile(filename, config);

                case { } intType when intType == typeof(TranslateRuFile):
                    return new TranslateRuFile(filename, config);

                case { } intType when intType == typeof(SystranNetFile):
                    return new SystranNetFile(filename, config);

            }
            return null;
        }
    }
}
