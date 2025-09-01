using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Data.Specifications;

public class ActiveInvoicesSpecification : BaseSpecification<Invoice>
{
    public ActiveInvoicesSpecification() 
        : base(i => i.IsActive)
    {
        AddInclude(i => i.Client);
        AddInclude(i => i.CostCenter);
        AddInclude(i => i.InvoiceStatus);
        ApplyOrderByDescending(i => i.IssueDate);
    }
}

public class OverdueInvoicesSpecification : BaseSpecification<Invoice>
{
    public OverdueInvoicesSpecification(DateTime referenceDate) 
        : base(i => i.DueDate < referenceDate && i.PaidDate == null)
    {
        AddInclude(i => i.Client);
        AddInclude(i => i.CostCenter);
        AddInclude(i => i.InvoiceStatus);
        ApplyOrderBy(i => i.DueDate);
    }
}

public class InvoicesByClientSpecification : BaseSpecification<Invoice>
{
    public InvoicesByClientSpecification(int clientId) 
        : base(i => i.ClientId == clientId)
    {
        AddInclude(i => i.CostCenter);
        AddInclude(i => i.InvoiceStatus);
        ApplyOrderByDescending(i => i.IssueDate);
    }
}

public class InvoicesByDateRangeSpecification : BaseSpecification<Invoice>
{
    public InvoicesByDateRangeSpecification(DateTime startDate, DateTime endDate) 
        : base(i => i.IssueDate >= startDate && i.IssueDate <= endDate)
    {
        AddInclude(i => i.Client);
        AddInclude(i => i.CostCenter);
        AddInclude(i => i.InvoiceStatus);
        ApplyOrderByDescending(i => i.IssueDate);
    }
}

public class InvoiceWithFullDetailsSpecification : BaseSpecification<Invoice>
{
    public InvoiceWithFullDetailsSpecification(int invoiceId) 
        : base(i => i.Id == invoiceId)
    {
        AddInclude(i => i.Client);
        AddInclude(i => i.CostCenter);
        AddInclude("CostCenter.ResponsiblePersons");
        AddInclude(i => i.InvoiceStatus);
        AddInclude("InvoiceHistories.FromStatus");
        AddInclude("InvoiceHistories.ToStatus");
    }
}

public class PaginatedInvoicesSpecification : BaseSpecification<Invoice>
{
    public PaginatedInvoicesSpecification(int pageNumber, int pageSize, int? clientId = null, int? statusId = null)
        : base(i => (!clientId.HasValue || i.ClientId == clientId) && 
                   (!statusId.HasValue || i.InvoiceStatusId == statusId))
    {
        AddInclude(i => i.Client);
        AddInclude(i => i.CostCenter);
        AddInclude(i => i.InvoiceStatus);
        ApplyOrderByDescending(i => i.IssueDate);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}