using CashFlow.Communication.Enums;

namespace CashFlow.Communication.Requests;

public class RequestExpenseJson
{
    public string Title { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public DateTime Date {  get; set; }
    public decimal Amount { get; set; }
    public PaymentType paymentType { get; set; }
}
