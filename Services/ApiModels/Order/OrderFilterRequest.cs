namespace Services.ApiModels.Order;
public class OrderFilterRequest
{
    public string OrderId { get; set; }
    public string Status { get; set; }
    public string AccountId { get; set; }
    public string ServiceType { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}