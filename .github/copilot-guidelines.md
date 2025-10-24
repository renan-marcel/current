# Diretrizes de Desenvolvimento com Copilot

## ?? Visão Geral

Este documento define as diretrizes, padrões e melhores práticas que o GitHub Copilot deve seguir ao desenvolver e manter a aplicação **Current**.

Este é um arquivo de referência para auxiliar na geração de código consistente com os padrões do projeto.

---

## ?? Princípios Fundamentais

### 1. **Qualidade Acima de Tudo**
- Código limpo, legível e bem documentado
- Seguir princípios SOLID
- Manter cobertura de testes acima de 80%

### 2. **Consistência**
- Manter padrão de código em todo o projeto
- Aplicar convenções estabelecidas
- Documentar desvios justificados

### 3. **Segurança**
- Nunca incluir credenciais em código
- Validar entradas de usuário
- Seguir OWASP Top 10

### 4. **Performance**
- Otimizar para velocidade
- Minimizar consumo de memória
- Considerar escalabilidade

### 5. **Manutenibilidade**
- Escrever código autoexplicativo
- Adicionar comentários apenas quando necessário
- Facilitar futuras mudanças

---

## ??? Domain-Driven Design (DDD)

### Conceitos Fundamentais

Este projeto segue os princípios de **Domain-Driven Design**, garantindo que o domínio de negócio está no centro da arquitetura.

#### Camadas DDD

1. **SharedKernel** (Núcleo Compartilhado)
   - Código reutilizável entre domínios
   - Entidades base (`Entity`, `AggregateRoot`)
   - Value Objects compartilhados
   - Eventos de domínio base
   - Exceções de domínio base
   - Especificações reutilizáveis

2. **Domain** (Domínio)
   - Lógica de negócio central
   - Entidades específicas do domínio
   - Value Objects
   - Agregados (Aggregate Roots)
   - Interfaces de repositório
   - Serviços de domínio
   - Eventos de domínio específicos

3. **Application** (Aplicação)
   - Orquestração de processos
   - Comandos (Commands) - alteram estado
   - Queries (Queries) - leem dados
   - Handlers - processam Commands/Queries
   - DTOs (Data Transfer Objects)
   - Validações de aplicação
   - Mapeamentos (AutoMapper)
   - Serviços de aplicação

4. **Infrastructure** (Infraestrutura)
   - Implementações técnicas
   - Acesso a dados (Repositories)
   - Banco de dados (EF Core, Migrations)
   - Integrações externas
   - Logging
   - Caching
   - Event Bus

5. **API** (Apresentação)
   - Controllers/Endpoints
   - Middleware customizado
   - Filtros de ação
   - Configurações de startup

### Padrões DDD

#### Aggregate Root
```csharp
// ? Bom - Agregado com raiz bem definida
public class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = new();
    
  public string OrderNumber { get; private set; }
 public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
  public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
  
    // Factory method para criação
  public static Order Create(string orderNumber, CustomerId customerId)
    {
        var order = new Order
        {
            OrderNumber = orderNumber,
            CustomerId = customerId,
         Status = OrderStatus.Pending
        };
        
        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id, customerId));
 return order;
    }
    
    public void AddItem(ProductId productId, Money price, int quantity)
    {
        if (Status != OrderStatus.Pending)
      throw new InvalidOperationException("Não é possível adicionar itens a um pedido finalizado");
        
  var item = OrderItem.Create(productId, price, quantity);
        _items.Add(item);
        
 this.RaiseDomainEvent(new OrderItemAddedDomainEvent(this.Id, item.ProductId));
    }
}
```

#### Value Object
```csharp
// ? Bom - Value Object imutável
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }
    
    public Money(decimal amount, string currency = "BRL")
  {
        if (amount < 0)
    throw new ArgumentException("Valor não pode ser negativo");
        
     Amount = amount;
        Currency = currency;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
     yield return Currency;
    }
}
```

