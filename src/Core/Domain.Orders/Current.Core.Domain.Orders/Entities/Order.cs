using Current.Core.SharedKernel;
using Current.Core.SharedKernel.Entities;
using Current.Core.Domain.Orders.ValueObjects;
using Current.Core.Domain.Orders.Events;

namespace Current.Core.Domain.Orders.Entities;

/// <summary>
/// Agregado raiz para pedidos
/// Representa um pedido no domínio de negócio
/// </summary>
public class Order : AggregateRoot
{
    public string OrderNumber { get; private set; }
    public int CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
public DateTime OrderDate { get; private set; }
    public decimal TotalAmount { get; private set; }

    private Order() { }

    /// <summary>
    /// Factory method para criar um novo pedido
    /// </summary>
    public static Order Create(string orderNumber, int customerId)
    {
     if (string.IsNullOrWhiteSpace(orderNumber))
       throw new ArgumentException("Número do pedido é obrigatório", nameof(orderNumber));

        if (customerId <= 0)
          throw new ArgumentException("ID do cliente inválido", nameof(customerId));

        var order = new Order
        {
  OrderNumber = orderNumber,
            CustomerId = customerId,
     Status = OrderStatus.Pending,
  OrderDate = DateTime.UtcNow,
       TotalAmount = 0
        };

        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id, orderNumber, customerId));

  return order;
    }

    /// <summary>
    /// Processa o pedido
    /// </summary>
    public void Process()
    {
      if (Status != OrderStatus.Pending)
      throw new InvalidOperationException("Apenas pedidos pendentes podem ser processados");

        Status = OrderStatus.Processing;
 RaiseDomainEvent(new OrderProcessedDomainEvent(Id, CustomerId));
  }

    /// <summary>
  /// Marca o pedido como enviado
 /// </summary>
    public void Ship()
    {
    if (Status != OrderStatus.Processing)
   throw new InvalidOperationException("Apenas pedidos em processamento podem ser enviados");

        Status = OrderStatus.Shipped;
        RaiseDomainEvent(new OrderShippedDomainEvent(Id, OrderNumber));
   }

    /// <summary>
/// Marca o pedido como entregue
    /// </summary>
    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
       throw new InvalidOperationException("Apenas pedidos enviados podem ser entregues");

        Status = OrderStatus.Delivered;
     RaiseDomainEvent(new OrderDeliveredDomainEvent(Id, OrderNumber));
    }

    /// <summary>
    /// Cancela o pedido
    /// </summary>
    public void Cancel()
    {
      if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
          throw new InvalidOperationException("Pedido não pode ser cancelado neste estado");

        Status = OrderStatus.Cancelled;
    RaiseDomainEvent(new OrderCancelledDomainEvent(Id, OrderNumber));
    }
}
