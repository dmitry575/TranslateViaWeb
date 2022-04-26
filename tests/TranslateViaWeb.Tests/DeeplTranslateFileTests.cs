using FluentAssertions;
using NUnit.Framework;
using TranslateViaWeb.Translates.Impl;
using TranslateViaWeb.Configs;

namespace TranslateViaWeb.Tests;

public class DeeplTranslateFileTests
{
    private string[]_args = new string[] { "/from", "en", "/to","ru" };
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Translate_Success()
    {
        var config = new Configuration(_args);
        var translater = new DeeplTranslateFile("test.txt", config);
        var (translateText,res) = translater.Translating("Test");

        res.Should().BeTrue();
        translateText.Should().Be("тест");

    }
}