#### Repository Interface (no Domínio)
```csharp
// ? Bom - Interface no domínio, implementação na infraestrutura
public interface IOrderRepository
{
    Task<Order> GetByIdAsync(OrderId id);
    Task<IReadOnlyCollection<Order>> GetByCustomerAsync(CustomerId customerId);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Order order);
}
```

#### Domain Service
```csharp
// ? Bom - Serviço de domínio sem estado
public class OrderCalculationService
{
    public Money CalculateTotal(Order order)
    {
        var total = order.Items
            .Aggregate(new Money(0), (acc, item) => 
    new Money(acc.Amount + (item.Price.Amount * item.Quantity)));
        
        return total;
    }
    
    public Money CalculateTax(Money amount)
    {
 return new Money(amount.Amount * 0.15m); // 15%
    }
}
```

#### Command Handler (Aplicação)
```csharp
// ? Bom - Handler de comando orquestrando lógica de domínio
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventPublisher _eventPublisher;
    
    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
    IEventPublisher eventPublisher)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _eventPublisher = eventPublisher;
    }
    
    public async Task Handle(CreateOrderCommand command)
    {
        // Validar cliente existe
        var customer = await _customerRepository.GetByIdAsync(
            new CustomerId(command.CustomerId));
        
        if (customer == null)
       throw new CustomerNotFoundException(command.CustomerId);
        
        // Criar agregado de domínio
        var order = Order.Create(
      command.OrderNumber,
      customer.Id);
        
 // Persistir
        await _orderRepository.AddAsync(order);
        
    // Publicar eventos
  foreach (var domainEvent in order.DomainEvents)
  {
            await _eventPublisher.PublishAsync(domainEvent);
 }
    }
}
```

#### Query Handler (Aplicação)
```csharp
// ? Bom - Handler de query retornando DTO
public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDTO>
{
 private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    
    public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }
    
    public async Task<OrderDTO> Handle(GetOrderByIdQuery query)
    {
      var order = await _orderRepository.GetByIdAsync(new OrderId(query.OrderId));
        
  if (order == null)
            throw new OrderNotFoundException(query.OrderId);
        
        return _mapper.Map<OrderDTO>(order);
    }
}
```

#### Specification Pattern
```csharp
// ? Bom - Especificação reutilizável
public class OrdersByCustomerSpecification : Specification<Order>
{
    public OrdersByCustomerSpecification(CustomerId customerId)
    {
        Query.Where(o => o.CustomerId == customerId)
          .Include(o => o.Items);
    }
}
```

---

## ?? Padrões de Código

### Estrutura de Arquivos

```
Current/
??? src/
?   ??? Core/
?   ?   ??? SharedKernel/        # Código compartilhado entre domínios
?   ?   ??? Entities/    # Entidades base
?   ?   ??? ValueObjects/# Value Objects compartilhados
?   ?   ? ??? Events/     # Eventos de domínio
?   ?   ?   ??? Specifications/  # Especificações reutilizáveis
?   ?   ?   ??? Exceptions/      # Exceções de domínio
?   ?   ??? [DomainName]/        # Cada domínio separado
?   ?       ??? Entities/        # Entidades do domínio
?   ?       ??? ValueObjects/    # Value Objects do domínio
?   ?       ??? Aggregates/      # Raízes agregadas
?   ?       ??? Repositories/    # Interfaces de repositório
?   ?       ??? Services/        # Serviços de domínio
?   ?   ??? Events/    # Eventos do domínio
?   ?       ??? Specifications/  # Especificações do domínio
?   ??? Application/
?   ?   ??? [DomainName]/
?   ?   ?   ??? Commands/        # Comandos (escrita)
?   ?   ?   ??? Queries/      # Queries (leitura)
?   ?   ?   ??? Handlers/    # Command/Query handlers
?   ?   ?   ??? DTOs/  # Data Transfer Objects
?   ?   ?   ??? Mappings/        # AutoMapper profiles
?   ?   ?   ??? Services/      # Serviços de aplicação
?   ?   ??? Common/
?   ?   ?   ??? Behaviors/   # Pipeline behaviors
?   ?   ?   ??? Exceptions/      # Exceções de aplicação
?   ?   ?   ??? Interfaces/   # Interfaces comuns
?   ?   ??? Validations/ # Validadores compartilhados
???? Infrastructure/
?   ?   ??? Persistence/
?   ?   ?   ??? Data/         # DbContext e migrations
?   ?   ?   ??? Repositories/    # Implementações de repositório
?   ?   ?   ??? Configurations/  # EF Core configurations
?   ?   ??? ExternalServices/    # Integrações externas
?   ?   ??? Logging/           # Implementação de logging
?   ?   ??? Caching/     # Implementação de cache
?   ?   ??? EventBus/            # Publicação de eventos
?   ??? API/
?   ?   ??? Controllers/   # Controllers/Endpoints
?   ?   ??? Middleware/          # Custom middleware
?   ?   ??? Filters/     # Filtros de ação
?   ?   ??? Extensions/          # Extensões de startup
?   ??? Utils/    # Utilitários gerais
??? tests/
?   ??? UnitTests/
? ?   ??? [DomainName]/
?   ??? IntegrationTests/
?   ?   ??? [DomainName]/
?   ??? E2ETests/
??? docs/   # Documentação
??? .github/       # Configurações do GitHub
```

