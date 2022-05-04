using System.IO;
using FluentAssertions;
using NUnit.Framework;
using TranslateViaWeb.Translates.Impl;
using TranslateViaWeb.Configs;

namespace TranslateViaWeb.Tests;

public class ReversoFileTests
{
    private string[] _args = { "/from", "en", "/to", "ru", "/dir", "./Data", "/output", "./out" };

    [Test]
    public void Translate_Success()
    {
        var config = new Configuration(_args);
        using (var translate = new ReversoFile("./Data/ReversoFile.txt", config))
        {
            var res = translate.Translate();
            res.Should().BeTrue();
        }

        var text = File.ReadAllText("./out/ReversoFile.en.ru.txt");
        text.Should().Be("несомненно");
    }
}