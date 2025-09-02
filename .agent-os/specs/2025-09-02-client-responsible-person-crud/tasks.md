# Spec Tasks

These are the tasks to be completed for the spec detailed in @.agent-os/specs/2025-09-02-client-responsible-person-crud/spec.md

> Created: 2025-09-02
> Status: Ready for Implementation

## Tasks

- [ ] 1. Database and Entity Layer Implementation
  - [ ] 1.1 Write unit tests for ResponsiblePerson entity and relationships
  - [ ] 1.2 Create ResponsiblePerson entity with proper Entity Framework mappings
  - [ ] 1.3 Create and apply database migration for ResponsiblePersons table
  - [ ] 1.4 Implement Repository pattern for ResponsiblePerson operations
  - [ ] 1.5 Add validation logic for primary contact uniqueness constraint
  - [ ] 1.6 Test database operations and foreign key relationships
  - [ ] 1.7 Verify all entity and repository tests pass

- [ ] 2. Razor Pages Structure and Routing
  - [ ] 2.1 Write integration tests for all ResponsiblePerson page routes
  - [ ] 2.2 Create Index page for listing responsible persons by client
  - [ ] 2.3 Implement Create page with proper form binding and validation
  - [ ] 2.4 Build Edit page with data loading and update functionality
  - [ ] 2.5 Add Details page for read-only responsible person display
  - [ ] 2.6 Create Delete page with confirmation workflow
  - [ ] 2.7 Configure routing and navigation between pages
  - [ ] 2.8 Verify all page routing and integration tests pass

- [ ] 3. UI Components and User Experience
  - [ ] 3.1 Write UI tests for form validation and user interactions
  - [ ] 3.2 Create reusable partial view for responsible person forms
  - [ ] 3.3 Implement Bootstrap responsive table with action buttons
  - [ ] 3.4 Add primary contact badge/highlighting in list views
  - [ ] 3.5 Implement jQuery validation for client-side form feedback
  - [ ] 3.6 Add proper error messaging and success notifications
  - [ ] 3.7 Ensure responsive design across mobile and desktop
  - [ ] 3.8 Verify all UI and validation tests pass

- [ ] 4. Business Logic and Validation
  - [ ] 4.1 Write tests for primary contact validation business rules
  - [ ] 4.2 Implement server-side validation in PageModel OnPost methods
  - [ ] 4.3 Add email format validation and required field checking
  - [ ] 4.4 Create logic to handle primary contact uniqueness per client
  - [ ] 4.5 Implement proper error handling and user feedback
  - [ ] 4.6 Add data persistence with proper transaction handling
  - [ ] 4.7 Test edge cases and validation scenarios
  - [ ] 4.8 Verify all business logic and validation tests pass

- [ ] 5. Integration and End-to-End Testing
  - [ ] 5.1 Write comprehensive end-to-end tests covering full CRUD workflow
  - [ ] 5.2 Test integration with existing client management pages
  - [ ] 5.3 Verify primary contact constraint works across all operations
  - [ ] 5.4 Test form validation, error handling, and user feedback
  - [ ] 5.5 Validate responsive design and cross-browser compatibility
  - [ ] 5.6 Run full test suite and fix any integration issues
  - [ ] 5.7 Perform manual testing of complete user workflow
  - [ ] 5.8 Verify all tests pass and feature is production-ready