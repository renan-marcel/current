# Padrões Domain-Driven Design (DDD)

## ?? Visão Geral

Este documento detalha os padrões e práticas de **Domain-Driven Design** utilizados no projeto Current. Ele complementa as diretrizes gerais em `copilot-guidelines.md`.

---

## ??? Arquitetura DDD - Visão Geral

```
???????????????????????????????????????????????????????
?    API / Presentation ?
?              (Controllers, Middleware)       ?
???????????????????????????????????????????????????????
           ? HTTP/REST
???????????????????????????????????????????????????????
?  Application Layer       ?
?  (Commands, Queries, Handlers, DTOs, Services)     ?
???????????????????????????????????????????????????????
   ? Domain Events, Interfaces
???????????????????????????????????????????????????????
?     Domain Layer     ?
?  (Entities, Value Objects, Aggregates, Events)     ?
???????????????????????????????????????????????????????
          ? Repository Interfaces
???????????????????????????????????????????????????????
?      Infrastructure Layer  ?
?  (Repositories, DbContext, External Services)      ?
???????????????????????????????????????????????????????
  ???????????????????????????????????????????????
        SharedKernel (Abstrações Reutilizáveis)
```

---

## ?? Camadas Detalhadas

### 1. SharedKernel

Contém código compartilhado entre domínios.

#### Estrutura
```
SharedKernel/
??? Entities/
? ??? Entity.cs           # Classe base para entidades
?   ??? AggregateRoot.cs    # Classe base para raízes agregadas
?   ??? ValueObject.cs      # Classe base para value objects
??? ValueObjects/
?   ??? Money.cs
?   ??? Email.cs
?   ??? PhoneNumber.cs
??? Events/
?   ??? DomainEvent.cs      # Classe base para eventos
?   ??? DomainEventHandler.cs
??? Specifications/
?   ??? Specification.cs
??? Exceptions/
?   ??? DomainException.cs
??? Interfaces/
    ??? IUnitOfWork.cs
```

#### Exemplo: Classe Base Entity
```csharp
namespace Current.Core.SharedKernel.Entities
{
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
  }
}
```

#### Exemplo: Classe Base AggregateRoot
```csharp
namespace Current.Core.SharedKernel.Entities
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<DomainEvent> _domainEvents = new();
        
        public IReadOnlyCollection<DomainEvent> DomainEvents 
 => _domainEvents.AsReadOnly();
        
        protected AggregateRoot() { }
        protected AggregateRoot(int id) : base(id) { }
  
        protected void RaiseDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
     }
        
        public void ClearDomainEvents()
        {
         _domainEvents.Clear();
     }
    }
}
```

### 2. Domain Layer

Contém a lógica de negócio central.

#### Estrutura por Domínio (Exemplo: Orders)
```
Domain/
??? Orders/
    ??? Entities/
    ?   ??? Order.cs        # Raiz agregada
    ?   ??? OrderItem.cs       # Entidade interna
    ??? ValueObjects/
    ?   ??? OrderId.cs
 ?   ??? OrderStatus.cs
    ? ??? OrderNumber.cs
    ??? Aggregates/
    ?   ??? OrderAggregate.cs  # Se necessário
    ??? Repositories/
    ???? IOrderRepository.cs
    ??? Services/
    ? ??? OrderCalculationService.cs
    ??? Events/
    ?   ??? OrderCreatedDomainEvent.cs
    ?   ??? OrderItemAddedDomainEvent.cs
    ?   ??? OrderProcessedDomainEvent.cs
    ??? Specifications/
    ?   ??? OrdersByCustomerSpec.cs
    ?   ??? PendingOrdersSpec.cs
    ??? Exceptions/
        ??? OrderNotFoundException.cs
        ??? InvalidOrderStatusException.cs
```

