using Current.Core.SharedKernel;

namespace Current.Core.Domain.Orders.Events;

/// <summary>
/// Evento disparado quando um pedido é criado
/// </summary>
public class OrderCreatedDomainEvent : DomainEvent
{
    public int OrderId { get; }
    public string OrderNumber { get; }
    public int CustomerId { get; }

    public OrderCreatedDomainEvent(int orderId, string orderNumber, int customerId)
    {
        OrderId = orderId;
      OrderNumber = orderNumber;
        CustomerId = customerId;
  }
}

/// <summary>
/// Evento disparado quando um pedido é processado
/// </summary>
public class OrderProcessedDomainEvent : DomainEvent
{
   public int OrderId { get; }
    public int CustomerId { get; }

    public OrderProcessedDomainEvent(int orderId, int customerId)
    {
   OrderId = orderId;
        CustomerId = customerId;
    }
}

/// <summary>
/// Evento disparado quando um pedido é enviado
/// </summary>
public class OrderShippedDomainEvent : DomainEvent
{
  public int OrderId { get; }
  public string OrderNumber { get; }

    public OrderShippedDomainEvent(int orderId, string orderNumber)
    {
    OrderId = orderId;
    OrderNumber = orderNumber;
 }
}

/// <summary>
/// Evento disparado quando um pedido é entregue
/// </summary>
public class OrderDeliveredDomainEvent : DomainEvent
{
    public int OrderId { get; }
    public string OrderNumber { get; }

    public OrderDeliveredDomainEvent(int orderId, string orderNumber)
  {
    OrderId = orderId;
     OrderNumber = orderNumber;
  }
}

/// <summary>
/// Evento disparado quando um pedido é cancelado
/// </summary>
public class OrderCancelledDomainEvent : DomainEvent
{
   public int OrderId { get; }
    public string OrderNumber { get; }

    public OrderCancelledDomainEvent(int orderId, string orderNumber)
    {
  OrderId = orderId;
   OrderNumber = orderNumber;
    }
}
