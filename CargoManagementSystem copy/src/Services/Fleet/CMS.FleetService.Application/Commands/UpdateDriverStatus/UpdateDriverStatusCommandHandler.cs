using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.UpdateDriverStatus;

public class UpdateDriverStatusCommandHandler : IRequestHandler<UpdateDriverStatusCommand, ApiResponse<DriverDto>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public UpdateDriverStatusCommandHandler(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DriverDto>> Handle(UpdateDriverStatusCommand command, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(command.DriverId)
            ?? throw new NotFoundException("Driver", command.DriverId);

        if (!Enum.TryParse<DriverStatus>(command.Request.Status, ignoreCase: true, out var status))
            throw new ValidationException(new[] { $"Invalid status. Valid values: {string.Join(", ", Enum.GetNames<DriverStatus>())}" });

        driver.SetStatus(status);
        await _driverRepository.UpdateAsync(driver);

        var dto = _mapper.Map<DriverDto>(driver);
        return ApiResponse<DriverDto>.Ok(dto, "Driver status updated.");
    }
}
