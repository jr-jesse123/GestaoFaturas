using GestaoFaturas.Api.Data.Repositories;
using GestaoFaturas.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestaoFaturas.Api.Pages.Clients.ResponsiblePersons;

public class IndexModel : PageModel
{
    private readonly IResponsiblePersonRepository _responsiblePersonRepository;
    private readonly IClientRepository _clientRepository;

    public IndexModel(
        IResponsiblePersonRepository responsiblePersonRepository,
        IClientRepository clientRepository)
    {
        _responsiblePersonRepository = responsiblePersonRepository;
        _clientRepository = clientRepository;
    }

    public IEnumerable<ResponsiblePerson> ResponsiblePersons { get; set; } = new List<ResponsiblePerson>();
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

        ResponsiblePersons = await _responsiblePersonRepository.GetByClientIdAsync(ClientId);

        return Page();
    }
}