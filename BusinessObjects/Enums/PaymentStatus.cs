namespace BusinessObjects.Enums;

public enum PaymentStatus
{
    /// <summary>
    /// Payment was successfully completed
    /// </summary>
    Paid,

    /// <summary>
    /// Payment failed or was canceled
    /// </summary>
    Failed,

    /// <summary>
    /// Payment is still pending or waiting for confirmation
    /// </summary>
    Pending
}
public enum PaymentType
{
    Package,
    PrePaid,
    UsePackage,
    PaidAtStation
}