using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetInventory;

public class GetInventoryQueryHandler : IRequestHandler<GetInventoryQuery, ApiResponse<InventorySummaryDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICargoReceiptRepository _receiptRepository;

    public GetInventoryQueryHandler(
        IWarehouseRepository warehouseRepository,
        ICargoReceiptRepository receiptRepository)
    {
        _warehouseRepository = warehouseRepository;
        _receiptRepository = receiptRepository;
    }

    public async Task<ApiResponse<InventorySummaryDto>> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId)
            ?? throw new NotFoundException("Warehouse", request.WarehouseId);

        var activeBins = warehouse.Bins.Where(b => b.IsActive).ToList();
        var totalBins = activeBins.Count;
        var occupiedBins = activeBins.Count(b => b.IsOccupied);
        var availableBins = totalBins - occupiedBins;
        var totalCapacityKg = activeBins.Sum(b => b.CapacityKg);
        var occupancyPercent = totalBins > 0 ? Math.Round((double)occupiedBins / totalBins * 100, 2) : 0;

        // Get active receipts (not yet released) for this warehouse
        var (receipts, _) = await _receiptRepository.GetPagedAsync(
            page: 1, pageSize: int.MaxValue,
            warehouseId: request.WarehouseId);

        var activeReceipts = receipts.Where(r => !r.ReleasedAt.HasValue).ToList();

        var currentShipments = activeReceipts.Select(r =>
        {
            var bin = warehouse.Bins.FirstOrDefault(b => b.Id == r.BinId);
            return new CurrentShipmentDto
            {
                ShipmentId = r.ShipmentId,
                TrackingNumber = r.TrackingNumber,
                BinCode = bin?.BinCode ?? "Unknown",
                ArrivedAt = r.ReceivedAt
            };
        }).ToList();

        var summary = new InventorySummaryDto
        {
            WarehouseId = warehouse.Id,
            WarehouseName = warehouse.Name,
            TotalBins = totalBins,
            OccupiedBins = occupiedBins,
            AvailableBins = availableBins,
            TotalCapacityKg = totalCapacityKg,
            OccupancyPercent = occupancyPercent,
            CurrentShipments = currentShipments
        };

        return ApiResponse<InventorySummaryDto>.Ok(summary);
    }
}
