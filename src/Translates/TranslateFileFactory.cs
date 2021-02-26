using System;
using TranslateViaWeb.Configs;

namespace TranslateViaWeb.Translates
{
    public class TranslateFileFactory
    {
        public static ITranslateFile Create(Type type, string filename,Configuration config) 
        {
            switch (type) 
            {
                case Type intType when intType == typeof(DeeplTranslateFile):
                    return new DeeplTranslateFile(filename, config);

            }
            return null;
        }
    }
}
