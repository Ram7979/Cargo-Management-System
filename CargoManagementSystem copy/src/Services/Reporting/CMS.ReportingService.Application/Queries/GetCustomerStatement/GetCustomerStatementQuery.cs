using CMS.ReportingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetCustomerStatement;

public record GetCustomerStatementQuery(
    string CustomerId,
    DateTime FromDate,
    DateTime ToDate)
    : IRequest<ApiResponse<CustomerStatementDto>>;
