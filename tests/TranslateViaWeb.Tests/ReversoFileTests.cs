using FluentAssertions;
using NUnit.Framework;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Translates.Impl;

namespace TranslateViaWeb.Tests;

public class ReversoFileTests
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
        var reverso = new ReversoFile("test.txt", config);
        var (t,e) = reverso.Translating("Test");
    }
}