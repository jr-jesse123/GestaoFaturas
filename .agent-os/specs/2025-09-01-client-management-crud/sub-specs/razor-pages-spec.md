# Razor Pages Specification

This is the Razor Pages specification for the spec detailed in @.agent-os/specs/2025-09-01-client-management-crud/spec.md

## Page Structure

### /Pages/Clients/Index.cshtml

**Purpose:** Display paginated list of clients with search and filtering capabilities
**Page Model:** ClientsIndexModel with properties for pagination, search, and filtering
**Features:**
- Paginated client table with company name, tax ID, email, and status
- Search form with company name and tax ID filtering
- Active/inactive status toggle buttons
- "Add New Client" button
- Responsive table design with mobile-friendly layout

**Handler Methods:**
- OnGetAsync() - Load clients with pagination and filtering
- OnPostSearchAsync() - Handle search form submission
- OnPostToggleStatusAsync() - Handle client status changes

### /Pages/Clients/Details.cshtml

**Purpose:** Display comprehensive client information
**Page Model:** ClientDetailsModel with full client data and associated cost centers
**Features:**
- Complete client information display
- Associated cost centers list
- Edit and back navigation buttons
- Audit trail information (created/updated dates)

**Handler Methods:**
- OnGetAsync(int id) - Load client details with related data

### /Pages/Clients/Create.cshtml

**Purpose:** Create new client form
**Page Model:** ClientCreateModel with form binding and validation
**Features:**
- Client information form with all required fields
- Tax ID validation with Brazilian CNPJ/CPF format
- Email format validation
- Cancel and save buttons
- Client-side validation with JavaScript

**Handler Methods:**
- OnGetAsync() - Initialize empty form
- OnPostAsync() - Process form submission with validation

### /Pages/Clients/Edit.cshtml

**Purpose:** Edit existing client information
**Page Model:** ClientEditModel with pre-populated form data
**Features:**
- Pre-filled form with current client data
- Same validation as Create page
- Update and cancel buttons
- Audit information display

**Handler Methods:**
- OnGetAsync(int id) - Load existing client data
- OnPostAsync(int id) - Process update with validation

## Shared Components

### _ClientFormPartial.cshtml

**Purpose:** Reusable form component for Create and Edit pages
**Features:**
- Complete client form fields
- Consistent validation styling
- Bootstrap form controls
- Tax ID input mask

### _ClientTablePartial.cshtml

**Purpose:** Reusable client table component
**Features:**
- Responsive table design
- Status badges
- Action buttons (View, Edit, Toggle Status)
- Sort headers

## Navigation Flow

1. **Index Page** → Main client listing with search and pagination
2. **Details Page** → View client information with edit option
3. **Create Page** → Add new client with validation
4. **Edit Page** → Update client information
5. **Back to Index** → After successful operations with success messages

## Model Binding and Validation

### Client Input Model

```csharp
public class ClientInputModel
{
    [Required]
    [StringLength(200)]
    public string CompanyName { get; set; }
    
    [StringLength(200)]
    public string TradeName { get; set; }
    
    [Required]
    [StringLength(14)]
    public string TaxId { get; set; }
    
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
    
    [StringLength(20)]
    public string Phone { get; set; }
    
    [StringLength(500)]
    public string Address { get; set; }
    
    [StringLength(100)]
    public string ContactPerson { get; set; }
}
```

All pages implement proper model binding, validation, and error handling with user-friendly messages.