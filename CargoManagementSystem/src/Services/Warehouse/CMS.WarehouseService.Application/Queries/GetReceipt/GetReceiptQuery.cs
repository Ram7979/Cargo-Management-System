using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetReceipt;

public record GetReceiptQuery(Guid ReceiptId) : IRequest<ApiResponse<CargoReceiptDto>>;