### Convenções de Nomenclatura

#### C# (Se aplicável)
- **Classes**: PascalCase (ex: `UserService`, `DatabaseContext`)
- **Métodos**: PascalCase (ex: `GetUser()`, `CreateOrder()`)
- **Propriedades**: PascalCase (ex: `UserId`, `CreatedAt`)
- **Variáveis Locais**: camelCase (ex: `userId`, `isValid`)
- **Constantes**: UPPER_SNAKE_CASE (ex: `MAX_RETRY_COUNT`)
- **Interfaces**: Prefixo "I" + PascalCase (ex: `IUserRepository`)
- **Abstratas**: Sufixo "Base" (ex: `RepositoryBase`)

#### JavaScript/TypeScript
- **Arquivos**: kebab-case (ex: `user-service.ts`)
- **Classes**: PascalCase (ex: `UserService`)
- **Funções**: camelCase (ex: `getUserById()`)
- **Constantes**: UPPER_SNAKE_CASE (ex: `API_TIMEOUT`)

### Isolamento de Camadas DDD

#### Dependências Permitidas

```
API ? Application ? Domain ? Infrastructure
         ?        ?
    SharedKernel  ?????????????????
```

#### O Que FAZER

? **Domain** pode depender de:
- SharedKernel
- Apenas abstrações (interfaces)

? **Application** pode depender de:
- Domain (abstrações)
- SharedKernel
- Não pode depender de Infrastructure diretamente

? **Infrastructure** pode depender de:
- Domain (implementação)
- Application
- SharedKernel
- Frameworks e bibliotecas externas

? **API** pode depender de:
- Application
- Infrastructure
- SharedKernel

#### O Que NÃO FAZER

? Domain depender de Application ou Infrastructure
? Application depender de Infrastructure (usar injeção de dependência)
? Infrastructure ter dependências cíclicas
? Misturar lógica de negócio entre camadas

### Exemplo de Estrutura Correta

```csharp
// ? ERRADO - Domain dependendo de Infrastructure
namespace Current.Core.Domain.Orders
{
    public class Order
    {
        public void Save(IDbContext dbContext) // ERRADO!
        {
        // ...
        }
    }
}

// ? CORRETO - Domain com repositório injetado
namespace Current.Core.Domain.Orders
{
    public interface IOrderRepository
    {
        Task SaveAsync(Order order);
    }
    
    public class Order
    {
        // Lógica de negócio pura
    }
}

// ? CORRETO - Infrastructure implementa interface
namespace Current.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
     private readonly ApplicationDbContext _context;
        
    public async Task SaveAsync(Order order)
 {
            _context.Orders.Add(order);
    await _context.SaveChangesAsync();
        }
    }
}

// ? CORRETO - Application orquestra
namespace Current.Application.Orders.Commands
{
    public class CreateOrderCommandHandler
    {
        private readonly IOrderRepository _orderRepository;
        
   public async Task Handle(CreateOrderCommand command)
        {
       var order = Order.Create(command.OrderNumber);
            await _orderRepository.SaveAsync(order);
        }
    }
}
```

