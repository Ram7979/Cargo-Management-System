using CMS.ShipmentService.Domain.Enums;

namespace CMS.ShipmentService.Domain.StateMachine;

public static class ShipmentStateMachine
{
    public static readonly Dictionary<ShipmentStatus, HashSet<ShipmentStatus>> ValidTransitions = new()
    {
        [ShipmentStatus.Pending] = new() { ShipmentStatus.Assigned, ShipmentStatus.Cancelled },
        [ShipmentStatus.Assigned] = new() { ShipmentStatus.PickedUp, ShipmentStatus.Cancelled },
        [ShipmentStatus.PickedUp] = new() { ShipmentStatus.InTransit, ShipmentStatus.AtWarehouse },
        [ShipmentStatus.InTransit] = new() { ShipmentStatus.AtWarehouse, ShipmentStatus.OutForDelivery },
        [ShipmentStatus.AtWarehouse] = new() { ShipmentStatus.InTransit, ShipmentStatus.OutForDelivery },
        [ShipmentStatus.OutForDelivery] = new() { ShipmentStatus.Delivered, ShipmentStatus.FailedDelivery },
        [ShipmentStatus.FailedDelivery] = new() { ShipmentStatus.OutForDelivery, ShipmentStatus.ReturnedToWarehouse },
        [ShipmentStatus.ReturnedToWarehouse] = new() { ShipmentStatus.OutForDelivery, ShipmentStatus.Cancelled },
        [ShipmentStatus.Delivered] = new(),
        [ShipmentStatus.Cancelled] = new()
    };

    public static bool CanTransition(ShipmentStatus from, ShipmentStatus to)
        => ValidTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
}
