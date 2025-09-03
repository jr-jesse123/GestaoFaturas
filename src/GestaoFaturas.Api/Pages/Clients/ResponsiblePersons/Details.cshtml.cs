using GestaoFaturas.Api.Data;
using GestaoFaturas.Api.Data.Repositories;
using GestaoFaturas.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Api.Pages.Clients.ResponsiblePersons;

public class DetailsModel : PageModel
{
    private readonly IResponsiblePersonRepository _responsiblePersonRepository;
    private readonly ApplicationDbContext _context;

    public DetailsModel(
        IResponsiblePersonRepository responsiblePersonRepository,
        ApplicationDbContext context)
    {
        _responsiblePersonRepository = responsiblePersonRepository;
        _context = context;
    }

    public ResponsiblePerson ResponsiblePerson { get; set; } = null!;
    public List<CostCenterResponsible> CostCenters { get; set; } = new();

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

        // Load associated cost centers
        CostCenters = await _context.CostCenterResponsibles
            .Include(cc => cc.CostCenter)
            .Where(cc => cc.ResponsiblePersonId == id.Value)
            .OrderBy(cc => cc.IsPrimary ? 0 : 1)
            .ThenBy(cc => cc.CostCenter.Name)
            .ToListAsync();

        return Page();
    }
}