# Spec Tasks

These are the tasks to be completed for the spec detailed in @.agent-os/specs/2025-09-01-client-management-crud/spec.md

> Created: 2025-09-01
> Status: Ready for Implementation

## Tasks

### 1. Data Layer Implementation

- [x] 1.1 Write tests for ClientDto, CreateClientDto, and UpdateClientDto classes
- [x] 1.2 Create ClientDto with all client properties for data transfer
- [x] 1.3 Create CreateClientDto for client creation with validation attributes
- [x] 1.4 Create UpdateClientDto for client updates with validation attributes
- [x] 1.5 Implement static mapping functions between entities and DTOs
- [x] 1.6 Add server-side validation for tax ID format (CNPJ/CPF)
- [x] 1.7 Add email format validation and required field constraints
- [x] 1.8 Verify all data layer tests pass

### 2. Service Layer Implementation

- [x] 2.1 Write tests for ClientService with all CRUD operations
- [x] 2.2 Create IClientService interface with method signatures
- [x] 2.3 Implement ClientService with dependency injection for IClientRepository
- [x] 2.4 Add CreateClientAsync method with business logic and validation
- [x] 2.5 Add GetClientByIdAsync and GetClientsAsync methods with pagination
- [x] 2.6 Add UpdateClientAsync method with audit trail tracking
- [x] 2.7 Add DeactivateClientAsync method for soft delete (IsActive = false)
- [x] 2.8 Implement search and filtering functionality (name, tax ID, email)
- [x] 2.9 Register ClientService in dependency injection container
- [x] 2.10 Verify all service layer tests pass

### 3. Razor Pages Implementation

- [ ] 3.1 Write tests for all Page Models and their handlers
- [ ] 3.2 Create Clients/Index.cshtml page with client listing and search functionality
- [ ] 3.3 Create Clients/Index.cshtml.cs PageModel with pagination and filtering logic
- [ ] 3.4 Create Clients/Details.cshtml page for comprehensive client information display
- [ ] 3.5 Create Clients/Details.cshtml.cs PageModel with client retrieval logic
- [ ] 3.6 Create Clients/Create.cshtml page with client creation form
- [ ] 3.7 Create Clients/Create.cshtml.cs PageModel with POST handler for client creation
- [ ] 3.8 Create Clients/Edit.cshtml page with client editing form
- [ ] 3.9 Create Clients/Edit.cshtml.cs PageModel with GET/POST handlers for updates
- [ ] 3.10 Verify all Razor Pages tests pass

### 4. UI/UX Implementation

- [ ] 4.1 Write tests for client-side validation and JavaScript functionality
- [ ] 4.2 Implement responsive Bootstrap 5.3 layout for all client pages
- [ ] 4.3 Add client-side JavaScript validation for immediate user feedback
- [ ] 4.4 Implement Bootstrap toast notifications for success/error messages
- [ ] 4.5 Add pagination controls with configurable page sizes
- [ ] 4.6 Implement search filters with case-insensitive matching
- [ ] 4.7 Add breadcrumb navigation for client management workflow
- [ ] 4.8 Ensure mobile-friendly design for tablet and phone usage
- [ ] 4.9 Add status indicators for active/inactive clients
- [ ] 4.10 Verify all UI/UX tests pass

### 5. Integration and Testing

- [ ] 5.1 Write integration tests for complete client management workflows
- [ ] 5.2 Add navigation links to main dashboard for client management access
- [ ] 5.3 Test client creation workflow with validation error handling
- [ ] 5.4 Test client listing with pagination, search, and filtering
- [ ] 5.5 Test client details view with associated cost centers display
- [ ] 5.6 Test client update workflow with audit trail verification
- [ ] 5.7 Test client deactivation and reactivation functionality
- [ ] 5.8 Perform cross-browser testing for compatibility
- [ ] 5.9 Verify data integrity constraints and error handling
- [ ] 5.10 Verify all integration tests pass
