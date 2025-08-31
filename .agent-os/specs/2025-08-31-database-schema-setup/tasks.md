# Spec Tasks

## Tasks

- [x] 1. Set up Entity Framework Core and PostgreSQL dependencies
  - [x] 1.1 Write tests for database connection and context initialization
  - [x] 1.2 Install required NuGet packages (Npgsql.EntityFrameworkCore.PostgreSQL, EFCore.NamingConventions)
  - [x] 1.3 Configure connection string in appsettings.json for development environment
  - [x] 1.4 Create ApplicationDbContext inheriting from IdentityDbContext
  - [x] 1.5 Configure DbContext options and PostgreSQL provider in Program.cs
  - [x] 1.6 Verify all tests pass

- [ ] 2. Implement core domain entities and configurations
  - [ ] 2.1 Write tests for Client entity CRUD operations
  - [ ] 2.2 Create Client entity with required properties and validation attributes
  - [ ] 2.3 Create CostCenter entity with hierarchical structure support
  - [ ] 2.4 Create ResponsiblePerson entity linked to CostCenter
  - [ ] 2.5 Create Invoice and InvoiceStatus entities with relationships
  - [ ] 2.6 Create InvoiceHistory entity for audit trail
  - [ ] 2.7 Implement IEntityTypeConfiguration for each entity with Fluent API
  - [ ] 2.8 Verify all tests pass

- [ ] 3. Configure database constraints and indexes
  - [ ] 3.1 Write tests for unique constraints and check constraints
  - [ ] 3.2 Configure unique constraints (Client.TaxId, CostCenter.Code, Client+InvoiceNumber)
  - [ ] 3.3 Add check constraints for business rules (amount > 0, date validations)
  - [ ] 3.4 Create composite and covering indexes for query optimization
  - [ ] 3.5 Configure cascade delete rules and referential integrity
  - [ ] 3.6 Verify all tests pass

- [ ] 4. Create and execute initial database migration
  - [ ] 4.1 Write integration tests using TestContainers for PostgreSQL
  - [ ] 4.2 Generate initial migration using EF Core tools
  - [ ] 4.3 Review generated migration for correctness
  - [ ] 4.4 Add seed data for InvoiceStatus and SystemSetting tables
  - [ ] 4.5 Test migration execution in local PostgreSQL instance
  - [ ] 4.6 Verify database schema matches specification
  - [ ] 4.7 Verify all integration tests pass

- [ ] 5. Implement repository pattern and data access layer
  - [ ] 5.1 Write tests for generic repository operations
  - [ ] 5.2 Create IRepository and IUnitOfWork interfaces
  - [ ] 5.3 Implement generic Repository base class with common operations
  - [ ] 5.4 Create specific repositories for each entity as needed
  - [ ] 5.5 Implement UnitOfWork pattern for transaction management
  - [ ] 5.6 Add query specifications for complex queries
  - [ ] 5.7 Verify all tests pass