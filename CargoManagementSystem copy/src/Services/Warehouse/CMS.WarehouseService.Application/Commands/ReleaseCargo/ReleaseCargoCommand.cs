using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.ReleaseCargo;

public record ReleaseCargoCommand(ReleaseCargoRequest Request, string ActorUserId) : IRequest<ApiResponse<CargoReceiptDto>>;
