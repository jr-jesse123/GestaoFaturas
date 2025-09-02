# Product Roadmap

## Phase 1: Core MVP

**Goal:** Establish fundamental invoice management capabilities
**Success Criteria:** Successfully track and manage 100+ invoices with basic CRUD operations

### Features

- [x] Database schema design and setup - PostgreSQL with Entity Framework Core `M`
- [x] Client management CRUD operations - Add, edit, delete, list clients `M`
- [ ] Client Responsible Person CRUD operations - Add, edit, delete, list Responsible persons `M`
- [ ] Cost Center management - Create and manage cost centers `S`
- [ ] Basic invoice tracking - Create, update, and track invoice status `L`
- [ ] Invoice status workflow - Expected, Received, Under Analysis, Analyzed, Approved `M`
- [ ] User authentication and authorization - Basic role-based access `M`

### Dependencies

- PostgreSQL database setup
- ASP.NET Core 8.0 environment
- Entity Framework Core configuration

## Phase 2: Automation & Intelligence

**Goal:** Add automated validation and notification capabilities
**Success Criteria:** 90% of invoices automatically validated against ranges with zero missed notifications

### Features

- [ ] Range validation system - Define and validate expected invoice ranges `M`
- [ ] Automatic anomaly detection - Flag invoices exceeding ranges `S`
- [ ] Email notification system - SendGrid/SMTP integration `M`
- [ ] Status change triggers - Automated emails on invoice status updates `S`
- [ ] Analysis documentation - Record and track analysis results `S`
- [ ] Dashboard with key metrics - Real-time invoice processing overview `M`
- [ ] Bulk invoice import - CSV/Excel file upload capability `M`

### Dependencies

- Email service provider account (SendGrid)
- Notification template system
- Background job processing (TinkerQ)

## Phase 3: Reporting & Analytics

**Goal:** Provide comprehensive reporting and advanced features
**Success Criteria:** Generate actionable insights reducing billing errors by 50%

### Features

- [ ] Advanced reporting module - Custom reports by period, client, cost center `L`
- [ ] Export functionality - PDF, Excel, CSV export options `S`
- [ ] Audit trail system - Complete history of all changes `M`
- [ ] Invoice comparison tools - Month-over-month, year-over-year analysis `M`
- [ ] Cost center budget tracking - Budget vs actual reporting `M`
- [ ] Mobile-responsive interface - Full functionality on mobile devices `M`

### Dependencies

- Reporting engine setup
- Chart.js integration
- API documentation framework
