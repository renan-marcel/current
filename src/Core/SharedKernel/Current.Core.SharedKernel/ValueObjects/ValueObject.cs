namespace Current.Core.SharedKernel.ValueObjects;

/// <summary>
/// Classe base para Value Objects
/// Value Objects são objetos que representam um valor e não possuem identidade própria
/// Devem ser imutáveis
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Retorna os componentes que definem a igualdade do Value Object
    /// </summary>
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var valueObject = (ValueObject)obj;
return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
    .Select(x => x?.GetHashCode() ?? 0)
    .Aggregate((x, y) => new { x, y }.GetHashCode())
    .GetHashCode();
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
         return true;

        if (left is null || right is null)
      return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
