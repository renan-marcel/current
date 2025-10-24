namespace Current.Core.SharedKernel.Entities;

/// <summary>
/// Classe base para raízes agregadas (Aggregate Roots)
/// Agregados são grupos de entidades que trabalham juntas como uma unidade
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot() { }
    protected AggregateRoot(int id) : base(id) { }

    /// <summary>
    /// Levanta um evento de domínio que ocorreu
    /// </summary>
    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
      _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Limpa os eventos de domínio após serem publicados
/// </summary>
    public void ClearDomainEvents()
    {
    _domainEvents.Clear();
    }
}
