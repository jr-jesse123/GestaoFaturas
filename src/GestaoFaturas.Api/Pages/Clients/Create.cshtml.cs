using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestaoFaturas.Api.Services;
using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Pages.Clients;

public class CreateModel : PageModel
{
    private readonly IClientService _clientService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(IClientService clientService, ILogger<CreateModel> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    [BindProperty]
    public ClientInputModel ClientInput { get; set; } = new();

    public void OnGet()
    {
        // Initialize empty form
        ClientInput = new ClientInputModel();
        
        _logger.LogDebug("Initialized create client form");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            _logger.LogDebug("Create client form validation failed");
            return Page();
        }

        try
        {
            _logger.LogInformation("Creating new client: {CompanyName} - {TaxId}", 
                ClientInput.CompanyName, ClientInput.TaxId);

            var createDto = ClientInput.ToCreateDto();
            var createdClient = await _clientService.CreateClientAsync(createDto);

            _logger.LogInformation("Successfully created client with ID: {ClientId}", createdClient.Id);

            TempData["SuccessMessage"] = $"Client '{createdClient.CompanyName}' has been created successfully.";
            
            return RedirectToPage("Details", new { id = createdClient.Id });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            _logger.LogWarning(ex, "Duplicate Tax ID attempted: {TaxId}", ClientInput.TaxId);
            ModelState.AddModelError("ClientInput.TaxId", ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client: {CompanyName} - {TaxId}", 
                ClientInput.CompanyName, ClientInput.TaxId);
            
            ModelState.AddModelError("", "An error occurred while creating the client. Please try again.");
            return Page();
        }
    }
}