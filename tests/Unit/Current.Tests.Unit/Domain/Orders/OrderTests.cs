using Xunit;
using Current.Core.Domain.Orders.Entities;
using Current.Core.Domain.Orders.ValueObjects;
using Current.Core.Domain.Orders.Events;

namespace Current.Tests.Unit.Domain.Orders;

public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrder()
    {
       // Arrange
     var orderNumber = "ORD-001";
        var customerId = 1;

  // Act
    var order = Order.Create(orderNumber, customerId);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(orderNumber, order.OrderNumber);
    Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.Pending.Value, order.Status.Value);
        Assert.NotEmpty(order.DomainEvents);
        Assert.IsType<OrderCreatedDomainEvent>(order.DomainEvents.First());
    }

    [Fact]
    public void Create_WithInvalidOrderNumber_ShouldThrow()
    {
       // Arrange
      var orderNumber = "";
  var customerId = 1;

     // Act & Assert
     Assert.Throws<ArgumentException>(() => Order.Create(orderNumber, customerId));
    }

    [Fact]
    public void Create_WithInvalidCustomerId_ShouldThrow()
    {
       // Arrange
      var orderNumber = "ORD-001";
        var customerId = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Order.Create(orderNumber, customerId));
    }

    [Fact]
    public void Process_WhenPending_ShouldChangeStatusToProcessing()
    {
   // Arrange
        var order = Order.Create("ORD-001", 1);
        order.ClearDomainEvents();

 // Act
        order.Process();

        // Assert
        Assert.Equal(OrderStatus.Processing.Value, order.Status.Value);
    Assert.NotEmpty(order.DomainEvents);
        Assert.IsType<OrderProcessedDomainEvent>(order.DomainEvents.First());
    }

    [Fact]
    public void Process_WhenNotPending_ShouldThrow()
    {
     // Arrange
  var order = Order.Create("ORD-001", 1);
     order.Process();

     // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Process());
    }

    [Fact]
    public void Ship_WhenProcessing_ShouldChangeStatusToShipped()
    {
   // Arrange
    var order = Order.Create("ORD-001", 1);
        order.Process();
       order.ClearDomainEvents();

        // Act
        order.Ship();

        // Assert
        Assert.Equal(OrderStatus.Shipped.Value, order.Status.Value);
      Assert.NotEmpty(order.DomainEvents);
   Assert.IsType<OrderShippedDomainEvent>(order.DomainEvents.First());
    }

    [Fact]
    public void Deliver_WhenShipped_ShouldChangeStatusToDelivered()
    {
        // Arrange
        var order = Order.Create("ORD-001", 1);
  order.Process();
        order.Ship();
      order.ClearDomainEvents();

    // Act
    order.Deliver();

 // Assert
        Assert.Equal(OrderStatus.Delivered.Value, order.Status.Value);
 Assert.NotEmpty(order.DomainEvents);
        Assert.IsType<OrderDeliveredDomainEvent>(order.DomainEvents.First());
    }

    [Fact]
    public void Cancel_WhenPending_ShouldChangeStatusToCancelled()
  {
        // Arrange
 var order = Order.Create("ORD-001", 1);
       order.ClearDomainEvents();

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled.Value, order.Status.Value);
        Assert.NotEmpty(order.DomainEvents);
       Assert.IsType<OrderCancelledDomainEvent>(order.DomainEvents.First());
    }

    [Fact]
    public void Cancel_WhenDelivered_ShouldThrow()
    {
        // Arrange
        var order = Order.Create("ORD-001", 1);
 order.Process();
        order.Ship();
       order.Deliver();

    // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }
}
