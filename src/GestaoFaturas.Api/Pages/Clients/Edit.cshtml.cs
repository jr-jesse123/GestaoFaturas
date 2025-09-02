using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestaoFaturas.Api.Services;
using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Pages.Clients;

public class EditModel : PageModel
{
    private readonly IClientService _clientService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IClientService clientService, ILogger<EditModel> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    [BindProperty]
    public ClientInputModel ClientInput { get; set; } = new();

    [BindProperty]
    public int ClientId { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            _logger.LogDebug("Loading client for editing - ID: {ClientId}", id);

            var client = await _clientService.GetClientByIdAsync(id);
            
            if (client == null)
            {
                _logger.LogWarning("Client not found for editing - ID: {ClientId}", id);
                return NotFound();
            }

            ClientId = id;
            ClientInput = ClientInputModel.FromDto(client);

            _logger.LogDebug("Loaded client for editing: {CompanyName}", client.CompanyName);
            
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading client for editing - ID: {ClientId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the client. Please try again.";
            return RedirectToPage("/Clients/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            _logger.LogDebug("Edit client form validation failed for ID: {ClientId}", ClientId);
            return Page();
        }

        try
        {
            _logger.LogInformation("Updating client ID: {ClientId} - {CompanyName}", 
                ClientId, ClientInput.CompanyName);

            var updateDto = ClientInput.ToUpdateDto();
            var updatedClient = await _clientService.UpdateClientAsync(ClientId, updateDto);

            if (updatedClient == null)
            {
                _logger.LogWarning("Client not found for update - ID: {ClientId}", ClientId);
                return NotFound();
            }

            _logger.LogInformation("Successfully updated client ID: {ClientId}", ClientId);

            TempData["SuccessMessage"] = $"Client '{updatedClient.CompanyName}' has been updated successfully.";
            
            return RedirectToPage("Details", new { id = ClientId });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            _logger.LogWarning(ex, "Duplicate Tax ID attempted for client ID: {ClientId} - {TaxId}", 
                ClientId, ClientInput.TaxId);
            ModelState.AddModelError("ClientInput.TaxId", ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client ID: {ClientId} - {CompanyName}", 
                ClientId, ClientInput.CompanyName);
            
            ModelState.AddModelError("", "An error occurred while updating the client. Please try again.");
            return Page();
        }
    }
}