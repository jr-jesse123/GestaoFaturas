using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestaoFaturas.Api.Services;
using GestaoFaturas.Api.DTOs;

namespace GestaoFaturas.Api.Pages.Clients;

public class IndexModel : PageModel
{
    private readonly IClientService _clientService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IClientService clientService, ILogger<IndexModel> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    public PagedResult<ClientDto> Clients { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty]
    public string SearchInput { get; set; } = string.Empty;

    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
    {
        try
        {
            PageNumber = Math.Max(1, pageNumber);
            PageSize = Math.Clamp(pageSize, 5, 50);
            SearchTerm = search;

            _logger.LogDebug("Loading clients page - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}", 
                PageNumber, PageSize, SearchTerm);

            Clients = await _clientService.GetClientsAsync(PageNumber, PageSize, SearchTerm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading clients page");
            TempData["ErrorMessage"] = "An error occurred while loading clients. Please try again.";
            Clients = new PagedResult<ClientDto>
            {
                Items = new List<ClientDto>(),
                TotalItems = 0,
                PageNumber = PageNumber,
                PageSize = PageSize,
                TotalPages = 0
            };
        }
    }

    public IActionResult OnPostSearchAsync()
    {
        return RedirectToPage("/Clients/Index", new { search = SearchInput, pageNumber = 1 });
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(int id)
    {
        try
        {
            var success = await _clientService.DeactivateClientAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Client status updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Client not found.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client status for ID: {ClientId}", id);
            TempData["ErrorMessage"] = "An error occurred while updating the client status.";
        }

        return RedirectToPage("/Clients/Index", new { search = SearchTerm, pageNumber = PageNumber, pageSize = PageSize });
    }
}