#### Exemplo: Entity (Order) - Raiz Agregada
```csharp
namespace Current.Core.Domain.Orders.Entities
{
    public class Order : AggregateRoot
    {
        private readonly List<OrderItem> _items = new();
        
        public string OrderNumber { get; private set; } = string.Empty;
  public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
        public Money Total { get; private set; }
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        
        // Construtor privado - usar factory method
        private Order() { }
        
 /// <summary>
        /// Cria um novo pedido
      /// </summary>
        public static Order Create(string orderNumber, CustomerId customerId)
     {
        if (string.IsNullOrWhiteSpace(orderNumber))
                throw new ArgumentException("Número do pedido é obrigatório");
                
         var order = new Order
    {
                OrderNumber = orderNumber,
      CustomerId = customerId,
                Status = OrderStatus.Pending,
             Total = new Money(0)
      };
          
   order.RaiseDomainEvent(
         new OrderCreatedDomainEvent(order.Id, customerId, order.OrderNumber));
        
          return order;
        }
        
        /// <summary>
        /// Adiciona um item ao pedido
        /// Só é possível adicionar itens em pedidos pendentes
        /// </summary>
        public void AddItem(ProductId productId, Money price, int quantity)
        {
   if (Status != OrderStatus.Pending)
         throw new InvalidOperationException(
           "Não é possível adicionar itens a um pedido que não está pendente");
         
            if (quantity <= 0)
         throw new ArgumentException("Quantidade deve ser maior que zero");
       
   var item = OrderItem.Create(this.Id, productId, price, quantity);
            _items.Add(item);
            
            UpdateTotal();
            
         this.RaiseDomainEvent(
         new OrderItemAddedDomainEvent(this.Id, productId, quantity));
        }
        
        /// <summary>
        /// Processa o pedido para preparação
        /// </summary>
        public void Process()
        {
       if (Status != OrderStatus.Pending)
                throw new InvalidOrderStatusException(Status, OrderStatus.Pending);
   
    if (!_items.Any())
  throw new InvalidOperationException("Pedido sem itens não pode ser processado");
        
  Status = OrderStatus.Processing;
     
            this.RaiseDomainEvent(
     new OrderProcessedDomainEvent(this.Id, this.CustomerId, this.Total));
    }
    
        private void UpdateTotal()
 {
            var total = _items.Aggregate(
          new Money(0),
         (acc, item) => new Money(acc.Amount + item.GetSubtotal().Amount));
 
       Total = total;
        }
 }
}
```

#### Exemplo: Value Object (OrderId)
```csharp
namespace Current.Core.Domain.Orders.ValueObjects
{
    public class OrderId : ValueObject
    {
        public int Value { get; }
        
        public OrderId(int value)
        {
          if (value <= 0)
         throw new ArgumentException("OrderId deve ser maior que zero");
  
            Value = value;
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
 {
        yield return Value;
  }
  
        public override string ToString() => Value.ToString();
    }
}
```

#### Exemplo: Value Object (OrderStatus)
```csharp
namespace Current.Core.Domain.Orders.ValueObjects
{
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
    }
}
```

#### Exemplo: Repository Interface
```csharp
namespace Current.Core.Domain.Orders.Repositories
{
    public interface IOrderRepository
    {
      /// <summary>
    /// Obtém um pedido por ID
 /// </summary>
        Task<Order?> GetByIdAsync(OrderId id);
     
        /// <summary>
        /// Obtém todos os pedidos de um cliente
        /// </summary>
        Task<IReadOnlyCollection<Order>> GetByCustomerAsync(CustomerId customerId);
        
        /// <summary>
        /// Adiciona um novo pedido
        /// </summary>
        Task AddAsync(Order order);
        
     /// <summary>
        /// Atualiza um pedido existente
  /// </summary>
        Task UpdateAsync(Order order);
        
        /// <summary>
/// Remove um pedido
        /// </summary>
     Task DeleteAsync(Order order);
    }
}
```

#### Exemplo: Domain Event
```csharp
namespace Current.Core.Domain.Orders.Events
{
    public class OrderCreatedDomainEvent : DomainEvent
    {
        public int OrderId { get; }
        public int CustomerId { get; }
  public string OrderNumber { get; }
        
      public OrderCreatedDomainEvent(int orderId, CustomerId customerId, string orderNumber)
        {
            OrderId = orderId;
    CustomerId = customerId.Value;
  OrderNumber = orderNumber;
      OccurredOn = DateTime.UtcNow;
   }
    }
}
```

