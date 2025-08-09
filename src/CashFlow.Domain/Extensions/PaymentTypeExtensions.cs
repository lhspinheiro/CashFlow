using CashFlow.Domain.Enums;
using CashFlow.Domain.Reports;

namespace CashFlow.Domain.Extensions;

public static class PaymentTypeExtensions
{
    public static string PaymentTypeToString(this PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Cash => ResourcerReportGenerationMessages.CASH,
            PaymentType.CreditCard => ResourcerReportGenerationMessages.CREDIT_CARD,
            PaymentType.DebitCard => ResourcerReportGenerationMessages.DEBIT_CARD,
            PaymentType.EletronicTransfer => ResourcerReportGenerationMessages.ELETRONIC_TRANSFER,
            _ => string.Empty
        };
    }
}
