namespace Services.ApiModels;

public class AccountInfomation
{
    public string AccountId { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
    public string Phone { get; set; }
    public string role { get; set; }
    public string Address { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}