using GestaoFaturas.Api.Data.Repositories;
using GestaoFaturas.Api.Models;
using GestaoFaturas.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestaoFaturas.Api.Pages.Clients.ResponsiblePersons;

public class EditModel : PageModel
{
    private readonly IResponsiblePersonRepository _responsiblePersonRepository;
    private readonly IResponsiblePersonValidationService _validationService;
    private readonly IUnitOfWork _unitOfWork;

    public EditModel(
        IResponsiblePersonRepository responsiblePersonRepository,
        IResponsiblePersonValidationService validationService,
        IUnitOfWork unitOfWork)
    {
        _responsiblePersonRepository = responsiblePersonRepository;
        _validationService = validationService;
        _unitOfWork = unitOfWork;
    }

    [BindProperty]
    public ResponsiblePerson ResponsiblePerson { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (!id.HasValue)
        {
            return BadRequest("ID is required");
        }

        var person = await _responsiblePersonRepository.GetByIdWithClientAsync(id.Value);
        
        if (person == null)
        {
            return NotFound($"Responsible person with ID {id} not found");
        }

        ResponsiblePerson = person;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (!id.HasValue)
        {
            return BadRequest("ID is required");
        }

        if (ResponsiblePerson.Id != id.Value)
        {
            return BadRequest("ID mismatch");
        }

        ResponsiblePerson.UpdatedAt = DateTime.UtcNow;

        // Validate the responsible person
        var validationResult = await _validationService.ValidateForUpdateAsync(ResponsiblePerson);
        
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            
            // Reload the Client for display
            ResponsiblePerson = await _responsiblePersonRepository.GetByIdWithClientAsync(id.Value) ?? ResponsiblePerson;
            return Page();
        }

        if (!ModelState.IsValid)
        {
            // Reload the Client for display
            ResponsiblePerson = await _responsiblePersonRepository.GetByIdWithClientAsync(id.Value) ?? ResponsiblePerson;
            return Page();
        }

        try
        {
            _responsiblePersonRepository.Update(ResponsiblePerson);
            await _unitOfWork.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Responsible person updated successfully.";
            return RedirectToPage("Index", new { clientId = ResponsiblePerson.ClientId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred while saving: {ex.Message}");
            
            // Reload the Client for display
            ResponsiblePerson = await _responsiblePersonRepository.GetByIdWithClientAsync(id.Value) ?? ResponsiblePerson;
            return Page();
        }
    }
}