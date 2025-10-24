# ?? Guia Rápido para Iniciantes

**Quer começar a desenvolver no projeto Current? Este é o lugar certo!**

---

## ?? Tempo de Leitura: 10 minutos

---

## ?? O Que Você Precisa Saber

### 1. O Projeto Usa Domain-Driven Design (DDD)

O projeto é organizado em **camadas bem definidas**:

```
?? API (Recebe requisições HTTP)
?? Application (Orquestra a lógica)
?? Domain (Regras de negócio)
?? Infrastructure (Acesso a dados)
```

**Cada camada tem responsabilidades específicas** e não deve depender de camadas inferiores.

### 2. Você Vai Lidar Com...

| Conceito | O Quê | Exemplo |
|----------|-------|---------|
| **Aggregate** | Grupo de objetos que trabalham juntos | Order (Pedido) |
| **Entity** | Objeto com identidade | OrderItem |
| **Value Object** | Objeto sem identidade | Money, Email |
| **Repository** | Persiste Aggregates | OrderRepository |
| **Command** | Ordem para alterar dados | CreateOrderCommand |
| **Query** | Requisição para ler dados | GetOrderByIdQuery |
| **Event** | Algo importante aconteceu | OrderCreatedDomainEvent |

### 3. Estrutura de Pastas

```
src/
??? Core/
?   ??? SharedKernel/     # Código reutilizável
?   ??? Orders/     # Seu novo domínio aqui
?       ??? Entities/
?       ??? ValueObjects/
?  ??? Repositories/
?       ??? Events/
??? Application/Orders/
?   ??? Commands/
?   ??? Queries/
?   ??? Handlers/
?   ??? DTOs/
?   ??? Mappings/
??? Infrastructure/
?   ??? Persistence/
?       ??? Repositories/  (implementação)
?       ??? Configurations/
??? API/
?   ??? Controllers/
??? tests/
    ??? UnitTests/
    ??? IntegrationTests/
    ??? E2ETests/
```

---

## ?? Seu Primeiro Domínio

### Passo 1: Criar a Estrutura
```bash
mkdir src/Core/MyDomain
mkdir src/Core/MyDomain/Entities
mkdir src/Core/MyDomain/ValueObjects
mkdir src/Core/MyDomain/Repositories
mkdir src/Core/MyDomain/Events
```

### Passo 2: Criar um Value Object
```csharp
// src/Core/MyDomain/ValueObjects/MyId.cs
public class MyId : ValueObject
{
    public int Value { get; }
    
    public MyId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("ID deve ser > 0");
        Value = value;
 }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
 }
}
```

### Passo 3: Criar um Aggregate Root
```csharp
// src/Core/MyDomain/Entities/MyEntity.cs
public class MyEntity : AggregateRoot
{
    public string Name { get; private set; }
    
    private MyEntity() { }
    
    public static MyEntity Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Nome obrigatório");
            
        var entity = new MyEntity { Name = name };
        entity.RaiseDomainEvent(new MyEntityCreatedDomainEvent(entity.Id));
        return entity;
    }
}
```

### Passo 4: Criar um Command
```csharp
// src/Application/MyDomain/Commands/CreateMyEntityCommand.cs
public class CreateMyEntityCommand : ICommand
{
    public string Name { get; set; }
}

// src/Application/MyDomain/Commands/CreateMyEntityCommandHandler.cs
public class CreateMyEntityCommandHandler : ICommandHandler<CreateMyEntityCommand>
{
    private readonly IMyEntityRepository _repository;
    
    public CreateMyEntityCommandHandler(IMyEntityRepository repository)
    {
   _repository = repository;
    }
    
    public async Task Handle(CreateMyEntityCommand command)
    {
        var entity = MyEntity.Create(command.Name);
  await _repository.AddAsync(entity);
    }
}
```

### Passo 5: Criar um Controller
```csharp
// src/API/Controllers/MyEntitiesController.cs
[ApiController]
[Route("api/[controller]")]
public class MyEntitiesController : ControllerBase
{
    private readonly ICommandHandler<CreateMyEntityCommand> _handler;
    
    public MyEntitiesController(ICommandHandler<CreateMyEntityCommand> handler)
    {
        _handler = handler;
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(CreateMyEntityCommand command)
    {
        await _handler.Handle(command);
  return Ok();
  }
}
```

### Passo 6: Testar
```csharp
// tests/UnitTests/MyDomain/MyEntityTests.cs
public class MyEntityTests
{
    [Fact]
    public void Create_WithValidName_ShouldSucceed()
    {
    // Arrange
        var name = "Test Name";
        
      // Act
        var entity = MyEntity.Create(name);

        // Assert
        Assert.Equal(name, entity.Name);
        Assert.Single(entity.DomainEvents);
    }
    
    [Fact]
    public void Create_WithEmptyName_ShouldThrow()
    {
        // Arrange & Act & Assert
     Assert.Throws<ArgumentException>(() => MyEntity.Create(""));
    }
}
```

---

## ? Checklist Antes de Começar

- [ ] Li este guia
- [ ] Entendi a estrutura de pastas
- [ ] Conheço os conceitos principais (Entity, Value Object, Command, Query)
- [ ] Criei a estrutura de pastas do meu domínio
- [ ] Criei um Value Object simples
- [ ] Criei um Aggregate Root
- [ ] Criei um Command e CommandHandler
- [ ] Criei um Controller
- [ ] Escrevi testes unitários
- [ ] Tudo compila sem erros

---

## ?? Próximas Leituras

1. **Para Entender DDD em Profundidade**
   ? Leia `.github/DDD-PATTERNS.md`

