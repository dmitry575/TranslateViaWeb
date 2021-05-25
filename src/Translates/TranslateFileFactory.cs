using System;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Translates.Impl;

namespace TranslateViaWeb.Translates
{
    public static class TranslateFileFactory
    {
        public static BaseTranslateFile Create(Type type, string filename, Configuration config)
        {
            var constructorInfo = type.GetConstructor(new[] { typeof(string), typeof(Configuration) });
            if (constructorInfo != null)
            {
                object[] parameters = { filename, config };
                return (BaseTranslateFile)constructorInfo.Invoke(parameters);
            }

            return null;
            /*   switch (type)
               {
                   case { } intType when intType == typeof(DeeplTranslateFile):
                       return new DeeplTranslateFile(filename, config);

                   case { } intType when intType == typeof(MTranslateByTranslateFile):
                       return new MTranslateByTranslateFile(filename, config);

                   case { } intType when intType == typeof(TranslateRuFile):
                       return new TranslateRuFile(filename, config);

                   case { } intType when intType == typeof(SystranNetFile):
                       return new SystranNetFile(filename, config);

                   case { } intType when intType == typeof(BingFile):
                       return new BingFile(filename, config);

                   //case { } intType when intType == typeof(TranslatorEuFile):
                   //    return new TranslatorEuFile(filename, config);

               }
               return null;*/
        }
    }
}
