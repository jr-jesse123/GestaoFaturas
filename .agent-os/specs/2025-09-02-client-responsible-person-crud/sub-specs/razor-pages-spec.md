# Razor Pages Specification

This is the Razor Pages specification for the spec detailed in @.agent-os/specs/2025-09-02-client-responsible-person-crud/spec.md

## Razor Pages Structure

### /Pages/Clients/{clientId}/ResponsiblePersons/Index.cshtml

**Purpose:** Display list of responsible persons for a specific client

**Route:** `/Clients/{clientId}/ResponsiblePersons`

**PageModel Properties:**
- ClientId (int) - Client identifier from route
- Client (Client) - Client entity for display
- ResponsiblePersons (List<ResponsiblePerson>) - List of responsible persons

**Features:**
- Bootstrap table with responsive design
- Action buttons for Create, Edit, Delete operations
- Primary contact highlighting/badge display

### /Pages/Clients/{clientId}/ResponsiblePersons/Create.cshtml

**Purpose:** Create new responsible person for a client

**Route:** `/Clients/{clientId}/ResponsiblePersons/Create`

**PageModel Properties:**
- ClientId (int) - Client identifier from route
- ResponsiblePerson (ResponsiblePerson) - Bound model for form data

**OnPost Method:**
- Validate required fields (Name, Email, phone)
- Check email format validation
- Ensure only one primary contact per client
- Save to database with proper error handling
- Redirect to Index on success or return page with errors

### /Pages/Clients/{clientId}/ResponsiblePersons/Edit.cshtml

**Purpose:** Edit existing responsible person

**Route:** `/Clients/{clientId}/ResponsiblePersons/Edit/{id}`

**PageModel Properties:**
- Id (int) - ResponsiblePerson identifier
- ClientId (int) - Client identifier
- ResponsiblePerson (ResponsiblePerson) - Bound model with existing data

**OnGet Method:**
- Load existing responsible person data
- Populate form fields

**OnPost Method:**
- Validate form data
- Handle primary contact uniqueness validation
- Update database with proper concurrency handling
- Redirect to Index or return with validation errors

### /Pages/Clients/{clientId}/ResponsiblePersons/Details.cshtml

**Purpose:** Display detailed view of responsible person

**Route:** `/Clients/{clientId}/ResponsiblePersons/Details/{id}`

**PageModel Properties:**
- ResponsiblePerson (ResponsiblePerson) - Full responsible person data
- Client (Client) - Associated client information

**Features:**
- Read-only display of all responsible person fields
- Navigation links back to client and responsible persons list
- Edit and Delete action buttons

### /Pages/Clients/{clientId}/ResponsiblePersons/Delete.cshtml

**Purpose:** Confirm deletion of responsible person

**Route:** `/Clients/{clientId}/ResponsiblePersons/Delete/{id}`

**PageModel Properties:**
- ResponsiblePerson (ResponsiblePerson) - Data to be deleted

**OnPost Method:**
- Soft delete or hard delete based on business requirements
- Handle foreign key constraints properly
- Redirect to Index after successful deletion

## Shared Components

### Partial Views

- **_ResponsiblePersonForm.cshtml** - Reusable form partial for Create/Edit pages
- **_ResponsiblePersonCard.cshtml** - Display component for individual responsible person
- **_DeleteConfirmation.cshtml** - Standard delete confirmation modal

### Validation

- Client-side: jQuery Validation for immediate feedback
- Server-side: Data Annotations and custom validation in PageModel
- Primary contact uniqueness validation across client scope
