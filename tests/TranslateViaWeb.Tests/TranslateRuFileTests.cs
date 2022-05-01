using System.IO;
using FluentAssertions;
using NUnit.Framework;
using TranslateViaWeb.Translates.Impl;
using TranslateViaWeb.Configs;

namespace TranslateViaWeb.Tests;

public class TranslateRuFileTests
{
    private string[] _args = { "/from", "en", "/to", "ru", "/dir", "./Data", "/output", "./out" };

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Translate_Success()
    {
        var config = new Configuration(_args);
        using (var translater = new TranslateRuFile("./Data/TranslateRuFile.txt", config))
        {
            var res = translater.Translate();
            res.Should().BeTrue();
        }

        var text = File.ReadAllText("./out/TranslateRuFile.en.ru.txt");
        text.Should().Be("обратиться");
    }
}