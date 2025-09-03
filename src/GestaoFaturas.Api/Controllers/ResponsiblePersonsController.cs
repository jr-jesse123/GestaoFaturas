using GestaoFaturas.Api.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GestaoFaturas.Api.Controllers;

[ApiController]
[Route("api/responsible-persons")]
public class ResponsiblePersonsController : ControllerBase
{
    private readonly IResponsiblePersonRepository _repository;

    public ResponsiblePersonsController(IResponsiblePersonRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmail(
        [FromQuery] string email,
        [FromQuery] int clientId,
        [FromQuery] int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(email) || clientId <= 0)
        {
            return BadRequest(new { exists = false });
        }

        var exists = await _repository.EmailExistsAsync(email, clientId, excludeId);
        return Ok(new { exists });
    }

    [HttpGet("check-primary")]
    public async Task<IActionResult> CheckPrimaryContact(
        [FromQuery] int clientId,
        [FromQuery] int? excludeId = null)
    {
        if (clientId <= 0)
        {
            return BadRequest(new { hasPrimary = false });
        }

        var hasPrimary = await _repository.HasPrimaryContactAsync(clientId, excludeId);
        return Ok(new { hasPrimary });
    }
}