using CMS.WarehouseService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.ReceiveCargo;

public record ReceiveCargoCommand(ReceiveCargoRequest Request, string ActorUserId) : IRequest<ApiResponse<CargoReceiptDto>>;
