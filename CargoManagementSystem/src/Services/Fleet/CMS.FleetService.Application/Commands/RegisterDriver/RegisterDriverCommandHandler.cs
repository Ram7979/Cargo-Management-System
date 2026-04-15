using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.RegisterDriver;

public class RegisterDriverCommandHandler : IRequestHandler<RegisterDriverCommand, ApiResponse<DriverDto>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public RegisterDriverCommandHandler(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DriverDto>> Handle(RegisterDriverCommand command, CancellationToken cancellationToken)
    {
        var driver = Driver.Create(
            command.Request.UserId,
            command.Request.EmployeeId,
            command.Request.LicenseNumber,
            command.Request.LicenseExpiry,
            command.Request.Phone,
            command.Request.DateJoined);

        if (driver.IsLicenseExpired())
            throw new ValidationException(new[] { "Driver license is already expired." });

        await _driverRepository.AddAsync(driver);

        var dto = _mapper.Map<DriverDto>(driver);
        return ApiResponse<DriverDto>.Ok(dto, "Driver registered successfully.");
    }
}
