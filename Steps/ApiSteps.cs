using GenpactAssignment.Infra.Api;

namespace GenpactAssignment.Steps;

public class ApiSteps
{
    public static async Task<int> GetDebuggingFeaturesUniqueWords()
    {
        await using var wiki = new WikipediaClient();

        string page = "Playwright_(software)";
        string section = "Debugging features";

        string? html = await wiki.GetSectionHtmlAsync(page, section);
        string plainText = WikipediaClient.StripHtml(html!);
        string normalized = WikipediaClient.NormalizeText(plainText);
        var ApiNormalized = TextUtil.GetWords(normalized);
        int ApiUniqueCount = TextUtil.CountUniqueWords(ApiNormalized);
        Console.WriteLine($"Normalized: {string.Join(", ", ApiNormalized)}");
        Console.WriteLine($"Unique words: {ApiUniqueCount}");
        return ApiUniqueCount;
    }
}