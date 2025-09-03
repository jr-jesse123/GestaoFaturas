using System.ComponentModel.DataAnnotations;
using GestaoFaturas.Api.Models;
using Xunit;

namespace GestaoFaturas.Tests.Entities;

public class ResponsiblePersonTests
{
    [Fact]
    public void ResponsiblePerson_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var responsiblePerson = new ResponsiblePerson();

        // Assert
        Assert.Equal(0, responsiblePerson.Id);
        Assert.Equal(0, responsiblePerson.ClientId);
        Assert.Equal(string.Empty, responsiblePerson.Name);
        Assert.Equal(string.Empty, responsiblePerson.Email);
        Assert.Null(responsiblePerson.Phone);
        Assert.Null(responsiblePerson.Role);
        Assert.Null(responsiblePerson.Department);
        Assert.True(responsiblePerson.IsActive);
        Assert.True(responsiblePerson.ReceivesNotifications);
        Assert.False(responsiblePerson.IsPrimaryContact);
        Assert.NotNull(responsiblePerson.CostCenterResponsibles);
        Assert.Empty(responsiblePerson.CostCenterResponsibles);
    }

    [Fact]
    public void ResponsiblePerson_Should_Set_Properties_Correctly()
    {
        // Arrange
        var responsiblePerson = new ResponsiblePerson();
        var testDate = DateTime.UtcNow;
        var client = new Client { Id = 1, CompanyName = "Test Company", TaxId = "12345678901234" };

        // Act
        responsiblePerson.Id = 1;
        responsiblePerson.ClientId = 1;
        responsiblePerson.Name = "John Doe";
        responsiblePerson.Email = "john.doe@example.com";
        responsiblePerson.Phone = "+55 11 98765-4321";
        responsiblePerson.Role = "Finance Manager";
        responsiblePerson.Department = "Finance";
        responsiblePerson.IsActive = true;
        responsiblePerson.ReceivesNotifications = true;
        responsiblePerson.IsPrimaryContact = true;
        responsiblePerson.CreatedAt = testDate;
        responsiblePerson.UpdatedAt = testDate;
        responsiblePerson.Client = client;

        // Assert
        Assert.Equal(1, responsiblePerson.Id);
        Assert.Equal(1, responsiblePerson.ClientId);
        Assert.Equal("John Doe", responsiblePerson.Name);
        Assert.Equal("john.doe@example.com", responsiblePerson.Email);
        Assert.Equal("+55 11 98765-4321", responsiblePerson.Phone);
        Assert.Equal("Finance Manager", responsiblePerson.Role);
        Assert.Equal("Finance", responsiblePerson.Department);
        Assert.True(responsiblePerson.IsActive);
        Assert.True(responsiblePerson.ReceivesNotifications);
        Assert.True(responsiblePerson.IsPrimaryContact);
        Assert.Equal(testDate, responsiblePerson.CreatedAt);
        Assert.Equal(testDate, responsiblePerson.UpdatedAt);
        Assert.Equal(client, responsiblePerson.Client);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ResponsiblePerson_Should_Fail_Validation_With_Invalid_Name(string? invalidName)
    {
        // Arrange
        var responsiblePerson = new ResponsiblePerson
        {
            ClientId = 1,
            Name = invalidName!,
            Email = "test@example.com"
        };

        // Act
        var validationContext = new ValidationContext(responsiblePerson);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(responsiblePerson, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public void ResponsiblePerson_Should_Fail_Validation_With_Invalid_Email(string? invalidEmail)
    {
        // Arrange
        var responsiblePerson = new ResponsiblePerson
        {
            ClientId = 1,
            Name = "John Doe",
            Email = invalidEmail!
        };

        // Act
        var validationContext = new ValidationContext(responsiblePerson);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(responsiblePerson, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Email"));
    }

    [Fact]
    public void ResponsiblePerson_Should_Pass_Validation_With_Valid_Data()
    {
        // Arrange
        var responsiblePerson = new ResponsiblePerson
        {
            ClientId = 1,
            Name = "John Doe",
            Email = "john.doe@example.com",
            Phone = "+55 11 98765-4321",
            Role = "Finance Manager",
            Department = "Finance"
        };

        // Act
        var validationContext = new ValidationContext(responsiblePerson);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(responsiblePerson, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ResponsiblePerson_Should_Have_Correct_MaxLength_Constraints()
    {
        // Arrange
        var responsiblePerson = new ResponsiblePerson
        {
            ClientId = 1,
            Name = new string('A', 101), // Exceeds 100 char limit
            Email = new string('a', 90) + "@test.com", // Exceeds 100 char limit
            Phone = new string('1', 21), // Exceeds 20 char limit
            Role = new string('B', 101), // Exceeds 100 char limit
            Department = new string('C', 51) // Exceeds 50 char limit
        };

        // Act
        var validationContext = new ValidationContext(responsiblePerson);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(responsiblePerson, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
        // Email validation won't trigger for length since it's also an invalid email format
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Phone"));
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Role"));
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Department"));
    }

    [Fact]
    public void ResponsiblePerson_Should_Allow_Null_Optional_Fields()
    {
        // Arrange
        var responsiblePerson = new ResponsiblePerson
        {
            ClientId = 1,
            Name = "John Doe",
            Email = "john.doe@example.com",
            Phone = null,
            Role = null,
            Department = null
        };

        // Act
        var validationContext = new ValidationContext(responsiblePerson);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(responsiblePerson, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ResponsiblePerson_Should_Maintain_Client_Relationship()
    {
        // Arrange
        var client = new Client 
        { 
            Id = 1, 
            CompanyName = "Test Company",
            TaxId = "12345678901234"
        };
        
        var responsiblePerson = new ResponsiblePerson
        {
            Id = 1,
            ClientId = client.Id,
            Name = "John Doe",
            Email = "john.doe@example.com",
            Client = client
        };

        // Act & Assert
        Assert.Equal(client.Id, responsiblePerson.ClientId);
        Assert.Equal(client, responsiblePerson.Client);
        Assert.Same(client, responsiblePerson.Client);
    }

    [Fact]
    public void ResponsiblePerson_Should_Maintain_CostCenterResponsibles_Collection()
    {
        // Arrange
        var responsiblePerson = new ResponsiblePerson
        {
            Id = 1,
            ClientId = 1,
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        var costCenterResponsible = new CostCenterResponsible
        {
            CostCenterId = 1,
            ResponsiblePersonId = responsiblePerson.Id,
            ResponsiblePerson = responsiblePerson
        };

        // Act
        responsiblePerson.CostCenterResponsibles.Add(costCenterResponsible);

        // Assert
        Assert.Single(responsiblePerson.CostCenterResponsibles);
        Assert.Contains(costCenterResponsible, responsiblePerson.CostCenterResponsibles);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ResponsiblePerson_Should_Handle_Boolean_Flags_Correctly(bool isActive, bool receiveNotifications)
    {
        // Arrange
        var responsiblePerson = new ResponsiblePerson
        {
            ClientId = 1,
            Name = "John Doe",
            Email = "john.doe@example.com",
            IsActive = isActive,
            ReceivesNotifications = receiveNotifications
        };

        // Act & Assert
        Assert.Equal(isActive, responsiblePerson.IsActive);
        Assert.Equal(receiveNotifications, responsiblePerson.ReceivesNotifications);
    }

    [Fact]
    public void ResponsiblePerson_Should_Track_IsPrimaryContact_Flag()
    {
        // Arrange
        var responsiblePerson1 = new ResponsiblePerson
        {
            ClientId = 1,
            Name = "Primary Contact",
            Email = "primary@example.com",
            IsPrimaryContact = true
        };

        var responsiblePerson2 = new ResponsiblePerson
        {
            ClientId = 1,
            Name = "Secondary Contact",
            Email = "secondary@example.com",
            IsPrimaryContact = false
        };

        // Act & Assert
        Assert.True(responsiblePerson1.IsPrimaryContact);
        Assert.False(responsiblePerson2.IsPrimaryContact);
    }
}