#### Exemplo: Domain Service
```csharp
namespace Current.Core.Domain.Orders.Services
{
    /// <summary>
    /// Serviço de domínio para cálculos relacionados a pedidos
    /// Sem estado, reutilizável
    /// </summary>
public class OrderCalculationService
    {
        public Money CalculateTotal(Order order)
        {
     var total = order.Items
           .Aggregate(
        new Money(0),
   (acc, item) => new Money(acc.Amount + item.GetSubtotal().Amount));

    return total;
        }
        
        public Money CalculateTax(Money amount, decimal taxRate = 0.15m)
  {
      if (taxRate < 0 || taxRate > 1)
   throw new ArgumentException("Taxa deve estar entre 0 e 1");
          
         return new Money(amount.Amount * taxRate);
   }
        
     public Money CalculateShipping(Order order)
        {
   if (order.Total.Amount > 100)
        return new Money(0); // Frete grátis
   
      return new Money(15);
   }
    }
}
```

### 3. Application Layer

Orquestra a lógica de negócio.

#### Estrutura por Domínio (Exemplo: Orders)
```
Application/
??? Orders/
    ??? Commands/
    ?   ??? CreateOrderCommand.cs
    ?   ??? CreateOrderCommandHandler.cs
    ?   ??? ProcessOrderCommand.cs
    ?   ??? ProcessOrderCommandHandler.cs
    ??? Queries/
    ?   ??? GetOrderByIdQuery.cs
    ?   ??? GetOrderByIdQueryHandler.cs
    ?   ??? GetCustomerOrdersQuery.cs
    ?   ??? GetCustomerOrdersQueryHandler.cs
    ??? DTOs/
 ?   ??? OrderDTO.cs
    ?   ??? OrderItemDTO.cs
    ??? Handlers/
  ?   ??? OrderCreatedDomainEventHandler.cs
    ??? Mappings/
    ?   ??? OrderMappingProfile.cs
    ??? Validations/
    ?   ??? CreateOrderCommandValidator.cs
    ?   ??? ProcessOrderCommandValidator.cs
    ??? Services/
        ??? OrderApplicationService.cs
```

#### Exemplo: Command
```csharp
namespace Current.Application.Orders.Commands
{
    public class CreateOrderCommand : ICommand
    {
        public int CustomerId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
    
    public class CreateOrderItemRequest
    {
   public int ProductId { get; set; }
     public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
```

#### Exemplo: Command Handler
```csharp
namespace Current.Application.Orders.Commands
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
    {
     private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
   private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        
     public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
  IEventPublisher eventPublisher,
         ILogger<CreateOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
        _customerRepository = customerRepository;
_eventPublisher = eventPublisher;
   _logger = logger;
        }
        
    public async Task Handle(CreateOrderCommand command)
     {
            _logger.LogInformation("Iniciando criação de pedido para cliente {CustomerId}", 
           command.CustomerId);

            // Validar que cliente existe
       var customer = await _customerRepository.GetByIdAsync(
    new CustomerId(command.CustomerId));
   
          if (customer == null)
     throw new CustomerNotFoundException(command.CustomerId);
    
    // Criar agregado de domínio
         var order = Order.Create(command.OrderNumber, customer.Id);
      
   // Adicionar itens
     foreach (var item in command.Items)
          {
           order.AddItem(
      new ProductId(item.ProductId),
        new Money(item.Price),
    item.Quantity);
            }
            
 // Persistir
            await _orderRepository.AddAsync(order);
            
            // Publicar eventos de domínio
      foreach (var domainEvent in order.DomainEvents)
 {
     await _eventPublisher.PublishAsync(domainEvent);
  }
            
          order.ClearDomainEvents();
       
            _logger.LogInformation("Pedido {OrderNumber} criado com sucesso (ID: {OrderId})", 
  order.OrderNumber, order.Id);
    }
    }
}
```

#### Exemplo: Query
```csharp
namespace Current.Application.Orders.Queries
{
    public class GetOrderByIdQuery : IQuery<OrderDTO>
    {
        public int OrderId { get; set; }
        
     public GetOrderByIdQuery(int orderId)
        {
   OrderId = orderId;
}
    }
}
```

#### Exemplo: Query Handler
```csharp
namespace Current.Application.Orders.Queries
{
  public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDTO>
    {
        private readonly IOrderRepository _orderRepository;
private readonly IMapper _mapper;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;
      
        public GetOrderByIdQueryHandler(
            IOrderRepository orderRepository,
  IMapper mapper,
          ILogger<GetOrderByIdQueryHandler> logger)
        {
  _orderRepository = orderRepository;
    _mapper = mapper;
          _logger = logger;
        }
        
  public async Task<OrderDTO> Handle(GetOrderByIdQuery query)
        {
            _logger.LogDebug("Consultando pedido {OrderId}", query.OrderId);
  
  var order = await _orderRepository.GetByIdAsync(new OrderId(query.OrderId));
      
            if (order == null)
        throw new OrderNotFoundException(query.OrderId);
            
       return _mapper.Map<OrderDTO>(order);
        }
    }
}
```

