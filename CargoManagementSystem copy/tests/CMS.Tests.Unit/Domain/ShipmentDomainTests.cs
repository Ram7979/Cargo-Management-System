using CMS.ShipmentService.Domain.Entities;
using CMS.ShipmentService.Domain.Enums;
using CMS.ShipmentService.Domain.Exceptions;
using CMS.ShipmentService.Domain.StateMachine;
using FluentAssertions;
using NUnit.Framework;

namespace CMS.Tests.Unit.Domain;

[TestFixture]
[Category("UC-SHP-001")]
public class ShipmentDomainTests
{
    private static Shipment CreateTestShipment(string trackingNumber = "CMS-2025-000001")
    {
        return Shipment.Create(
            trackingNumber,
            Guid.NewGuid(),
            "Sender Co", "123 Origin St", "Mumbai", "400001", "IN", "+91-9999999999",
            "Recipient Co", "456 Dest Ave", "Delhi", "110001", "IN", "+91-8888888888",
            10.5m, 0.5m, 1, CargoType.Standard, 5000m, "Electronics",
            "Standard", PaymentMode.Prepaid);
    }

    [Test]
    public void Create_WithValidData_SetsStatusToPending()
    {
        var shipment = CreateTestShipment();

        shipment.Status.Should().Be(ShipmentStatus.Pending);
    }

    [Test]
    public void Create_WithValidData_SetsTrackingNumber()
    {
        var shipment = CreateTestShipment("CMS-2025-000042");

        shipment.TrackingNumber.Should().Be("CMS-2025-000042");
    }

    [Test]
    public void Create_WithValidData_HasEmptyStatusHistory()
    {
        var shipment = CreateTestShipment();

        shipment.StatusHistory.Should().BeEmpty();
    }

    [Test]
    [Category("UC-SHP-003")]
    public void UpdateStatus_ValidTransition_ChangesStatus()
    {
        var shipment = CreateTestShipment();

        shipment.UpdateStatus(ShipmentStatus.Assigned, "actor-1", "Assigned to driver");

        shipment.Status.Should().Be(ShipmentStatus.Assigned);
    }

    [Test]
    [Category("UC-SHP-003")]
    public void UpdateStatus_ValidTransition_AddsHistoryEntry()
    {
        var shipment = CreateTestShipment();

        shipment.UpdateStatus(ShipmentStatus.Assigned, "actor-1", "Assigned");

        shipment.StatusHistory.Should().HaveCount(1);
        shipment.StatusHistory.First().ToStatus.Should().Be(ShipmentStatus.Assigned);
        shipment.StatusHistory.First().FromStatus.Should().Be(ShipmentStatus.Pending);
    }

    [Test]
    [Category("UC-SHP-003")]
    public void UpdateStatus_InvalidTransition_ThrowsInvalidStatusTransitionException()
    {
        var shipment = CreateTestShipment();

        var act = () => shipment.UpdateStatus(ShipmentStatus.Delivered, "actor-1", "Invalid");

        act.Should().Throw<InvalidStatusTransitionException>();
    }

    [Test]
    [Category("UC-SHP-004")]
    public void UpdateStatus_ToCancelled_SetsRefundEligibleTrue()
    {
        var shipment = CreateTestShipment();

        shipment.UpdateStatus(ShipmentStatus.Cancelled, "actor-1", "Customer request");

        shipment.RefundEligible.Should().BeTrue();
        shipment.CancelledAt.Should().NotBeNull();
    }

    [Test]
    [Category("UC-SHP-003")]
    public void UpdateStatus_MultipleTransitions_BuildsFullTimeline()
    {
        var shipment = CreateTestShipment();

        shipment.UpdateStatus(ShipmentStatus.Assigned, "actor-1", "Assigned");
        shipment.UpdateStatus(ShipmentStatus.PickedUp, "actor-1", "Picked up");
        shipment.UpdateStatus(ShipmentStatus.InTransit, "actor-1", "In transit");

        shipment.StatusHistory.Should().HaveCount(3);
        shipment.Status.Should().Be(ShipmentStatus.InTransit);
    }

    [Test]
    [Category("UC-SHP-003")]
    public void UpdateStatus_Delivered_IsTerminalState()
    {
        var shipment = CreateTestShipment();
        shipment.UpdateStatus(ShipmentStatus.Assigned, "a", "");
        shipment.UpdateStatus(ShipmentStatus.PickedUp, "a", "");
        shipment.UpdateStatus(ShipmentStatus.InTransit, "a", "");
        shipment.UpdateStatus(ShipmentStatus.OutForDelivery, "a", "");
        shipment.UpdateStatus(ShipmentStatus.Delivered, "a", "");

        var act = () => shipment.UpdateStatus(ShipmentStatus.InTransit, "a", "");

        act.Should().Throw<InvalidStatusTransitionException>();
    }

    [Test]
    [Category("UC-SHP-003")]
    public void UpdateStatus_Cancelled_IsTerminalState()
    {
        var shipment = CreateTestShipment();
        shipment.UpdateStatus(ShipmentStatus.Cancelled, "a", "");

        var act = () => shipment.UpdateStatus(ShipmentStatus.Pending, "a", "");

        act.Should().Throw<InvalidStatusTransitionException>();
    }

    [Test]
    public void SetFailureReason_SetsReasonOnShipment()
    {
        var shipment = CreateTestShipment();

        shipment.SetFailureReason(FailureReason.RecipientAbsent, DateTime.UtcNow.AddDays(1));

        shipment.LastFailureReason.Should().Be(FailureReason.RecipientAbsent);
        shipment.ReDeliveryScheduledAt.Should().NotBeNull();
    }

    [Test]
    public void SetPod_SetsPodUrlAndSignature()
    {
        var shipment = CreateTestShipment();

        shipment.SetPod("https://blob/pod.jpg", "base64signature");

        shipment.PodImageUrl.Should().Be("https://blob/pod.jpg");
        shipment.PodSignatureData.Should().Be("base64signature");
    }

    [Test]
    public void SetBolUrl_SetsBolDocumentUrl()
    {
        var shipment = CreateTestShipment();

        shipment.SetBolUrl("https://blob/bol.pdf");

        shipment.BolDocumentUrl.Should().Be("https://blob/bol.pdf");
    }

    [Test]
    public void SetEstimatedDeliveryDate_SetsDate()
    {
        var shipment = CreateTestShipment();
        var eta = DateTime.UtcNow.AddDays(3);

        shipment.SetEstimatedDeliveryDate(eta);

        shipment.EstimatedDeliveryDate.Should().Be(eta);
    }
}
