using CMS.ShipmentService.Domain.Enums;
using CMS.ShipmentService.Domain.StateMachine;
using FluentAssertions;
using NUnit.Framework;

namespace CMS.Tests.Unit.Domain;

[TestFixture]
[Category("UC-SHP-003")]
public class ShipmentStateMachineTests
{
    [TestCase(ShipmentStatus.Pending, ShipmentStatus.Assigned, true)]
    [TestCase(ShipmentStatus.Pending, ShipmentStatus.Cancelled, true)]
    [TestCase(ShipmentStatus.Pending, ShipmentStatus.Delivered, false)]
    [TestCase(ShipmentStatus.Assigned, ShipmentStatus.PickedUp, true)]
    [TestCase(ShipmentStatus.Assigned, ShipmentStatus.Cancelled, true)]
    [TestCase(ShipmentStatus.Assigned, ShipmentStatus.Delivered, false)]
    [TestCase(ShipmentStatus.PickedUp, ShipmentStatus.InTransit, true)]
    [TestCase(ShipmentStatus.PickedUp, ShipmentStatus.AtWarehouse, true)]
    [TestCase(ShipmentStatus.PickedUp, ShipmentStatus.Delivered, false)]
    [TestCase(ShipmentStatus.InTransit, ShipmentStatus.AtWarehouse, true)]
    [TestCase(ShipmentStatus.InTransit, ShipmentStatus.OutForDelivery, true)]
    [TestCase(ShipmentStatus.InTransit, ShipmentStatus.Pending, false)]
    [TestCase(ShipmentStatus.AtWarehouse, ShipmentStatus.InTransit, true)]
    [TestCase(ShipmentStatus.AtWarehouse, ShipmentStatus.OutForDelivery, true)]
    [TestCase(ShipmentStatus.AtWarehouse, ShipmentStatus.Pending, false)]
    [TestCase(ShipmentStatus.OutForDelivery, ShipmentStatus.Delivered, true)]
    [TestCase(ShipmentStatus.OutForDelivery, ShipmentStatus.FailedDelivery, true)]
    [TestCase(ShipmentStatus.OutForDelivery, ShipmentStatus.Pending, false)]
    [TestCase(ShipmentStatus.FailedDelivery, ShipmentStatus.OutForDelivery, true)]
    [TestCase(ShipmentStatus.FailedDelivery, ShipmentStatus.ReturnedToWarehouse, true)]
    [TestCase(ShipmentStatus.FailedDelivery, ShipmentStatus.Delivered, false)]
    [TestCase(ShipmentStatus.ReturnedToWarehouse, ShipmentStatus.OutForDelivery, true)]
    [TestCase(ShipmentStatus.ReturnedToWarehouse, ShipmentStatus.Cancelled, true)]
    [TestCase(ShipmentStatus.ReturnedToWarehouse, ShipmentStatus.Delivered, false)]
    [TestCase(ShipmentStatus.Delivered, ShipmentStatus.InTransit, false)]
    [TestCase(ShipmentStatus.Delivered, ShipmentStatus.Cancelled, false)]
    [TestCase(ShipmentStatus.Cancelled, ShipmentStatus.Pending, false)]
    [TestCase(ShipmentStatus.Cancelled, ShipmentStatus.Assigned, false)]
    public void CanTransition_ReturnsExpectedResult(ShipmentStatus from, ShipmentStatus to, bool expected)
    {
        var result = ShipmentStateMachine.CanTransition(from, to);

        result.Should().Be(expected);
    }

    [Test]
    public void Delivered_IsTerminalState_NoTransitionsAllowed()
    {
        var allStatuses = Enum.GetValues<ShipmentStatus>();

        foreach (var status in allStatuses)
        {
            ShipmentStateMachine.CanTransition(ShipmentStatus.Delivered, status).Should().BeFalse(
                $"Delivered should not transition to {status}");
        }
    }

    [Test]
    public void Cancelled_IsTerminalState_NoTransitionsAllowed()
    {
        var allStatuses = Enum.GetValues<ShipmentStatus>();

        foreach (var status in allStatuses)
        {
            ShipmentStateMachine.CanTransition(ShipmentStatus.Cancelled, status).Should().BeFalse(
                $"Cancelled should not transition to {status}");
        }
    }
}
