using Newtonsoft.Json;

namespace BusinessObjects.Dtos
{
    /// <summary>
    /// Request DTO when creating a new payment link
    /// </summary>
    public class PayOSPaymentRequestDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response DTO after creating a payment link
    /// </summary>
    public class PayOSPaymentResponseDto
    {
        public string PaymentUrl { get; set; } = string.Empty;

        public string OrderId { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }

    public class PayOSWebhookData
    {
        [JsonProperty("orderCode")]
        public long OrderCode { get; set; }

        [JsonProperty("amount")]
        public int? Amount { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("accountNumber")]
        public string? AccountNumber { get; set; }

        [JsonProperty("reference")]
        public string? Reference { get; set; }

        [JsonProperty("transactionDateTime")]
        public string? TransactionDateTime { get; set; }

        [JsonProperty("currency")]
        public string? Currency { get; set; }

        [JsonProperty("paymentLinkId")]
        public string? PaymentLinkId { get; set; }

        [JsonProperty("code")]
        public string? Code { get; set; }

        [JsonProperty("desc")]
        public string? Desc { get; set; }

        [JsonProperty("counterAccountBankId")]
        public string? CounterAccountBankId { get; set; }

        [JsonProperty("counterAccountBankName")]
        public string? CounterAccountBankName { get; set; }

        [JsonProperty("counterAccountName")]
        public string? CounterAccountName { get; set; }

        [JsonProperty("counterAccountNumber")]
        public string? CounterAccountNumber { get; set; }

        [JsonProperty("virtualAccountName")]
        public string? VirtualAccountName { get; set; }

        [JsonProperty("virtualAccountNumber")]
        public string? VirtualAccountNumber { get; set; }
    }

    public class PayOSWebhookRequestDto
    {
        [JsonProperty("code")]
        public string? Code { get; set; }

        [JsonProperty("desc")]
        public string? Desc { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public PayOSWebhookData? Data { get; set; }

        [JsonProperty("signature")]
        public string? Signature { get; set; }
    }

    /// <summary>
    /// Response DTO when handling webhook
    /// </summary>
    public class PayOSWebhookResponseDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
