using CMS.ShipmentService.Domain.Enums;

namespace CMS.ShipmentService.Domain.StateMachine;

public static class ShipmentStateMachine
{
    public static readonly Dictionary<ShipmentStatus, HashSet<ShipmentStatus>> ValidTransitions = new()
    {
        [ShipmentStatus.Pending] = new() { ShipmentStatus.Assigned, ShipmentStatus.PickedUp, ShipmentStatus.InTransit, ShipmentStatus.AtWarehouse, ShipmentStatus.OutForDelivery, ShipmentStatus.Cancelled, ShipmentStatus.OnHold },
        [ShipmentStatus.Assigned] = new() { ShipmentStatus.PickedUp, ShipmentStatus.InTransit, ShipmentStatus.AtWarehouse, ShipmentStatus.OutForDelivery, ShipmentStatus.Cancelled, ShipmentStatus.OnHold },
        [ShipmentStatus.PickedUp] = new() { ShipmentStatus.InTransit, ShipmentStatus.AtWarehouse, ShipmentStatus.OutForDelivery, ShipmentStatus.OnHold },
        [ShipmentStatus.InTransit] = new() { ShipmentStatus.AtWarehouse, ShipmentStatus.OutForDelivery, ShipmentStatus.Delivered, ShipmentStatus.OnHold },
        [ShipmentStatus.AtWarehouse] = new() { ShipmentStatus.InTransit, ShipmentStatus.OutForDelivery, ShipmentStatus.Delivered, ShipmentStatus.OnHold },
        [ShipmentStatus.OutForDelivery] = new() { ShipmentStatus.Delivered, ShipmentStatus.FailedDelivery, ShipmentStatus.ReturnedToWarehouse, ShipmentStatus.OnHold },
        [ShipmentStatus.FailedDelivery] = new() { ShipmentStatus.OutForDelivery, ShipmentStatus.ReturnedToWarehouse, ShipmentStatus.Cancelled, ShipmentStatus.OnHold },
        [ShipmentStatus.ReturnedToWarehouse] = new() { ShipmentStatus.OutForDelivery, ShipmentStatus.Cancelled, ShipmentStatus.AtWarehouse, ShipmentStatus.OnHold },
        [ShipmentStatus.Delivered] = new() { ShipmentStatus.Pending, ShipmentStatus.InTransit, ShipmentStatus.Cancelled },
        [ShipmentStatus.Cancelled] = new() { ShipmentStatus.Pending },
        [ShipmentStatus.OnHold] = new() { ShipmentStatus.Pending, ShipmentStatus.Assigned, ShipmentStatus.PickedUp, ShipmentStatus.InTransit, ShipmentStatus.AtWarehouse, ShipmentStatus.OutForDelivery, ShipmentStatus.Cancelled }
    };

    public static bool CanTransition(ShipmentStatus from, ShipmentStatus to)
        => ValidTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
}