#### Exemplo: DTO
```csharp
namespace Current.Application.Orders.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
 public int CustomerId { get; set; }
public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new();
    }
    
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
```

#### Exemplo: Mapping Profile
```csharp
namespace Current.Application.Orders.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
    {
            CreateMap<Order, OrderDTO>()
      .ForMember(dest => dest.Status, 
      opt => opt.MapFrom(src => src.Status.Value))
         .ForMember(dest => dest.Total,
    opt => opt.MapFrom(src => src.Total.Amount));
       
        CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.Price,
      opt => opt.MapFrom(src => src.Price.Amount));
      }
    }
}
```

#### Exemplo: Domain Event Handler (em Application)
```csharp
namespace Current.Application.Orders.Handlers
{
    public class OrderCreatedDomainEventHandler 
    : IDomainEventHandler<OrderCreatedDomainEvent>
    {
    private readonly IEmailService _emailService;
        private readonly ILogger<OrderCreatedDomainEventHandler> _logger;
        
        public OrderCreatedDomainEventHandler(
            IEmailService emailService,
            ILogger<OrderCreatedDomainEventHandler> logger)
      {
    _emailService = emailService;
            _logger = logger;
        }
      
 public async Task Handle(OrderCreatedDomainEvent domainEvent)
        {
        _logger.LogInformation(
            "Pedido {OrderNumber} criado. Enviando confirmação para cliente {CustomerId}",
      domainEvent.OrderNumber, domainEvent.CustomerId);
  
await _emailService.SendOrderConfirmationAsync(
     domainEvent.OrderNumber, 
       domainEvent.CustomerId);
   }
    }
}
```

### 4. Infrastructure Layer

Implementações técnicas.

#### Estrutura
```
Infrastructure/
??? Persistence/
?   ??? Data/
?   ?   ??? ApplicationDbContext.cs
?   ?   ??? Migrations/
?   ??? Repositories/
?   ?   ??? OrderRepository.cs
?   ?   ??? BaseRepository.cs
?   ??? Configurations/
?       ??? OrderEntityConfiguration.cs
?       ??? OrderItemEntityConfiguration.cs
??? ExternalServices/
?   ??? EmailService.cs
?   ??? PaymentGateway.cs
??? Logging/
???? LoggerFactory.cs
??? Caching/
?   ??? CacheService.cs
??? EventBus/
    ??? DomainEventPublisher.cs
```

#### Exemplo: DbContext
```csharp
namespace Current.Infrastructure.Persistence.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
     public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
   
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
   : base(options)
        {
        }
        
      protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
    base.OnModelCreating(modelBuilder);
            
            // Aplicar configurações de entidades
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    }
}
```

#### Exemplo: Repository Implementation
```csharp
namespace Current.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
  {
        private readonly ApplicationDbContext _context;
     private readonly ILogger<OrderRepository> _logger;
        
        public OrderRepository(ApplicationDbContext context, ILogger<OrderRepository> logger)
  {
   _context = context;
            _logger = logger;
        }
      
        public async Task<Order?> GetByIdAsync(OrderId id)
   {
        _logger.LogDebug("Consultando pedido {OrderId} na base de dados", id.Value);
          
            var order = await _context.Orders
        .Include(o => o.Items)
        .FirstOrDefaultAsync(o => o.Id == id.Value);
            
       return order;
        }
        
        public async Task<IReadOnlyCollection<Order>> GetByCustomerAsync(
            CustomerId customerId)
        {
            _logger.LogDebug("Consultando pedidos do cliente {CustomerId}", customerId.Value);
    
            var orders = await _context.Orders
        .Where(o => o.CustomerId == customerId.Value)
  .Include(o => o.Items)
                .ToListAsync();
        
   return orders.AsReadOnly();
        }
    
    public async Task AddAsync(Order order)
        {
     _logger.LogInformation("Adicionando novo pedido {OrderNumber}", order.OrderNumber);
    
    _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(Order order)
        {
    _logger.LogInformation("Atualizando pedido {OrderNumber} (ID: {OrderId})",
            order.OrderNumber, order.Id);
            
_context.Orders.Update(order);
await _context.SaveChangesAsync();
        }
        
     public async Task DeleteAsync(Order order)
    {
   _logger.LogInformation("Removendo pedido {OrderNumber} (ID: {OrderId})",
        order.OrderNumber, order.Id);
            
            _context.Orders.Remove(order);
     await _context.SaveChangesAsync();
        }
    }
}
```

