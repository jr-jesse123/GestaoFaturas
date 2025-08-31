# Technical Specification

This is the technical specification for the spec detailed in @.agent-os/specs/2025-08-31-database-schema-setup/spec.md

## Technical Requirements

### Domain Entity Design
- **Client Entity**: Store company information including Name, TaxId (CNPJ/CPF), Address, Phone, Email, IsActive status, and audit fields (CreatedAt, UpdatedAt)
- **CostCenter Entity**: Manage cost center hierarchy with Code, Name, Description, ParentCostCenterId for hierarchical structure, and IsActive status
- **ResponsiblePerson Entity**: Track individuals responsible for cost centers with Name, Email, Phone, Position, and relationship to CostCenter
- **Invoice Entity**: Core invoice data including InvoiceNumber, IssueDate, DueDate, Amount, Status, ServicePeriodStart, ServicePeriodEnd, and relationships to Client and CostCenter
- **InvoiceStatus Entity**: Reference table for invoice workflow states (Pending, Processing, Approved, Rejected, Paid)
- **InvoiceHistory Entity**: Audit trail for invoice status changes with timestamp, user, and comments
- **User Entity**: Extended from ASP.NET Core Identity with additional fields for FullName, Department, and LastLoginAt
- **SystemSetting Entity**: Key-value store for application configuration and feature flags

### Entity Framework Core Configuration
- **DbContext Setup**: Configure ApplicationDbContext inheriting from IdentityDbContext with proper DbSet properties for all entities
- **Connection String Management**: Support for environment-specific connection strings with Azure PostgreSQL format compatibility
- **Migration Strategy**: Code-first approach with automatic migration execution on application startup in development, manual for production
- **Fluent API Configuration**: Define entity configurations using IEntityTypeConfiguration for clean separation of concerns
- **Value Converters**: Implement converters for decimal precision (financial amounts) and DateTime UTC handling
- **Query Filters**: Global query filters for soft-delete support and multi-tenant data isolation

### Database Performance Optimization
- **Index Strategy**: Create composite indexes on (ClientId, CostCenterId, IssueDate) for invoice queries, unique index on Client.TaxId, and covering indexes for common report queries
- **Lazy Loading Configuration**: Disable lazy loading by default to prevent N+1 queries, use explicit Include() for related data
- **Connection Pooling**: Configure PostgreSQL connection pooling with appropriate min/max pool sizes for Azure environment
- **Query Optimization**: Implement AsNoTracking() for read-only queries and compiled queries for frequently used operations

### Data Integrity and Validation
- **Referential Integrity**: Configure cascade delete rules appropriately (restrict for Clients, cascade for InvoiceHistory)
- **Check Constraints**: Add database-level constraints for Amount > 0, IssueDate < DueDate, ServicePeriodStart < ServicePeriodEnd
- **Unique Constraints**: Enforce uniqueness on (ClientId, InvoiceNumber) combination and CostCenter.Code
- **Required Fields**: Configure non-nullable columns for business-critical fields with proper default values where applicable
- **String Length Limits**: Define appropriate MaxLength for all string properties based on business requirements

## External Dependencies

- **Npgsql.EntityFrameworkCore.PostgreSQL** (v8.0.x) - PostgreSQL provider for Entity Framework Core
- **Justification:** Required for PostgreSQL database connectivity and EF Core integration with PostgreSQL-specific features

- **Microsoft.EntityFrameworkCore.Design** (v8.0.x) - Design-time tools for EF Core migrations
- **Justification:** Necessary for creating and managing database migrations during development

- **EFCore.NamingConventions** (v8.0.x) - PostgreSQL naming convention support
- **Justification:** Automatically converts C# PascalCase to PostgreSQL snake_case naming convention