### Padrões de Código

#### 1. **Métodos e Funções**
- Máximo de 20 linhas por função
- Um nível de abstração por função
- Parâmetros: máximo 3 (usar objetos para mais)
- Use Command/Query Separation (CQRS) quando apropriado

```csharp
// ? Evitar - Múltiplas responsabilidades
public async Task<OrderDTO> ProcessOrderWithMultipleSteps(
    int orderId, string customerEmail, bool sendNotification)
{
    var order = await GetOrder(orderId);
    if (sendNotification)
        await SendEmail(customerEmail);
    order.Status = "Processed";
    await SaveOrder(order);
    return MapToDTO(order);
}

// ? Preferir - Separação de responsabilidades
// Em Application/Orders/Commands
public class ProcessOrderCommandHandler : ICommandHandler<ProcessOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmailService _emailService;
  
    public async Task Handle(ProcessOrderCommand command)
    {
      var order = await _orderRepository.GetByIdAsync(command.OrderId);
    order.Process();
await _orderRepository.UpdateAsync(order);
    }
}

// Em Application/Orders/Queries
public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDTO>
{
    public async Task<OrderDTO> Handle(GetOrderByIdQuery query)
    {
    var order = await _orderRepository.GetByIdAsync(query.OrderId);
        return _mapper.Map<OrderDTO>(order);
    }
}
```

#### 2. **Tratamento de Erros**
- Usar exceções específicas do domínio
- Não engolir exceções silenciosamente
- Adicionar contexto ao lançar exceções
- Domain exceptions são de negócio, Application exceptions são técnicas

```csharp
// ? Bom - Exceções de domínio bem definidas
namespace Current.Core.SharedKernel.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
    }
}

namespace Current.Core.Domain.Orders.Exceptions
{
    public class OrderNotFoundException : DomainException
    {
        public OrderNotFoundException(OrderId orderId) 
    : base($"Pedido {orderId.Value} não encontrado") { }
    }
    
    public class InvalidOrderStatusException : DomainException
    {
        public InvalidOrderStatusException(OrderStatus current, OrderStatus expected)
            : base($"Status inválido. Esperado: {expected}, Atual: {current}") { }
    }
}

// Usar em handlers
public async Task Handle(ProcessOrderCommand command)
{
    var order = await _orderRepository.GetByIdAsync(command.OrderId)
  ?? throw new OrderNotFoundException(command.OrderId);
    
    if (order.Status != OrderStatus.Pending)
    throw new InvalidOrderStatusException(order.Status, OrderStatus.Pending);
    
    order.Process();
}
```

#### 3. **Comentários**
- Explicar o "por quê" do negócio, não apenas técnico
- Documentar decisões de design
- Evitar óbvios

```csharp
// ? Evitar
// Incrementa o contador
counter++;

// ? Preferir - Explica a intenção de negócio
// Incrementa tentativas de processamento apenas se validação passou
// para evitar contabilizar erros de validação de entrada
counter++;

// ? Documentar decisões de design
// Usamos Value Object para Money ao invés de decimal simples
// para garantir que quantias sempre incluam moeda e validações
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }
}
```

#### 4. **Logging**
- Use log levels apropriados: `Debug`, `Info`, `Warning`, `Error`
- Inclua contexto relevante (IDs de agregados)
- Nunca faça log de informações sensíveis
- Considere eventos de domínio para auditoria

