using GestaoFaturas.Api.Data.Repositories;
using GestaoFaturas.Api.Models;
using GestaoFaturas.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestaoFaturas.Api.Pages.Clients.ResponsiblePersons;

public class CreateModel : PageModel
{
    private readonly IResponsiblePersonRepository _responsiblePersonRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IResponsiblePersonValidationService _validationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateModel(
        IResponsiblePersonRepository responsiblePersonRepository,
        IClientRepository clientRepository,
        IResponsiblePersonValidationService validationService,
        IUnitOfWork unitOfWork)
    {
        _responsiblePersonRepository = responsiblePersonRepository;
        _clientRepository = clientRepository;
        _validationService = validationService;
        _unitOfWork = unitOfWork;
    }

    [BindProperty]
    public ResponsiblePerson ResponsiblePerson { get; set; } = new();
    
    public Client? Client { get; set; }
    public int ClientId { get; set; }

    public async Task<IActionResult> OnGetAsync(int? clientId)
    {
        if (!clientId.HasValue)
        {
            return BadRequest("Client ID is required");
        }

        ClientId = clientId.Value;
        Client = await _clientRepository.GetByIdAsync(ClientId);

        if (Client == null)
        {
            return NotFound($"Client with ID {ClientId} not found");
        }

        ResponsiblePerson.ClientId = ClientId;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? clientId)
    {
        if (!clientId.HasValue)
        {
            return BadRequest("Client ID is required");
        }

        ClientId = clientId.Value;
        Client = await _clientRepository.GetByIdAsync(ClientId);

        if (Client == null)
        {
            return NotFound($"Client with ID {ClientId} not found");
        }

        ResponsiblePerson.ClientId = ClientId;
        ResponsiblePerson.CreatedAt = DateTime.UtcNow;
        ResponsiblePerson.UpdatedAt = DateTime.UtcNow;

        // Validate the responsible person
        var validationResult = await _validationService.ValidateForCreateAsync(ResponsiblePerson);
        
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _responsiblePersonRepository.AddAsync(ResponsiblePerson);
            await _unitOfWork.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Responsible person created successfully.";
            return RedirectToPage("Index", new { clientId = ClientId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred while saving: {ex.Message}");
            return Page();
        }
    }
}