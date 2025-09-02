using System.ComponentModel.DataAnnotations;
using GestaoFaturas.Api.DTOs;
using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Tests.DTOs;

public class ClientDtoTests
{
    #region ClientDto Tests
    
    [Fact]
    public void ClientDto_FromEntity_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var client = new Client
        {
            Id = 1,
            CompanyName = "Test Company",
            TradeName = "Test Trade Name",
            TaxId = "12345678901234",
            Email = "test@company.com",
            Phone = "11999999999",
            Address = "123 Test Street",
            ContactPerson = "John Doe",
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 1, 2)
        };

        // Act
        var dto = ClientDto.FromEntity(client);

        // Assert
        Assert.Equal(client.Id, dto.Id);
        Assert.Equal(client.CompanyName, dto.CompanyName);
        Assert.Equal(client.TradeName, dto.TradeName);
        Assert.Equal(client.TaxId, dto.TaxId);
        Assert.Equal(client.Email, dto.Email);
        Assert.Equal(client.Phone, dto.Phone);
        Assert.Equal(client.Address, dto.Address);
        Assert.Equal(client.ContactPerson, dto.ContactPerson);
        Assert.Equal(client.IsActive, dto.IsActive);
        Assert.Equal(client.CreatedAt, dto.CreatedAt);
        Assert.Equal(client.UpdatedAt, dto.UpdatedAt);
    }

    [Fact]
    public void ClientDto_FromEntity_ShouldHandleNullOptionalProperties()
    {
        // Arrange
        var client = new Client
        {
            Id = 1,
            CompanyName = "Test Company",
            TradeName = null,
            TaxId = "12345678901234",
            Email = null,
            Phone = null,
            Address = null,
            ContactPerson = null,
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 1, 2)
        };

        // Act
        var dto = ClientDto.FromEntity(client);

        // Assert
        Assert.Null(dto.TradeName);
        Assert.Null(dto.Email);
        Assert.Null(dto.Phone);
        Assert.Null(dto.Address);
        Assert.Null(dto.ContactPerson);
    }

    #endregion

    #region CreateClientDto Tests

    [Fact]
    public void CreateClientDto_ShouldHaveCorrectValidationAttributes()
    {
        // Arrange & Act & Assert
        var companyNameProperty = typeof(CreateClientDto).GetProperty("CompanyName")!;
        Assert.True(companyNameProperty.GetCustomAttributes(typeof(RequiredAttribute), false).Length > 0);
        Assert.True(companyNameProperty.GetCustomAttributes(typeof(StringLengthAttribute), false).Length > 0);

        var taxIdProperty = typeof(CreateClientDto).GetProperty("TaxId")!;
        Assert.True(taxIdProperty.GetCustomAttributes(typeof(RequiredAttribute), false).Length > 0);
        Assert.True(taxIdProperty.GetCustomAttributes(typeof(StringLengthAttribute), false).Length > 0);

        var emailProperty = typeof(CreateClientDto).GetProperty("Email")!;
        Assert.True(emailProperty.GetCustomAttributes(typeof(EmailAddressAttribute), false).Length > 0);
        Assert.True(emailProperty.GetCustomAttributes(typeof(StringLengthAttribute), false).Length > 0);

        var phoneProperty = typeof(CreateClientDto).GetProperty("Phone")!;
        Assert.True(phoneProperty.GetCustomAttributes(typeof(PhoneAttribute), false).Length > 0);
        Assert.True(phoneProperty.GetCustomAttributes(typeof(StringLengthAttribute), false).Length > 0);
    }

    [Theory]
    [InlineData("", false)] // Empty string should fail
    [InlineData(null, false)] // Null should fail
    [InlineData("Valid Company Name", true)] // Valid name should pass
    [InlineData("A", true)] // Single character should pass
    public void CreateClientDto_CompanyName_ValidationBehavior(string? companyName, bool shouldBeValid)
    {
        // Arrange
        var dto = new CreateClientDto
        {
            CompanyName = companyName!,
            TaxId = "12345678901234"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        var hasCompanyNameError = validationResults.Any(r => r.MemberNames.Contains("CompanyName"));
        Assert.Equal(!shouldBeValid, hasCompanyNameError);
    }

    [Theory]
    [InlineData("", false)] // Empty string should fail
    [InlineData(null, false)] // Null should fail  
    [InlineData("12345678901234", true)] // Valid CNPJ length should pass
    [InlineData("12345678901", true)] // Valid CPF length should pass
    [InlineData("123456789012345", false)] // Too long should fail
    public void CreateClientDto_TaxId_ValidationBehavior(string? taxId, bool shouldBeValid)
    {
        // Arrange
        var dto = new CreateClientDto
        {
            CompanyName = "Test Company",
            TaxId = taxId!
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        var hasTaxIdError = validationResults.Any(r => r.MemberNames.Contains("TaxId"));
        Assert.Equal(!shouldBeValid, hasTaxIdError);
    }

    [Theory]
    [InlineData(null, true)] // Null should be valid (optional)
    [InlineData("", false)] // Empty should be invalid (EmailAddress attribute validates empty as invalid)
    [InlineData("test@example.com", true)] // Valid email should pass
    [InlineData("invalid-email", false)] // Invalid email should fail
    public void CreateClientDto_Email_ValidationBehavior(string? email, bool shouldBeValid)
    {
        // Arrange
        var dto = new CreateClientDto
        {
            CompanyName = "Test Company",
            TaxId = "12345678901234",
            Email = email
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        var hasEmailError = validationResults.Any(r => r.MemberNames.Contains("Email"));
        Assert.Equal(!shouldBeValid, hasEmailError);
    }

    [Fact]
    public void CreateClientDto_ToEntity_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var dto = new CreateClientDto
        {
            CompanyName = "Test Company",
            TradeName = "Test Trade Name",
            TaxId = "12345678901234",
            Email = "test@company.com",
            Phone = "11999999999",
            Address = "123 Test Street",
            ContactPerson = "John Doe"
        };

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal(0, entity.Id); // New entity should have default Id
        Assert.Equal(dto.CompanyName, entity.CompanyName);
        Assert.Equal(dto.TradeName, entity.TradeName);
        Assert.Equal(dto.TaxId, entity.TaxId);
        Assert.Equal(dto.Email, entity.Email);
        Assert.Equal(dto.Phone, entity.Phone);
        Assert.Equal(dto.Address, entity.Address);
        Assert.Equal(dto.ContactPerson, entity.ContactPerson);
        Assert.True(entity.IsActive); // Should default to true
        Assert.Equal(default(DateTime), entity.CreatedAt); // Should be set by service
        Assert.Equal(default(DateTime), entity.UpdatedAt); // Should be set by service
    }

    #endregion

    #region UpdateClientDto Tests

    [Fact]
    public void UpdateClientDto_ShouldHaveCorrectValidationAttributes()
    {
        // Arrange & Act & Assert - Same validation as CreateClientDto
        var companyNameProperty = typeof(UpdateClientDto).GetProperty("CompanyName")!;
        Assert.True(companyNameProperty.GetCustomAttributes(typeof(RequiredAttribute), false).Length > 0);
        Assert.True(companyNameProperty.GetCustomAttributes(typeof(StringLengthAttribute), false).Length > 0);

        var taxIdProperty = typeof(UpdateClientDto).GetProperty("TaxId")!;
        Assert.True(taxIdProperty.GetCustomAttributes(typeof(RequiredAttribute), false).Length > 0);
        Assert.True(taxIdProperty.GetCustomAttributes(typeof(StringLengthAttribute), false).Length > 0);

        var emailProperty = typeof(UpdateClientDto).GetProperty("Email")!;
        Assert.True(emailProperty.GetCustomAttributes(typeof(EmailAddressAttribute), false).Length > 0);
        Assert.True(emailProperty.GetCustomAttributes(typeof(StringLengthAttribute), false).Length > 0);
    }

    [Fact]
    public void UpdateClientDto_UpdateEntity_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var existingEntity = new Client
        {
            Id = 1,
            CompanyName = "Old Company",
            TradeName = "Old Trade Name",
            TaxId = "11111111111111",
            Email = "old@company.com",
            Phone = "11888888888",
            Address = "Old Address",
            ContactPerson = "Old Contact",
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 1, 1)
        };

        var dto = new UpdateClientDto
        {
            CompanyName = "New Company",
            TradeName = "New Trade Name",
            TaxId = "22222222222222",
            Email = "new@company.com",
            Phone = "11777777777",
            Address = "New Address",
            ContactPerson = "New Contact"
        };

        // Act
        dto.UpdateEntity(existingEntity);

        // Assert
        Assert.Equal(1, existingEntity.Id); // Id should remain unchanged
        Assert.Equal(dto.CompanyName, existingEntity.CompanyName);
        Assert.Equal(dto.TradeName, existingEntity.TradeName);
        Assert.Equal(dto.TaxId, existingEntity.TaxId);
        Assert.Equal(dto.Email, existingEntity.Email);
        Assert.Equal(dto.Phone, existingEntity.Phone);
        Assert.Equal(dto.Address, existingEntity.Address);
        Assert.Equal(dto.ContactPerson, existingEntity.ContactPerson);
        Assert.True(existingEntity.IsActive); // Should remain unchanged
        Assert.Equal(new DateTime(2024, 1, 1), existingEntity.CreatedAt); // Should remain unchanged
        // UpdatedAt will be set by the service layer
    }

    [Fact]
    public void UpdateClientDto_UpdateEntity_ShouldHandleNullOptionalProperties()
    {
        // Arrange
        var existingEntity = new Client
        {
            Id = 1,
            CompanyName = "Company",
            TaxId = "12345678901234",
            TradeName = "Trade Name",
            Email = "test@company.com",
            IsActive = true
        };

        var dto = new UpdateClientDto
        {
            CompanyName = "Updated Company",
            TaxId = "22222222222222",
            TradeName = null,
            Email = null,
            Phone = null,
            Address = null,
            ContactPerson = null
        };

        // Act
        dto.UpdateEntity(existingEntity);

        // Assert
        Assert.Null(existingEntity.TradeName);
        Assert.Null(existingEntity.Email);
        Assert.Null(existingEntity.Phone);
        Assert.Null(existingEntity.Address);
        Assert.Null(existingEntity.ContactPerson);
    }

    #endregion

    #region Helper Methods

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    #endregion
}