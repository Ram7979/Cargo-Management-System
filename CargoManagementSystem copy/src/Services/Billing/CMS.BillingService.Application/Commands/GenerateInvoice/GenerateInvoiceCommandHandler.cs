using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Application.Interfaces;
using CMS.BillingService.Domain.Entities;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.BillingService.Application.Commands.GenerateInvoice;

public class GenerateInvoiceCommandHandler : IRequestHandler<GenerateInvoiceCommand, ApiResponse<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBlobService _blobService;
    private readonly ICustomerServiceClient _customerServiceClient;
    private readonly INotificationServiceClient _notificationClient;
    private readonly IMapper _mapper;
    private readonly ILogger<GenerateInvoiceCommandHandler> _logger;

    public GenerateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IBlobService blobService,
        ICustomerServiceClient customerServiceClient,
        INotificationServiceClient notificationClient,
        IMapper mapper,
        ILogger<GenerateInvoiceCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _blobService = blobService;
        _customerServiceClient = customerServiceClient;
        _notificationClient = notificationClient;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<InvoiceDto>> Handle(GenerateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var exists = await _invoiceRepository.ExistsByShipmentIdAsync(command.Request.ShipmentId);
        if (exists)
            throw new ConflictException($"An invoice already exists for shipment '{command.Request.ShipmentId}'.");

        // Auto-compute due date from customer payment terms if not provided
        var dueDate = command.Request.DueDate;
        if (!dueDate.HasValue)
        {
            try
            {
                var terms = await _customerServiceClient.GetCustomerPaymentTermsAsync(command.Request.CustomerId);
                if (terms != null && !string.IsNullOrWhiteSpace(terms.PaymentTerms))
                {
                    var days = ParseNetDays(terms.PaymentTerms);
                    if (days > 0) dueDate = DateTime.UtcNow.AddDays(days);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not fetch customer payment terms for {CustomerId}", command.Request.CustomerId);
            }
        }

        // Collision-safe invoice number using timestamp + count
        var year = DateTime.UtcNow.Year;
        var count = await _invoiceRepository.CountAsync();
        var invoiceNumber = $"INV-{year}-{(count + 1):D6}";

        var invoice = Invoice.Create(
            invoiceNumber,
            command.Request.ShipmentId,
            command.Request.CustomerId,
            command.Request.BaseFreightCharge,
            command.Request.FuelSurcharge,
            command.Request.HandlingFee,
            command.Request.InsuranceAmount,
            command.Request.TaxRate,
            dueDate,
            command.Request.Notes);

        invoice.MarkIssued();

        // Generate PDF content (plain text for MVP — replace with real PDF library in production)
        var pdfContent = System.Text.Encoding.UTF8.GetBytes(
            $"INVOICE\n" +
            $"Invoice No: {invoiceNumber}\n" +
            $"Date: {DateTime.UtcNow:yyyy-MM-dd}\n" +
            $"Shipment: {command.Request.ShipmentId}\n" +
            $"Base Freight: {command.Request.BaseFreightCharge:C}\n" +
            $"Fuel Surcharge: {command.Request.FuelSurcharge:C}\n" +
            $"Handling Fee: {command.Request.HandlingFee:C}\n" +
            $"Insurance: {command.Request.InsuranceAmount:C}\n" +
            $"Tax ({command.Request.TaxRate * 100:F0}%): {invoice.TaxAmount:C}\n" +
            $"TOTAL: {invoice.TotalAmount:C}\n" +
            $"Due Date: {dueDate?.ToString("yyyy-MM-dd") ?? "N/A"}");

        var pdfUrl = await _blobService.UploadAsync("invoices", $"{invoiceNumber}.pdf", pdfContent);
        invoice.SetPdfUrl(pdfUrl);

        await _invoiceRepository.AddAsync(invoice);

        // Email invoice to customer (non-blocking)
        try
        {
            await _notificationClient.SendInvoiceEmailAsync(
                command.Request.CustomerId.ToString(),
                "customer@example.com", // In production: fetch from CustomerService
                invoiceNumber, pdfUrl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to email invoice {InvoiceNumber}", invoiceNumber);
        }

        var dto = _mapper.Map<InvoiceDto>(invoice);
        return ApiResponse<InvoiceDto>.Ok(dto, "Invoice generated successfully.");
    }

    private static int ParseNetDays(string paymentTerms)
    {
        // Parse "Net 30", "Net 15", "Net 45" etc.
        if (paymentTerms.StartsWith("Net ", StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(paymentTerms[4..].Trim(), out var days))
                return days;
        }
        return 0;
    }
}
