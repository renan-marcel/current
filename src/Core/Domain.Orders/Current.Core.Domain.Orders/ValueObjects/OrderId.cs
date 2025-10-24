using Current.Core.SharedKernel.ValueObjects;

namespace Current.Core.Domain.Orders.ValueObjects;

/// <summary>
/// Value Object que representa o ID de um pedido
/// </summary>
public class OrderId : ValueObject
{
    public int Value { get; }

    public OrderId(int value)
    {
        if (value <= 0)
       throw new ArgumentException("OrderId deve ser maior que zero", nameof(value));

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
 
    public static OrderId Create(int value) => new(value);
}
