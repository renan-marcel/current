namespace Current.Core.SharedKernel.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio
/// </summary>
public abstract class Entity
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected Entity() { }

    protected Entity(int id)
    {
     Id = id;
   CreatedAt = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
  {
        if (obj is not Entity other)
   return false;

     return Id == other.Id && GetType() == other.GetType();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, GetType());
    }

    public override string ToString()
    {
      return $"{GetType().Name} [Id={Id}]";
  }
}