```csharp
// ? Bom - Logging contextualizado
_logger.LogInformation("Pedido {OrderId} processado com sucesso", order.Id);
_logger.LogError(ex, "Falha ao processar pedido {OrderId} para cliente {CustomerId}", 
    command.OrderId, command.CustomerId);

// ? Considerar eventos para auditoria
order.RaiseDomainEvent(new OrderProcessedDomainEvent(
    order.Id, 
  order.CustomerId, 
    DateTime.UtcNow));

// ? Evitar
_logger.LogInformation("Pedido criado"); // Falta contexto
_logger.LogInformation($"Dados de pagamento: {paymentData}"); // Informação sensível
```

---

## ?? Testes

### Cobertura de Testes

- **Mínimo**: 80% de cobertura de código
- **Objetivo**: 90% de cobertura
- Priorizar: lógica crítica e casos de erro

### Estrutura de Testes

#### Convenção: AAA (Arrange, Act, Assert)

```csharp
[Fact]
public async Task CreateUser_WithValidData_ShouldReturnUserId()
{
    // Arrange
    var userData = new CreateUserCommand
    {
        Name = "João Silva",
   Email = "joao@example.com"
    };

    // Act
    var userId = await _userService.CreateUserAsync(userData);

    // Assert
    Assert.NotNull(userId);
    Assert.True(userId > 0);
}

[Fact]
public async Task CreateUser_WithInvalidEmail_ShouldThrowException()
{
    // Arrange
    var userData = new CreateUserCommand
    {
     Name = "João",
        Email = "invalid-email"
    };

    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(
        () => _userService.CreateUserAsync(userData)
    );
}
```

### Nomenclatura de Testes

Padrão: `[MethodName]_[Scenario]_[ExpectedResult]`

```csharp
// ? Bom
public async Task GetUser_WithValidId_ShouldReturnUser()
public async Task GetUser_WithInvalidId_ShouldThrowException()
public async Task GetUser_WhenUserNotExists_ShouldReturnNull()

// ? Evitar
public async Task TestGetUser()
public async Task GetUserTest1()
```

---

## ?? Commits e Versionamento

### Conventional Commits

Seguir rigorosamente o padrão [Conventional Commits](https://www.conventionalcommits.org/pt-br/):

```
<tipo>(<escopo>): <descrição>

[corpo opcional]

[rodapé(s) opcional(is)]
```

#### Tipos Válidos
- **feat**: Uma nova funcionalidade
- **fix**: Uma correção de bug
- **docs**: Alterações apenas na documentação
- **style**: Alterações que não afetam o significado do código (formatação, etc)
- **refactor**: Uma mudança de código que não corrige um bug nem adiciona uma funcionalidade
- **perf**: Uma mudança de código que melhora a performance
- **test**: Adição ou correção de testes
- **chore**: Mudanças em ferramentas, configurações ou dependências

#### Exemplos

```
feat(auth): adicionar autenticação de dois fatores

fix(user): corrigir validação de email duplicado

docs(readme): atualizar instruções de setup

perf(database): otimizar query de listagem de usuários

test(user-service): adicionar cobertura de testes para validação
```

### Gitflow Workflow

Sempre seguir o fluxo definido em [docs/Gitflow.md](../docs/Gitflow.md):

1. Criar feature a partir de `develop`
2. Dar commit com Conventional Commits
3. Criar Pull Request para `develop`
4. Após testes, fazer merge em `develop`
5. Releases vêm de `develop` ? `main`

---

## ?? Segurança

### Checklist de Segurança

- [ ] Nenhuma credencial em código (usar variáveis de ambiente)
- [ ] Validação de entrada em todas as APIs
- [ ] Proteção contra SQL Injection
- [ ] Proteção contra XSS (se houver frontend)
- [ ] Autenticação e autorização implementadas
- [ ] Dados sensíveis não logados
- [ ] Dependências atualizadas e sem vulnerabilidades
- [ ] Senhas hasheadas com algoritmos seguros
- [ ] Validação de domínio (Value Objects validam no construtor)
- [ ] Invariantes de agregado são respeitados
- [ ] Eventos de domínio não expõem dados sensíveis
- [ ] Repositórios não retornam Entidades de outros agregados
