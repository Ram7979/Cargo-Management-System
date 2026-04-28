using CMS.BillingService.Domain.Entities;
using CMS.BillingService.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace CMS.Tests.Unit.Domain;

[TestFixture]
[Category("UC-BIL-001")]
public class InvoiceDomainTests
{
    private static Invoice CreateTestInvoice(decimal baseFreight = 1000m, decimal taxRate = 0.18m)
    {
        return Invoice.Create(
            "INV-2025-000001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            baseFreight,
            80m,
            10m,
            0m,
            taxRate,
            DateTime.UtcNow.AddDays(30),
            "Test invoice");
    }

    [Test]
    public void Create_WithValidData_StatusIsDraft()
    {
        var invoice = CreateTestInvoice();

        invoice.Status.Should().Be(InvoiceStatus.Draft);
    }

    [Test]
    public void Create_CalculatesTotalCorrectly()
    {
        var invoice = Invoice.Create(
            "INV-2025-000001", Guid.NewGuid(), Guid.NewGuid(),
            1000m, 80m, 10m, 0m, 0.18m,
            DateTime.UtcNow.AddDays(30));

        var subtotal = 1000m + 80m + 10m;
        var tax = Math.Round(subtotal * 0.18m, 2);
        var expected = subtotal + tax;

        invoice.TotalAmount.Should().Be(expected);
        invoice.OutstandingBalance.Should().Be(expected);
    }

    [Test]
    public void MarkIssued_ChangesStatusToIssued()
    {
        var invoice = CreateTestInvoice();

        invoice.MarkIssued();

        invoice.Status.Should().Be(InvoiceStatus.Issued);
    }

    [Test]
    [Category("UC-BIL-002")]
    public void ApplyPayment_FullAmount_StatusBecomePaid()
    {
        var invoice = CreateTestInvoice();
        invoice.MarkIssued();

        invoice.ApplyPayment(invoice.TotalAmount);

        invoice.Status.Should().Be(InvoiceStatus.Paid);
        invoice.OutstandingBalance.Should().BeLessOrEqualTo(0);
    }

    [Test]
    [Category("UC-BIL-002")]
    public void ApplyPayment_PartialAmount_StatusBecomesPartiallyPaid()
    {
        var invoice = CreateTestInvoice();
        invoice.MarkIssued();

        invoice.ApplyPayment(invoice.TotalAmount / 2);

        invoice.Status.Should().Be(InvoiceStatus.PartiallyPaid);
        invoice.OutstandingBalance.Should().BeGreaterThan(0);
    }

    [Test]
    public void Void_IssuedInvoice_ChangesStatusToVoid()
    {
        var invoice = CreateTestInvoice();
        invoice.MarkIssued();

        invoice.Void("Duplicate invoice");

        invoice.Status.Should().Be(InvoiceStatus.Void);
        invoice.VoidReason.Should().Be("Duplicate invoice");
    }

    [Test]
    public void Void_PaidInvoice_ThrowsInvalidOperationException()
    {
        var invoice = CreateTestInvoice();
        invoice.MarkIssued();
        invoice.ApplyPayment(invoice.TotalAmount);

        var act = () => invoice.Void("Trying to void paid");

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void CanAcceptPayment_IssuedInvoice_ReturnsTrue()
    {
        var invoice = CreateTestInvoice();
        invoice.MarkIssued();

        invoice.CanAcceptPayment().Should().BeTrue();
    }

    [Test]
    public void CanAcceptPayment_DraftInvoice_ReturnsFalse()
    {
        var invoice = CreateTestInvoice();

        invoice.CanAcceptPayment().Should().BeFalse();
    }

    [Test]
    public void CanAcceptPayment_VoidInvoice_ReturnsFalse()
    {
        var invoice = CreateTestInvoice();
        invoice.MarkIssued();
        invoice.Void("reason");

        invoice.CanAcceptPayment().Should().BeFalse();
    }

    [Test]
    public void IsOverdue_PastDueDateUnpaid_ReturnsTrue()
    {
        var invoice = Invoice.Create(
            "INV-2025-000002", Guid.NewGuid(), Guid.NewGuid(),
            1000m, 80m, 10m, 0m, 0.18m,
            DateTime.UtcNow.AddDays(-1));
        invoice.MarkIssued();

        invoice.IsOverdue().Should().BeTrue();
    }

    [Test]
    public void IsOverdue_FutureDueDate_ReturnsFalse()
    {
        var invoice = Invoice.Create(
            "INV-2025-000003", Guid.NewGuid(), Guid.NewGuid(),
            1000m, 80m, 10m, 0m, 0.18m,
            DateTime.UtcNow.AddDays(30));
        invoice.MarkIssued();

        invoice.IsOverdue().Should().BeFalse();
    }

    [Test]
    public void IsFullyPaid_AfterFullPayment_ReturnsTrue()
    {
        var invoice = CreateTestInvoice();
        invoice.MarkIssued();
        invoice.ApplyPayment(invoice.TotalAmount);

        invoice.IsFullyPaid().Should().BeTrue();
    }

    [Test]
    public void SetPdfUrl_SetsBlobUrl()
    {
        var invoice = CreateTestInvoice();

        invoice.SetPdfUrl("https://blob/invoices/INV-2025-000001.pdf");

        invoice.PdfBlobUrl.Should().Be("https://blob/invoices/INV-2025-000001.pdf");
    }
}
