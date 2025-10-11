namespace Services.ApiModels.Order;

public class OrderResponse
{
    public string OrderId { get; set; }
    public string AccountId { get; set; }
    public decimal? Total { get; set; }
    public string BatteryId { get; set; }
    public string ServiceId { get; set; }
    public string Status { get; set; }
    public string ServiceType { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public AccountBasicResponse Account { get; set; }
    public BatteryBasicResponse Battery { get; set; }

    public OrderResponse(BusinessObjects.Models.Order order)
    {
        OrderId = order.OrderId;
        AccountId = order.AccountId;
        Total = order.Total;
        BatteryId = order.BatteryId;
        ServiceId = order.ServiceId;
        Status = order.Status;
        ServiceType = order.ServiceType;
        Date = order.Date;
        StartDate = order.StartDate;
        UpdateDate = order.UpdateDate;
        Account = order.Account != null ? new AccountBasicResponse(order.Account) : null;
        Battery = order.Battery != null ? new BatteryBasicResponse(order.Battery) : null;
    }
}

public class AccountBasicResponse
{
    public string AccountId { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }

    public AccountBasicResponse(BusinessObjects.Models.Account account)
    {
        AccountId = account.AccountId;
        Name = account.Name;
        Phone = account.Phone;
        Email = account.Email;
        Status = account.Status;
    }
}

public class BatteryBasicResponse
{
    public string BatteryId { get; set; }
    public string Status { get; set; }
    public int? Capacity { get; set; }
    public string BatteryType { get; set; }
    public string BatteryName { get; set; }

    public BatteryBasicResponse(BusinessObjects.Models.Battery battery)
    {
        BatteryId = battery.BatteryId;
        Status = battery.Status;
        Capacity = battery.Capacity;
        BatteryType = battery.BatteryType;
        BatteryName = battery.BatteryName;
    }
}