using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Services;

public interface IResponsiblePersonValidationService
{
    Task<ValidationResult> ValidateForCreateAsync(ResponsiblePerson responsiblePerson, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateForUpdateAsync(ResponsiblePerson responsiblePerson, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidatePrimaryContactAsync(int clientId, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateEmailUniquenessAsync(string email, int clientId, int? excludeId = null, CancellationToken cancellationToken = default);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Failure(params string[] errors)
    {
        return new ValidationResult 
        { 
            IsValid = false, 
            Errors = errors.ToList() 
        };
    }
}