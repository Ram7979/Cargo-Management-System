using AutoMapper;
using CMS.ShipmentService.Application.DTOs;
using CMS.ShipmentService.Application.Events;
using CMS.ShipmentService.Application.Interfaces;
using CMS.ShipmentService.Domain.Entities;
using CMS.ShipmentService.Domain.Enums;
using CMS.ShipmentService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Commands.CreateShipment;

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, ApiResponse<ShipmentDto>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICustomerServiceClient _customerServiceClient;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CreateShipmentCommandHandler(
        IShipmentRepository shipmentRepository,
        ICustomerServiceClient customerServiceClient,
        IMediator mediator,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _customerServiceClient = customerServiceClient;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ShipmentDto>> Handle(CreateShipmentCommand command, CancellationToken cancellationToken)
    {
        var customerExists = await _customerServiceClient.CustomerExistsAsync(command.Request.CustomerId);
        if (!customerExists)
            throw new UnprocessableException($"Customer with ID '{command.Request.CustomerId}' does not exist.");

        // Parse enums
        if (!Enum.TryParse<CargoType>(command.Request.CargoType, ignoreCase: true, out var cargoType))
            throw new ValidationException(new[] { $"Invalid CargoType. Valid values: {string.Join(", ", Enum.GetNames<CargoType>())}" });

        if (!Enum.TryParse<PaymentMode>(command.Request.PaymentMode, ignoreCase: true, out var paymentMode))
            throw new ValidationException(new[] { $"Invalid PaymentMode. Valid values: {string.Join(", ", Enum.GetNames<PaymentMode>())}" });

        // Validate DG requirements
        var isDangerousGoods = cargoType == CargoType.Hazardous;

        // Validate international requirements
        var isInternational = string.Equals(command.Request.ServiceType, "International", StringComparison.OrdinalIgnoreCase);
        if (isInternational && (string.IsNullOrWhiteSpace(command.Request.HsCode) || string.IsNullOrWhiteSpace(command.Request.CountryOfOrigin)))
            throw new ValidationException(new[] { "International shipments require HsCode and CountryOfOrigin." });

        // Generate collision-safe tracking number
        var year = DateTime.UtcNow.Year;
        var seq = await _shipmentRepository.GetNextSequenceAsync(year);
        var trackingNumber = $"CMS-{year}-{seq:D6}";

        var shipment = Shipment.Create(
            trackingNumber,
            command.Request.CustomerId,
            command.Request.SenderName, command.Request.OriginAddress, command.Request.SenderCity,
            command.Request.SenderZip, command.Request.SenderCountry, command.Request.SenderContact,
            command.Request.RecipientName, command.Request.DestinationAddress, command.Request.RecipientCity,
            command.Request.RecipientZip, command.Request.RecipientCountry, command.Request.RecipientContact,
            command.Request.WeightKg, command.Request.VolumeCbm, command.Request.Quantity,
            cargoType, command.Request.DeclaredValue, command.Request.CargoDescription,
            command.Request.ServiceType, paymentMode,
            command.Request.HsCode, command.Request.CountryOfOrigin, isDangerousGoods);

        // Set estimated delivery date
        var estimatedDays = command.Request.ServiceType.ToLower() switch
        {
            "same-day" => 0,
            "express" => 1,
            "international" => 7,
            _ => 3
        };
        shipment.SetEstimatedDeliveryDate(DateTime.UtcNow.AddDays(estimatedDays));
        shipment.SetBolUrl($"bol/{trackingNumber}.pdf");

        await _shipmentRepository.AddAsync(shipment);

        await _mediator.Publish(new ShipmentCreatedEvent(shipment.Id, shipment.TrackingNumber, shipment.CustomerId), cancellationToken);

        var dto = _mapper.Map<ShipmentDto>(shipment);
        return ApiResponse<ShipmentDto>.Ok(dto, "Shipment created successfully.");
    }
}
