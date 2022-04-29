using System.IO;
using FluentAssertions;
using NUnit.Framework;
using TranslateViaWeb.Translates.Impl;
using TranslateViaWeb.Configs;

namespace TranslateViaWeb.Tests;

public class DeeplTranslateFileTests
{
    private string[]_args = new string[] { "/from", "en", "/to","ru", "/dir", "./Data", "/output", "./out"  };
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Translate_Success()
    {
        var config = new Configuration(_args);
        var translater = new DeeplTranslateFile("./Data/deepl.txt", config);
        var res= translater.Translate();

        res.Should().BeTrue();
        var text = File.ReadAllText("./out/deepl.en.ru.txt");
        text.Should().Be("стол");

    }
}