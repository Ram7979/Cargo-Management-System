namespace CMS.ShipmentService.Domain.Enums;

public enum FailureReason
{
    RecipientAbsent,
    WrongAddress,
    RefusedDelivery,
    DamagedPackage,
    Other
}
