using System.Text.RegularExpressions;

namespace GenpactAssignment.Infra.Api;

public class WikipediaClient : ApiClient
{
    private const string ApiBaseUrl = "https://en.wikipedia.org/w/api.php";

    public WikipediaClient() : base() { }

    public async Task<string?> GetSectionHtmlAsync(string pageTitle, string sectionName)
    {
        // 1. Get all sections to find the index
        var url = $"{ApiBaseUrl}?action=parse&page={Uri.EscapeDataString(pageTitle)}&prop=sections&format=json";
        using var doc = await GetJsonAsync(url);

        var sections = doc.RootElement.GetProperty("parse").GetProperty("sections");
        int? index = sections.EnumerateArray()
            .Select(sec => new
            {
                Line = sec.GetProperty("line").GetString(),
                Index = int.Parse(sec.GetProperty("index").GetString()!)
            })
            .FirstOrDefault(s => string.Equals(s.Line, sectionName, StringComparison.OrdinalIgnoreCase))
            ?.Index;

        if (index == null)
            return null;

        // 2. Fetch the section HTML
        var sectionUrl = $"{ApiBaseUrl}?action=parse&page={Uri.EscapeDataString(pageTitle)}&prop=text&section={index}&format=json";
        using var sectionDoc = await GetJsonAsync(sectionUrl);

        return sectionDoc.RootElement
                            .GetProperty("parse")
                            .GetProperty("text")
                            .GetProperty("*")
                            .GetString();
    }

    public static string StripHtml(string html)
    {
        // Remove entire heading tags (h2–h6) including [edit] spans
        html = Regex.Replace(html, "<h[2-6][^>]*>.*?</h[2-6]>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // Remove edit sections (fallback)
        html = Regex.Replace(html, "<span[^>]*class=\"[^\"]*mw-editsection[^\"]*\"[^>]*>.*?</span>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // Remove references like [14] or entire <sup class="reference">
        html = Regex.Replace(html, "<sup[^>]*class=\"[^\"]*reference[^\"]*\"[^>]*>.*?</sup>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // Remove citation tags (<cite>…</cite>)
        html = Regex.Replace(html, "<cite[^>]*>.*?</cite>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // Remove script/style/meta/link tags
        html = Regex.Replace(html, "<(script|style|meta|link)[^>]*>.*?</\\1>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // Remove HTML comments
        html = Regex.Replace(html, "<!--.*?-->", "", RegexOptions.Singleline);

        // Remove all remaining tags
        html = Regex.Replace(html, "<.*?>", " ", RegexOptions.Singleline);

        // Collapse whitespace
        html = Regex.Replace(html, "\\s+", " ").Trim();

        return html;
    }

    public static string NormalizeText(string text)
    {
        text = text.ToLowerInvariant();
        text = Regex.Replace(text, "[^a-z0-9\\s]", " ");
        text = Regex.Replace(text, "\\s+", " ").Trim();
        return text;
    }
}
