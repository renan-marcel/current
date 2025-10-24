namespace Current.Core.SharedKernel;

/// <summary>
/// Classe base para eventos de domínio
/// Eventos representam algo importante que ocorreu no domínio
/// </summary>
public abstract class DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(DomainEvent);

    public DomainEvent()
    {
     EventType = GetType().Name;
    }
}
