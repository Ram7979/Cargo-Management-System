using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.RegisterDriver;

public record RegisterDriverCommand(RegisterDriverRequest Request) : IRequest<ApiResponse<DriverDto>>;
