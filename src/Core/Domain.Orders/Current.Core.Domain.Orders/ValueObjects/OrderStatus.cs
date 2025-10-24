using Current.Core.SharedKernel.ValueObjects;

namespace Current.Core.Domain.Orders.ValueObjects;

/// <summary>
/// Value Object que representa o status de um pedido
/// </summary>
public class OrderStatus : ValueObject
{
    public static readonly OrderStatus Pending = new("Pending");
    public static readonly OrderStatus Processing = new("Processing");
    public static readonly OrderStatus Shipped = new("Shipped");
    public static readonly OrderStatus Delivered = new("Delivered");
    public static readonly OrderStatus Cancelled = new("Cancelled");

    public string Value { get; }

    private OrderStatus(string value)
    {
    Value = value;
    }

    public static OrderStatus FromString(string value)
    {
        return value switch
        {
            "Pending" => Pending,
       "Processing" => Processing,
       "Shipped" => Shipped,
    "Delivered" => Delivered,
  "Cancelled" => Cancelled,
 _ => throw new ArgumentException($"Status inválido: {value}")
};
}

    protected override IEnumerable<object> GetEqualityComponents()
    {
    yield return Value;
    }

    public override string ToString() => Value;
}
