using System.IO;
using FluentAssertions;
using NUnit.Framework;
using TranslateViaWeb.Translates.Impl;
using TranslateViaWeb.Configs;

namespace TranslateViaWeb.Tests;

public class TranslatorEuFileTests
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
        using (var translater = new TranslatorEuFile("./Data/TranslatorEu.txt", config))
        {
            var res = translater.Translate();
            res.Should().BeTrue();
        }

        var text = File.ReadAllText("./out/TranslatorEu.en.ru.txt");
        text.Should().Be("предполагать");
    }
}