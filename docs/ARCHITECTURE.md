# ??? Arquitetura DDD - Guia de Referência Rápida

## ?? Estrutura de Projetos

```
Current/
??? src/
?   ??? Core/
?   ?   ??? SharedKernel/   # Código base reutilizável
?   ?   ?   ??? Entities/
?   ?   ?   ?   ??? Entity.cs       # Base para entidades
?   ?   ?   ?   ??? AggregateRoot.cs        # Base para agregados
?   ?   ?   ??? ValueObjects/
?   ?   ?   ?   ??? ValueObject.cs          # Base para value objects
?   ?   ?   ??? Exceptions/
?   ?   ?   ?   ??? DomainException.cs      # Exceções de domínio
?   ?   ?   ??? DomainEvent.cs  # Eventos de domínio
?   ?   ?
?   ?   ??? Domain.Orders/      # Domínio de Pedidos
?   ? ??? Entities/
?   ?       ?   ??? Order.cs       # Agregado raiz
?   ?     ??? ValueObjects/
?   ?       ?   ??? OrderId.cs        # ID do pedido
?   ?       ?   ??? OrderStatus.cs          # Status do pedido
?   ?       ??? Events/
?   ?     ?   ??? OrderDomainEvents.cs    # Eventos: Created, Processed, etc
?   ?       ??? Repositories/     # Interfaces (implementação em Infrastructure)
?   ?
?   ??? Application/  # Lógica de aplicação
?   ?   ??? Commands/       # Comandos (escrita)
?   ?   ??? Queries/         # Queries (leitura)
?   ?   ??? Handlers/    # Command/Query handlers
?   ?   ??? DTOs/    # Data Transfer Objects
?   ?   ??? Mappings/  # AutoMapper profiles
?   ?
?   ??? Infrastructure/           # Implementações técnicas
?   ?   ??? Persistence/
?   ?   ?   ??? Data/      # DbContext
? ??   ??? Repositories/     # Implementação de repositórios
?   ?   ?   ??? Configurations/  # EF Core configurations
?   ?   ??? ExternalServices/               # Integrações externas
?   ?
?   ??? API/             # ASP.NET Core Web API
?     ??? Controllers/    # Endpoints HTTP
?       ??? Middleware/     # Custom middleware
?       ??? Extensions/       # Setup de DI
?
??? tests/
    ??? Unit/            # Testes unitários
        ??? Domain/
         ??? Orders/
    ??? OrderTests.cs           # Testes do Agregado Order
```

## ?? Fluxo de Dependências

```
??????????????????????????????????
?    API Layer (Controllers)     ?  ? HTTP Requests
??????????????????????????????????
?  Application Layer (Commands)  ?  ? Orquestrção
??????????????????????????????????
?    Domain Layer (Agregados)    ?  ? Lógica de Negócio
??????????????????????????????????
?   Infrastructure (Repositories)?  ? Acesso a dados
??????????????????????????????????
? SharedKernel (Base)       ?  ? Abstrações reutilizáveis
??????????????????????????????????

Regra de Ouro:
  ? Camadas superiores dependem de inferiores
  ? Camadas inferiores NUNCA dependem de superiores
```

## ?? Referências Entre Projetos

```
Current.Tests.Unit
??? Referencia: Current.Core.Domain.Orders
??? Referencia: Current.Core.SharedKernel
??? Referencia: Current.Application

Current.API
??? Referencia: Current.Core.Domain.Orders
??? Referencia: Current.Core.SharedKernel
??? Referencia: Current.Application
??? Referencia: Current.Infrastructure

Current.Infrastructure
??? Referencia: Current.Core.Domain.Orders
??? Referencia: Current.Core.SharedKernel
??? Referencia: Current.Application

Current.Application
??? Referencia: Current.Core.Domain.Orders
??? Referencia: Current.Core.SharedKernel

Current.Core.Domain.Orders
??? Referencia: Current.Core.SharedKernel

Current.Core.SharedKernel
??? (Nenhuma dependência de outros projetos)
```

## ?? Exemplo: Agregado Order

### 1. Criar Agregado (Domain Layer)

```csharp
// Domain/Orders/Entities/Order.cs
public class Order : AggregateRoot
{
    public string OrderNumber { get; private set; }
    public OrderStatus Status { get; private set; }
    
    public static Order Create(string orderNumber, int customerId)
    {
    var order = new Order { OrderNumber = orderNumber };
        order.RaiseDomainEvent(new OrderCreatedDomainEvent(...));
        return order;
}
    
    public void Process()
    {
        if (Status != OrderStatus.Pending)
 throw new InvalidOperationException("...");
        
 Status = OrderStatus.Processing;
      RaiseDomainEvent(new OrderProcessedDomainEvent(...));
    }
}
```

