namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public interface ISaleEventPublisher
{
    void PublishSaleCreated(Guid saleId, string saleNumber, DateTime date, Guid customerId, string customerName);
    void PublishSaleModified(Guid saleId, string saleNumber);
    void PublishSaleCancelled(Guid saleId, string saleNumber);
    void PublishItemCancelled(Guid saleId, Guid productId, string productName);
}