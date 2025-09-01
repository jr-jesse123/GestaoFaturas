# Spec Tasks

## Tasks

- [x] 1. Set up Entity Framework Core and PostgreSQL dependencies
  - [x] 1.1 Write tests for database connection and context initialization
  - [x] 1.2 Install required NuGet packages (Npgsql.EntityFrameworkCore.PostgreSQL, EFCore.NamingConventions)
  - [x] 1.3 Configure connection string in appsettings.json for development environment
  - [x] 1.4 Create ApplicationDbContext inheriting from IdentityDbContext
  - [x] 1.5 Configure DbContext options and PostgreSQL provider in Program.cs
  - [x] 1.6 Verify all tests pass

- [x] 2. Implement core domain entities and configurations
  - [x] 2.1 Write tests for Client entity CRUD operations
  - [x] 2.2 Create Client entity with required properties and validation attributes
  - [x] 2.3 Create CostCenter entity with hierarchical structure support
  - [x] 2.4 Create ResponsiblePerson entity linked to CostCenter
  - [x] 2.5 Create Invoice and InvoiceStatus entities with relationships
  - [x] 2.6 Create InvoiceHistory entity for audit trail
  - [x] 2.7 Implement IEntityTypeConfiguration for each entity with Fluent API
  - [x] 2.8 Verify all tests pass

- [x] 3. Configure database constraints and indexes
  - [x] 3.1 Write tests for unique constraints and check constraints
  - [x] 3.2 Configure unique constraints (Client.TaxId, CostCenter.Code, Client+InvoiceNumber)
  - [x] 3.3 Add check constraints for business rules (amount > 0, date validations)
  - [x] 3.4 Create composite and covering indexes for query optimization
  - [x] 3.5 Configure cascade delete rules and referential integrity
  - [x] 3.6 Verify all tests pass

- [x] 4. Create and execute initial database migration
  - [x] 4.1 Write integration tests using TestContainers for PostgreSQL
  - [x] 4.2 Generate initial migration using EF Core tools
  - [x] 4.3 Review generated migration for correctness
  - [x] 4.4 Add seed data for InvoiceStatus tables
  - [x] 4.5 Test migration execution in local PostgreSQL instance
  - [x] 4.6 Verify database schema matches specification
  - [x] 4.7 Verify all integration tests pass

- [x] 5. Implement repository pattern and data access layer
  - [x] 5.1 Write tests for generic repository operations
  - [x] 5.2 Create IRepository and IUnitOfWork interfaces
  - [x] 5.3 Implement generic Repository base class with common operations
  - [x] 5.4 Create specific repositories for each entity as needed
  - [x] 5.5 Implement UnitOfWork pattern for transaction management
  - [x] 5.6 Add query specifications for complex queries
  - [x] 5.7 Verify all tests pass