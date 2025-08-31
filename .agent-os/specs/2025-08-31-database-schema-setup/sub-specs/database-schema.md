# Database Schema

## Migration Strategy

### Initial Migration
```csharp
// Run in Package Manager Console
Add-Migration InitialSchema -Context ApplicationDbContext
Update-Database -Context ApplicationDbContext
```

### Production Deployment
```bash
# Generate SQL script for review
dotnet ef migrations script -o initial_schema.sql

# Apply after review
psql -h [server] -U [username] -d [database] -f initial_schema.sql
```

## Performance Considerations

### Index Strategy
- **Covering Indexes**: Composite index on invoices table covers most common query patterns
- **Partial Indexes**: Active-only indexes reduce index size and improve performance
- **Unique Indexes**: Enforce business rules at database level while providing query optimization

### Query Optimization
- **Join Reduction**: Denormalized certain fields to reduce join operations
- **Index-Only Scans**: Covering indexes enable index-only scans for common queries
- **Partition Strategy**: Ready for future partitioning by date ranges if data volume requires

### Data Integrity
- **Check Constraints**: Business rules enforced at database level
- **Foreign Keys**: Referential integrity with appropriate cascade rules
- **Unique Constraints**: Prevent duplicate data at database level
- **NOT NULL**: Required fields enforced to prevent incomplete data