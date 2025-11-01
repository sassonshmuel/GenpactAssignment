using System.Threading.Tasks;
using Microsoft.Playwright;

namespace GenpactAssignment.Infra.Web.Pages;

public abstract class BasePage
{
    protected readonly IPage page;

    public BasePage(IPage page)
    {
        this.page = page;
    }
}

