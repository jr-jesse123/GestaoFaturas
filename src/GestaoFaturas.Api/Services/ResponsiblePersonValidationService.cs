using GestaoFaturas.Api.Data.Repositories;
using GestaoFaturas.Api.Models;
using System.Text.RegularExpressions;

namespace GestaoFaturas.Api.Services;

public class ResponsiblePersonValidationService : IResponsiblePersonValidationService
{
    private readonly IResponsiblePersonRepository _repository;
    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public ResponsiblePersonValidationService(IResponsiblePersonRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ValidationResult> ValidateForCreateAsync(ResponsiblePerson responsiblePerson, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        // Basic field validation
        if (string.IsNullOrWhiteSpace(responsiblePerson.Name))
        {
            errors.Add("Name is required.");
        }
        else if (responsiblePerson.Name.Length > 100)
        {
            errors.Add("Name cannot exceed 100 characters.");
        }

        if (string.IsNullOrWhiteSpace(responsiblePerson.Email))
        {
            errors.Add("Email is required.");
        }
        else if (!EmailRegex.IsMatch(responsiblePerson.Email))
        {
            errors.Add("Email format is invalid.");
        }
        else if (responsiblePerson.Email.Length > 100)
        {
            errors.Add("Email cannot exceed 100 characters.");
        }

        if (!string.IsNullOrEmpty(responsiblePerson.Phone) && responsiblePerson.Phone.Length > 20)
        {
            errors.Add("Phone cannot exceed 20 characters.");
        }

        if (!string.IsNullOrEmpty(responsiblePerson.Role) && responsiblePerson.Role.Length > 100)
        {
            errors.Add("Role cannot exceed 100 characters.");
        }

        if (!string.IsNullOrEmpty(responsiblePerson.Department) && responsiblePerson.Department.Length > 50)
        {
            errors.Add("Department cannot exceed 50 characters.");
        }

        if (responsiblePerson.ClientId <= 0)
        {
            errors.Add("A valid client must be specified.");
        }

        // Check email uniqueness for the client
        if (!string.IsNullOrWhiteSpace(responsiblePerson.Email))
        {
            var emailExists = await _repository.EmailExistsAsync(
                responsiblePerson.Email, 
                responsiblePerson.ClientId, 
                cancellationToken: cancellationToken);
            
            if (emailExists)
            {
                errors.Add($"A responsible person with email '{responsiblePerson.Email}' already exists for this client.");
            }
        }

        // Check primary contact constraint
        if (responsiblePerson.IsPrimaryContact)
        {
            var hasPrimary = await _repository.HasPrimaryContactAsync(
                responsiblePerson.ClientId, 
                cancellationToken: cancellationToken);
            
            if (hasPrimary)
            {
                errors.Add("This client already has a primary contact. Only one primary contact is allowed per client.");
            }
        }

        return errors.Any() 
            ? ValidationResult.Failure(errors.ToArray()) 
            : ValidationResult.Success();
    }

    public async Task<ValidationResult> ValidateForUpdateAsync(ResponsiblePerson responsiblePerson, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        // Basic field validation
        if (string.IsNullOrWhiteSpace(responsiblePerson.Name))
        {
            errors.Add("Name is required.");
        }
        else if (responsiblePerson.Name.Length > 100)
        {
            errors.Add("Name cannot exceed 100 characters.");
        }

        if (string.IsNullOrWhiteSpace(responsiblePerson.Email))
        {
            errors.Add("Email is required.");
        }
        else if (!EmailRegex.IsMatch(responsiblePerson.Email))
        {
            errors.Add("Email format is invalid.");
        }
        else if (responsiblePerson.Email.Length > 100)
        {
            errors.Add("Email cannot exceed 100 characters.");
        }

        if (!string.IsNullOrEmpty(responsiblePerson.Phone) && responsiblePerson.Phone.Length > 20)
        {
            errors.Add("Phone cannot exceed 20 characters.");
        }

        if (!string.IsNullOrEmpty(responsiblePerson.Role) && responsiblePerson.Role.Length > 100)
        {
            errors.Add("Role cannot exceed 100 characters.");
        }

        if (!string.IsNullOrEmpty(responsiblePerson.Department) && responsiblePerson.Department.Length > 50)
        {
            errors.Add("Department cannot exceed 50 characters.");
        }

        // Check email uniqueness for the client (excluding current record)
        if (!string.IsNullOrWhiteSpace(responsiblePerson.Email))
        {
            var emailExists = await _repository.EmailExistsAsync(
                responsiblePerson.Email, 
                responsiblePerson.ClientId, 
                excludeId: responsiblePerson.Id,
                cancellationToken: cancellationToken);
            
            if (emailExists)
            {
                errors.Add($"A responsible person with email '{responsiblePerson.Email}' already exists for this client.");
            }
        }

        // Check primary contact constraint (excluding current record)
        if (responsiblePerson.IsPrimaryContact)
        {
            var hasPrimary = await _repository.HasPrimaryContactAsync(
                responsiblePerson.ClientId, 
                excludeId: responsiblePerson.Id,
                cancellationToken: cancellationToken);
            
            if (hasPrimary)
            {
                errors.Add("This client already has a primary contact. Only one primary contact is allowed per client.");
            }
        }

        return errors.Any() 
            ? ValidationResult.Failure(errors.ToArray()) 
            : ValidationResult.Success();
    }

    public async Task<ValidationResult> ValidatePrimaryContactAsync(int clientId, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var hasPrimary = await _repository.HasPrimaryContactAsync(clientId, excludeId, cancellationToken);
        
        if (hasPrimary)
        {
            return ValidationResult.Failure("This client already has a primary contact. Only one primary contact is allowed per client.");
        }

        return ValidationResult.Success();
    }

    public async Task<ValidationResult> ValidateEmailUniquenessAsync(string email, int clientId, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return ValidationResult.Failure("Email is required.");
        }

        if (!EmailRegex.IsMatch(email))
        {
            return ValidationResult.Failure("Email format is invalid.");
        }

        var emailExists = await _repository.EmailExistsAsync(email, clientId, excludeId, cancellationToken);
        
        if (emailExists)
        {
            return ValidationResult.Failure($"A responsible person with email '{email}' already exists for this client.");
        }

        return ValidationResult.Success();
    }
}