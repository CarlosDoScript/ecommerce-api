using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public class LoggingSaleEventPublisher : ISaleEventPublisher
{
    private readonly ILogger<LoggingSaleEventPublisher> _logger;

    public LoggingSaleEventPublisher(ILogger<LoggingSaleEventPublisher> logger)
    {
        _logger = logger;
    }

    public void PublishSaleCreated(Guid saleId, string saleNumber, DateTime date, Guid customerId, string customerName) =>
        _logger.LogInformation(
            "[Event] SaleCreated: SaleId={SaleId}, SaleNumber={SaleNumber}, Date={Date}, CustomerId={CustomerId}, Customer={CustomerName}",
            saleId, saleNumber, date, customerId, customerName);

    public void PublishSaleModified(Guid saleId, string saleNumber) =>
        _logger.LogInformation(
            "[Event] SaleModified: SaleId={SaleId}, SaleNumber={SaleNumber}",
            saleId, saleNumber);

    public void PublishSaleCancelled(Guid saleId, string saleNumber) =>
        _logger.LogInformation(
            "[Event] SaleCancelled: SaleId={SaleId}, SaleNumber={SaleNumber}",
            saleId, saleNumber);

    public void PublishItemCancelled(Guid saleId, Guid productId, string productName) =>
        _logger.LogInformation(
            "[Event] ItemCancelled: SaleId={SaleId}, ProductId={ProductId}, ProductName={ProductName}",
            saleId, productId, productName);
}