### 5. API Layer

Apresentação e entrada.

#### Estrutura
```
API/
??? Controllers/
?   ??? OrdersController.cs
?   ??? CustomersController.cs
??? Middleware/
?   ??? ExceptionHandlingMiddleware.cs
?   ??? LoggingMiddleware.cs
??? Filters/
?   ??? ValidateCommandFilter.cs
??? Extensions/
    ??? ServiceCollectionExtensions.cs
```

#### Exemplo: Controller
```csharp
namespace Current.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
 {
        private readonly ICommandHandler<CreateOrderCommand> _createOrderCommandHandler;
        private readonly IQueryHandler<GetOrderByIdQuery, OrderDTO> _getOrderQueryHandler;
        
        public OrdersController(
      ICommandHandler<CreateOrderCommand> createOrderCommandHandler,
            IQueryHandler<GetOrderByIdQuery, OrderDTO> getOrderQueryHandler)
      {
    _createOrderCommandHandler = createOrderCommandHandler;
            _getOrderQueryHandler = getOrderQueryHandler;
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDTO>> Create(CreateOrderCommand command)
        {
 await _createOrderCommandHandler.Handle(command);
     return CreatedAtAction(nameof(GetById), new { id = command.OrderNumber });
        }
        
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDTO>> GetById(int id)
  {
          var query = new GetOrderByIdQuery(id);
            var order = await _getOrderQueryHandler.Handle(query);
      return Ok(order);
   }
    }
}
```

---

## ?? Fluxo de Dados

### Exemplo Completo: Criar Pedido

```
1. HTTP POST /api/orders
   ??> CreateOrderCommand (JSON)

2. OrdersController.Create()
   ??> Validate command
   
3. CreateOrderCommandHandler.Handle(command)
   ??> Get customer from ICustomerRepository
   ??> Order.Create() [Domain Logic]
   ??> order.AddItem() [Domain Logic]
   ??> Save via IOrderRepository
   ??> Publish DomainEvents
   
4. OrderCreatedDomainEventHandler
   ??> Send confirmation email
   
5. Return 201 Created
   ??> OrderDTO (JSON)
```

---

## ? Checklist DDD

Ao criar novos domínios:

- [ ] Criação de Entidades
  - [ ] Classe herda de `Entity` ou `AggregateRoot`
  - [ ] ID é `ValueObject` específico
  - [ ] Métodos privados/factory methods controlam criação
  - [ ] Invariantes de negócio são respeitados

- [ ] Criação de Value Objects
  - [ ] Classe herda de `ValueObject`
  - [ ] Imutável (sem setters)
  - [ ] Validação no construtor
  - [ ] Implementa `GetEqualityComponents()`

- [ ] Repository
  - [ ] Interface definida no Domain
  - [ ] Implementação no Infrastructure
  - [ ] Métodos refletem linguagem ubíqua
  - [ ] Retorna Aggregate Roots, não Entities

- [ ] Events
  - [ ] Classe herda de `DomainEvent`
  - [ ] Imutável
  - [ ] Contém informações relevantes (não referências completas)
  - [ ] Handler implementa `IDomainEventHandler<T>`

- [ ] Commands & Queries
  - [ ] Command para operações de escrita
  - [ ] Query para operações de leitura
  - [ ] Handler com lógica de orquestração
  - [ ] DTOs usados para retorno

- [ ] Validações
  - [ ] Validação no domínio (Value Objects)
- [ ] Validação na aplicação (Validators)
  - [ ] Mensagens em português/linguagem do negócio

---

## ?? Referências

- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Implementing Domain-Driven Design - Vaughn Vernon](https://vaughnvernon.com/)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Specification Pattern](https://en.wikipedia.org/wiki/Specification_pattern)

---

**Última atualização**: Janeiro 2025
