using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestaoFaturas.Api.Services;
using GestaoFaturas.Api.DTOs;

namespace GestaoFaturas.Api.Pages.Clients;

public class DetailsModel : PageModel
{
    private readonly IClientService _clientService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IClientService clientService, ILogger<DetailsModel> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    public ClientDto? Client { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            _logger.LogDebug("Loading client details for ID: {ClientId}", id);

            Client = await _clientService.GetClientByIdAsync(id);
            
            if (Client == null)
            {
                _logger.LogWarning("Client not found for ID: {ClientId}", id);
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading client details for ID: {ClientId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the client details. Please try again.";
            return RedirectToPage("/Clients/Index");
        }
    }
}