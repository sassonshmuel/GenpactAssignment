using GenpactAssignment.Infra.Web.Client;
using GenpactAssignment.Steps;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;
namespace GenpactAssignment.Tests;

[AllureNUnit]
[AllureSuite("Wikipedia Tests")]
[AllureFeature("Playwright Page Validation")]
public class Tests
{
    private PlaywrightClient? client;
    private const string PageUnderTest = "https://en.wikipedia.org/wiki/Playwright_(software)";

    [SetUp]
    public async Task Setup()
    {
        client = new PlaywrightClient(SupportedBrowser.Chromium);
        await client.InitAsync();
        await client.NavigateAsync(PageUnderTest);
    }

    [TearDown]
    public async Task TearDown()
    {
        if (client is not null)
        {
            await client.DisposeAsync();
            client = null;
        }
    }

    [Test]
    public async Task DebuggingFeaturesUniqueWordsCount()
    {
        var ApiUniqueCount = await ApiSteps.GetDebuggingFeaturesUniqueWords();
        int UiUniqueCount = await WebWikiSteps.GetDebuggingFeaturesText(client);
        Assert.That(ApiUniqueCount, Is.EqualTo(UiUniqueCount));
    }

    [Test]
    public async Task MicrosoftDevToolsLinksValidation()
    {
        var noLinks = await WebWikiSteps.getMicrosoftDevToolsWithoutLinks(client);
        Assert.That(noLinks, Is.Empty, "Expected: No technology without a link");
    }

    [Test]
    public async Task ChangeColor()
    {
        var isDark = await WebWikiSteps.SetDarkColor(client);
        Assert.That(isDark, Is.True);
    }
}
