using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace GenpactAssignment.Infra.Web.Client;

public enum SupportedBrowser
{
    Chromium,
    Firefox,
    Webkit
}

public class PlaywrightClient : IAsyncDisposable
{
    private readonly SupportedBrowser _browserType;
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IBrowserContext _context = null!;
    private IPage _page = null!;

    public PlaywrightClient(SupportedBrowser browser = SupportedBrowser.Chromium)
    {
        _browserType = browser;
    }

    public async Task InitAsync()
    {
        _playwright = await Playwright.CreateAsync();

        _browser = _browserType switch
        {
            SupportedBrowser.Firefox => await _playwright.Firefox.LaunchAsync(new() { Headless = false }),
            SupportedBrowser.Webkit => await _playwright.Webkit.LaunchAsync(new() { Headless = false }),
            _ => await _playwright.Chromium.LaunchAsync(new() { Headless = false })
        };

        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    public IPage GetPage() => _page;

    public async Task NavigateAsync(string url)
    {
        await _page.GotoAsync(url);
        await _page.WaitForLoadStateAsync();
    }    

    public async Task<string> GetTitleAsync() =>
        await _page.TitleAsync();

    public async ValueTask DisposeAsync()
    {
        if (_context != null)
            await _context.CloseAsync();
        if (_browser != null)
            await _browser.CloseAsync();
        _playwright?.Dispose();
    }
}
