# Technical Specification

This is the technical specification for the spec detailed in @.agent-os/specs/2025-09-02-client-responsible-person-crud/spec.md

## Technical Requirements

- Create ResponsiblePerson entity with proper Entity Framework Core mapping and relationships to Client entity
- Implement Razor Pages with PageModel classes for CRUD operations following existing patterns
- Design UI using Bootstrap 5.3 components and DataTables.js for listing responsible persons
- Add client-side validation using jQuery Validation for email format and required fields
- Implement server-side validation in PageModel OnPost methods to ensure only one primary contact per client
- Create responsive UI components that integrate seamlessly with existing client detail pages
- Follow Clean Architecture patterns with proper separation of concerns (PageModel → Service → Repository)
- Use Entity Framework Core for data persistence with proper foreign key relationships
- Implement proper error handling and TempData messages following existing Razor Pages patterns
- Create partial views for responsible person forms to enable reusability across Create/Edit pages