using GestaoFaturas.Api.Data;
using GestaoFaturas.Api.Data.Repositories;
using GestaoFaturas.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Api.Pages.Clients.ResponsiblePersons;

public class DeleteModel : PageModel
{
    private readonly IResponsiblePersonRepository _responsiblePersonRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public DeleteModel(
        IResponsiblePersonRepository responsiblePersonRepository,
        IUnitOfWork unitOfWork,
        ApplicationDbContext context)
    {
        _responsiblePersonRepository = responsiblePersonRepository;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    [BindProperty]
    public ResponsiblePerson ResponsiblePerson { get; set; } = null!;
    
    public bool HasAssociations { get; set; }
    public int AssociatedCostCentersCount { get; set; }

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

        // Check for associations
        AssociatedCostCentersCount = await _context.CostCenterResponsibles
            .CountAsync(cc => cc.ResponsiblePersonId == id.Value);
        
        HasAssociations = AssociatedCostCentersCount > 0;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (!id.HasValue)
        {
            return BadRequest("ID is required");
        }

        var person = await _responsiblePersonRepository.GetByIdAsync(id.Value);
        
        if (person == null)
        {
            return NotFound($"Responsible person with ID {id} not found");
        }

        var clientId = person.ClientId;

        try
        {
            // Remove any cost center associations first
            var associations = await _context.CostCenterResponsibles
                .Where(cc => cc.ResponsiblePersonId == id.Value)
                .ToListAsync();
            
            if (associations.Any())
            {
                _context.CostCenterResponsibles.RemoveRange(associations);
            }

            // Remove the responsible person
            _responsiblePersonRepository.Remove(person);
            await _unitOfWork.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Responsible person deleted successfully.";
            return RedirectToPage("Index", new { clientId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred while deleting: {ex.Message}";
            return RedirectToPage("Index", new { clientId });
        }
    }
}