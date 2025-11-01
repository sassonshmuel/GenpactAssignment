using GenpactAssignment.Infra.Web.Client;
using GenpactAssignment.Infra.Web.Pages;
using NUnit.Allure.Attributes;

namespace GenpactAssignment.Steps;

public class WebWikiSteps
{
    public static async Task<int> GetDebuggingFeaturesText(PlaywrightClient? client)
    {
        VerifyPlaywrightInitialized(client);
        var WikiPage = new WikipediaPlaywrightPage(client!.GetPage());
        var UiText = await WikiPage.GetSectionTextAsync("Debugging_features");
        var UiNormalized = TextUtil.GetWords(UiText);
        Console.WriteLine($"UI text: {string.Join(", ", UiNormalized)}");
        int UiUniqueCount = TextUtil.CountUniqueWords(UiNormalized);
        Console.WriteLine($"UI Unique words: {UiUniqueCount}");
        return UiUniqueCount;
    }

    public static async Task<List<string>> getMicrosoftDevToolsWithoutLinks(PlaywrightClient? client)
    {
        VerifyPlaywrightInitialized(client);
        var WikiPage = new WikipediaPlaywrightPage(client!.GetPage());
        await WikiPage.SelectExternalLinks();
        await WikiPage.ShowMicrosoftDevelopmentTools();
        var noLinks = await WikiPage.GetAllTechnologiesWithoutALink();

        Console.WriteLine("Technologies without links:");
        foreach (var t in noLinks)
            Console.WriteLine($"'{t}'");
        return noLinks;
    }

    public static async Task<bool> SetDarkColor(PlaywrightClient? client)
    {
        VerifyPlaywrightInitialized(client);
        var WikiPage = new WikipediaPlaywrightPage(client!.GetPage());
        await WikiPage.SelectNightTheme();
        var isDark = await WikiPage.IsDark();
        return isDark;
    }

    private static void VerifyPlaywrightInitialized(PlaywrightClient? client)
    {
        Assert.That(client, Is.Not.Null, "Playwright was not initialized");
    }
}