using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestaoFaturas.Api.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogDebug("Dashboard page accessed");
    }
}