2. **Para Ver Exemplos Completos**
   ? Consulte `.github/DDD-PATTERNS.md` (exemplos do Order)

3. **Para Implementar Seu Domínio**
   ? Use `.github/DDD-CHECKLIST.md`

4. **Para Padrões Gerais de Código**
   ? Leia `.github/copilot-guidelines.md`

---

## ?? Erros Comuns

### ? Erro 1: Lógica de Negócio no Controller
```csharp
// ERRADO
[HttpPost]
public async Task<ActionResult> Create(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        return BadRequest();
    var entity = new MyEntity { Name = name };
    // ... salva
}

// CORRETO
[HttpPost]
public async Task<ActionResult> Create(CreateMyEntityCommand command)
{
 await _handler.Handle(command);
    // Lógica está no Aggregate/CommandHandler
}
```

### ? Erro 2: Value Object com Setter
```csharp
// ERRADO
public class Money : ValueObject
{
    public decimal Amount { get; set; } // Tem setter!
}

// CORRETO
public class Money : ValueObject
{
    public decimal Amount { get; private set; } // Sem setter
}
```

### ? Erro 3: Repository Retornando Entity
```csharp
// ERRADO
public class OrderRepository : IOrderRepository
{
    public async Task<OrderItem> GetByIdAsync(int id)
    {
        // Retorna Entity interna!
    }
}

// CORRETO
public class OrderRepository : IOrderRepository
{
    public async Task<Order> GetByIdAsync(OrderId id)
    {
  // Retorna Aggregate Root completo
    }
}
```

### ? Erro 4: Colocar Banco de Dados no Domain
```csharp
// ERRADO
public class Order : AggregateRoot
{
  public async Task SaveAsync(DbContext db)
    {
        db.Orders.Add(this);
        await db.SaveChangesAsync();
    }
}

// CORRETO
public class Order : AggregateRoot
{
    // Apenas lógica de negócio
    public void AddItem(Product product, int quantity)
    {
   // ...
    }
}

// Repository cuida de persistência
public class OrderRepository : IOrderRepository
{
    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
}
```

---

## ?? Dicas Profissionais

### 1. Comece Simples
- Crie um domínio pequeno primeiro
- Expanda gradualmente
- Aprenda com os erros

### 2. Use Factory Methods
```csharp
public static Order Create(string orderNumber)
{
    // Factory method garante estado válido
    return new Order { OrderNumber = orderNumber };
}
```

### 3. Value Objects para Validação
```csharp
// Ao invés de: string email
// Use: Email valueObject que valida no construtor
var email = new Email("user@example.com"); // Válido sempre
```

### 4. Events para Auditoria e Comunicação
```csharp
order.RaiseDomainEvent(new OrderCreatedDomainEvent(...));
// Depois alguém cuida do evento (enviar email, etc)
```

### 5. Testes São Essenciais
```csharp
// Faça testes de caso negativo também
[Fact]
public void AddItem_WhenOrderProcessed_ShouldThrow()
{
    var order = Order.Create("123");
    order.Process();
    
    Assert.Throws<InvalidOperationException>(
        () => order.AddItem(productId, price, qty)
    );
}
```

---

## ?? Perguntas Frequentes

**P: Quando uso Command vs Query?**  
R: Command quando altera dados (CreateOrderCommand), Query quando apenas lê (GetOrderByIdQuery).

**P: Value Object é como um enum?**  
R: Parecido, mas mais poderoso. Pode ter validação, métodos e múltiplas propriedades.

**P: Onde colocar validação?**  
R: No Domain (Value Objects, Aggregate Roots), não no Controller.

**P: Preciso de Repository para tudo?**  
R: Sim, é a interface entre Domain e Infrastructure. Garante isolamento.

**P: E se meu Aggregate é muito grande?**  
R: Considere dividir em Aggregates menores. Cada um com seu Repository.

---

## ?? Recursos Essenciais

- ?? **INDEX.md** - Mapa completo de documentação
- ?? **copilot-guidelines.md** - Padrões gerais
- ?? **DDD-PATTERNS.md** - Exemplos completos
- ?? **DDD-CHECKLIST.md** - Guia step-by-step
- ?? **[DDD - Eric Evans](https://www.domainlanguage.com/ddd/)** - Livro seminal

---

## ?? Próximo Passo

**Escolha um destes caminhos:**

### Caminho 1: Aprender Mais Sobre DDD
? Leia `.github/DDD-PATTERNS.md` (20 min)

### Caminho 2: Implementar Seu Primeiro Domínio
? Use `.github/DDD-CHECKLIST.md` como guia (1-2 horas)

### Caminho 3: Entender Padrões de Código
? Leia `.github/copilot-guidelines.md` (20 min)

---

## ?? Precisa de Ajuda?

1. **Não entendi um conceito?**
   ? Leia a seção relevante em `DDD-PATTERNS.md`

2. **Não sei por onde começar?**
   ? Siga `.github/DDD-CHECKLIST.md` passo a passo

3. **Tenho dúvida sobre código?**
   ? Verifique exemplos em `DDD-PATTERNS.md`

4. **Preciso seguir um padrão?**
   ? Consulte `copilot-guidelines.md`

---

## ?? Você Está Pronto!

Agora você entende:
- ? Arquitetura em camadas (DDD)
- ? Agregados e Value Objects
- ? Commands e Queries
- ? Repositories
- ? Testes
- ? Estrutura de pastas

**Comece a codificar! ??**

---

**Última atualização:** Janeiro 2025

**Dúvidas?** Consulte `.github/INDEX.md` para navegação completa.
