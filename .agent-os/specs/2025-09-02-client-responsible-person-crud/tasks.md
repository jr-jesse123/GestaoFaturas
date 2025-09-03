# Spec Tasks

These are the tasks to be completed for the spec detailed in @.agent-os/specs/2025-09-02-client-responsible-person-crud/spec.md

> Created: 2025-09-02
> Status: Ready for Implementation

## Tasks

- [x] 1. Database and Entity Layer Implementation
  - [x] 1.1 Write unit tests for ResponsiblePerson entity and relationships
  - [x] 1.2 Create ResponsiblePerson entity with proper Entity Framework mappings
  - [x] 1.3 Create and apply database migration for ResponsiblePersons table
  - [x] 1.4 Implement Repository pattern for ResponsiblePerson operations
  - [x] 1.5 Add validation logic for primary contact uniqueness constraint
  - [x] 1.6 Test database operations and foreign key relationships
  - [x] 1.7 Verify all entity and repository tests pass

- [x] 2. Razor Pages Structure and Routing
  - [x] 2.1 Write integration tests for all ResponsiblePerson page routes
  - [x] 2.2 Create Index page for listing responsible persons by client
  - [x] 2.3 Implement Create page with proper form binding and validation
  - [x] 2.4 Build Edit page with data loading and update functionality
  - [x] 2.5 Add Details page for read-only responsible person display
  - [x] 2.6 Create Delete page with confirmation workflow
  - [x] 2.7 Configure routing and navigation between pages
  - [x] 2.8 Verify all page routing and integration tests pass

- [x] 3. UI Components and User Experience
  - [x] 3.1 Write UI tests for form validation and user interactions
  - [x] 3.2 Create reusable partial view for responsible person forms
  - [x] 3.3 Implement Bootstrap responsive table with action buttons
  - [x] 3.4 Add primary contact badge/highlighting in list views
  - [x] 3.5 Implement jQuery validation for client-side form feedback
  - [x] 3.6 Add proper error messaging and success notifications
  - [x] 3.7 Ensure responsive design across mobile and desktop
  - [x] 3.8 Verify all UI and validation tests pass

- [x] 4. Business Logic and Validation
  - [x] 4.1 Write tests for primary contact validation business rules
  - [x] 4.2 Implement server-side validation in PageModel OnPost methods
  - [x] 4.3 Add email format validation and required field checking
  - [x] 4.4 Create logic to handle primary contact uniqueness per client
  - [x] 4.5 Implement proper error handling and user feedback
  - [x] 4.6 Add data persistence with proper transaction handling
  - [x] 4.7 Test edge cases and validation scenarios
  - [x] 4.8 Verify all business logic and validation tests pass

- [x] 5. Integration and End-to-End Testing
  - [x] 5.1 Write comprehensive end-to-end tests covering full CRUD workflow
  - [x] 5.2 Test integration with existing client management pages
  - [x] 5.3 Verify primary contact constraint works across all operations
  - [x] 5.4 Test form validation, error handling, and user feedback
  - [x] 5.5 Validate responsive design and cross-browser compatibility
  - [x] 5.6 Run full test suite and fix any integration issues
  - [x] 5.7 Perform manual testing of complete user workflow
  - [x] 5.8 Verify all tests pass and feature is production-ready