### 2. Criar Value Objects (Domain Layer)

```csharp
// Domain/Orders/ValueObjects/OrderStatus.cs
public class OrderStatus : ValueObject
{
    public static readonly OrderStatus Pending = new("Pending");
    public static readonly OrderStatus Processing = new("Processing");
    
  protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

### 3. Criar Eventos (Domain Layer)

```csharp
// Domain/Orders/Events/OrderDomainEvents.cs
public class OrderCreatedDomainEvent : DomainEvent
{
    public int OrderId { get; }
    public string OrderNumber { get; }
}
```

### 4. Criar Repositório Interface (Domain Layer)

```csharp
// Domain/Orders/Repositories/IOrderRepository.cs
public interface IOrderRepository
{
    Task<Order> GetByIdAsync(OrderId id);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}
```

### 5. Implementar Repositório (Infrastructure Layer)

```csharp
// Infrastructure/Persistence/Repositories/OrderRepository.cs
public class OrderRepository : IOrderRepository
{
    public async Task<Order> GetByIdAsync(OrderId id)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id.Value);
    }
}
```

### 6. Criar Command (Application Layer)

```csharp
// Application/Orders/Commands/CreateOrderCommand.cs
public class CreateOrderCommand : ICommand
{
    public string OrderNumber { get; set; }
    public int CustomerId { get; set; }
}

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IOrderRepository _repository;
 
    public async Task Handle(CreateOrderCommand command)
    {
        var order = Order.Create(command.OrderNumber, command.CustomerId);
      await _repository.AddAsync(order);
  }
}
```

### 7. Testar (Tests Layer)

```csharp
// Tests/Unit/Domain/Orders/OrderTests.cs
public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrder()
    {
        var order = Order.Create("ORD-001", 1);
        
        Assert.NotNull(order);
   Assert.NotEmpty(order.DomainEvents);
    }
}
```

## ?? Testes Disponíveis

```bash
# Executar todos os testes
dotnet test

# Executar apenas testes de Domain
dotnet test --filter Category=Domain

# Com verbosidade
dotnet test --verbosity detailed

# Gerar relatório de cobertura
dotnet test /p:CollectCoverage=true
```

## ?? Padrões de Nomenclatura

### Entidades
- Nome: PascalCase + sufixo "Aggregate" ou nome do conceito
- Exemplo: `Order`, `Customer`, `Invoice`

### Value Objects
- Nome: PascalCase + sufixo descritivo
- Exemplo: `OrderId`, `Money`, `Email`

### Agregados Raiz
- Herdam de: `AggregateRoot`
- Responsabilidade: Encapsular agregado e manter invariantes

### Eventos de Domínio
- Nome: PascalCase + sufixo "DomainEvent"
- Exemplo: `OrderCreatedDomainEvent`, `OrderProcessedDomainEvent`

### Repositórios
- Interface: `IOrderRepository`, `ICustomerRepository`
- Implementação: `OrderRepository`, `CustomerRepository`

### Commands
- Nome: PascalCase + sufixo "Command"
- Exemplo: `CreateOrderCommand`, `ProcessOrderCommand`

### Queries
- Nome: PascalCase + sufixo "Query"
- Exemplo: `GetOrderByIdQuery`, `ListOrdersQuery`

## ? Checklist de Implementação

Ao criar um novo agregado:

- [ ] Criar classe que herda de `AggregateRoot`
- [ ] Criar Value Objects se necessário
- [ ] Implementar factory methods (não construtores públicos)
- [ ] Adicionar validações no construtor
- [ ] Criar Domain Events para transições importantes
- [ ] Criar interface de repositório
- [ ] Escrever testes unitários (AAA pattern)
- [ ] Garantir que o agregado respeita invariantes
- [ ] Não expor coleções mutáveis
- [ ] Usar private setters para propriedades

## ?? Próximos Passos

1. **Expandir Domínios**: Criar mais agregados (Customer, Product, Invoice)
2. **Implementar Persistência**: DbContext e Migrations
3. **Criar Endpoints**: Controllers que usam Commands/Queries
4. **Event Publishing**: Publicar Domain Events para subscribers
5. **Error Handling**: Middleware de tratamento de exceções

## ?? Referências

- [DDD Patterns](.github/DDD-PATTERNS.md)
- [Copilot Guidelines](.github/copilot-guidelines.md)
- [Checklist DDD](.github/DDD-CHECKLIST.md)

---

**Última atualização**: Outubro 2025
