using Microsoft.Playwright;

namespace GenpactAssignment.Infra.Web.Pages;

public class WikipediaPlaywrightPage : BasePage
{
    public WikipediaPlaywrightPage(IPage page) : base(page)
    {
    }

    public async Task<bool> IsLight()
    {
        var element = await page.QuerySelectorAsync("html[class*='skin-theme-clientpref-day']");
        return element != null;
    }

    public async Task<bool> IsDark(int timeoutMs = 5000)
    {
        try
        {
            await page.WaitForSelectorAsync("html[class*='skin-theme-clientpref-night']",
                new PageWaitForSelectorOptions
                {
                    Timeout = timeoutMs,
                    State = WaitForSelectorState.Attached // waits until element exists in DOM
                });

            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public async Task SelectDayTheme()
    {
        await page.ClickAsync("#skin-client-pref-skin-theme-value-day");
    }

    public async Task SelectNightTheme()
    {
        await page.ClickAsync("#skin-client-pref-skin-theme-value-night");
    }

    public async Task SelectDebuggingFeatures()
    {
        await page.ClickAsync("#toc-Debugging_features");
    }

    public async Task SelectExternalLinks()
    {
        await page.ClickAsync("#toc-External_links");
    }

    public async Task ShowMicrosoftDevelopmentTools()
    {
        await page.ClickAsync("xpath=//*[@aria-labelledby='Microsoft&#95;development&#95;tools6288']//button");
    }

    public async Task<string> GetSectionTextAsync(string sectionId)
    {
        // Find the heading div for the given section (e.g., Debugging_features)
        var headingDiv = page.Locator($"div.mw-heading:has(h3#{sectionId})");
        if (await headingDiv.CountAsync() == 0)
            throw new Exception($"Section '{sectionId}' not found.");

        var sectionText = "";

        // Include the header text
        var headerText = await headingDiv.Locator($"h3#{sectionId}").InnerTextAsync();
        if (!string.IsNullOrEmpty(headerText))
            sectionText += headerText + " ";

        // Get all sibling elements after the heading div until the next heading
        var siblings = headingDiv.Locator("xpath=following-sibling::*");
        int count = await siblings.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var element = siblings.Nth(i);
            var classAttr = await element.GetAttributeAsync("class") ?? "";
            if (classAttr.Contains("mw-heading"))
                break;

            // Remove <sup> tags (Wikipedia footnotes)
            await element.EvaluateAsync(@"el => {
                el.querySelectorAll('sup').forEach(s => s.remove());
            }");

            var text = await element.InnerTextAsync();
            if (!string.IsNullOrEmpty(text))
                sectionText += text + " ";
        }

        return sectionText.Trim();
    }

    public async Task<List<string>> GetAllTechnologiesWithoutALink()
    {
        var results = new List<string>();

        var xpath = "//table//table//td//li[not(.//a[@href])]//*[normalize-space(.)]";

        var textNodes = page.Locator(xpath);

        var texts = await textNodes.AllTextContentsAsync();

        foreach (var text in texts)
        {
            var cleaned = text.Trim();
            if (!string.IsNullOrEmpty(cleaned))
            {
                results.Add(cleaned);
            }
        }

        return results;
    }
}

