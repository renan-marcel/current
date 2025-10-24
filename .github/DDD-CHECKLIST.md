# ? Checklist de Implementação DDD

Este arquivo serve como guia de verificação ao implementar novos componentes seguindo Domain-Driven Design.

---

## ?? Checklist: Criar um Novo Agregado (ex: Orders)

### Estrutura de Pastas
- [ ] Pasta `src/Core/Orders` criada
- [ ] Subpastas criadas:
  - [ ] `Entities/`
  - [ ] `ValueObjects/`
  - [ ] `Repositories/`
  - [ ] `Services/`
  - [ ] `Events/`
- [ ] `Exceptions/`
  - [ ] `Specifications/` (se usar Specification Pattern)

### Domain Layer - Entities
- [ ] `Order.cs` (Aggregate Root):
  - [ ] Herda de `AggregateRoot`
  - [ ] ID é um `ValueObject` específico (`OrderId`)
  - [ ] Construtor privado
  - [ ] Factory method `Create()` público
  - [ ] Métodos public refletem linguagem de negócio
  - [ ] Invariantes respeitados
  - [ ] `RaiseDomainEvent()` chamado apropriadamente
  - [ ] Sem dependências de Infrastructure

- [ ] `OrderItem.cs` (Entity):
  - [ ] Herda de `Entity`
  - [ ] Factory method para criação
  - [ ] Apenas propriedades do agregado pai (`Order`)

### Domain Layer - Value Objects
- [ ] `OrderId.cs`:
  - [ ] Herda de `ValueObject`
  - [ ] Validação no construtor
  - [ ] Imutável (sem setters)
  - [ ] Implementa `GetEqualityComponents()`

- [ ] `OrderStatus.cs` (se enum seria insuficiente):
  - [ ] Herda de `ValueObject`
  - [ ] Valores estáticos predefinidos
  - [ ] Métodos para conversão (FromString, ToString)

- [ ] Outros Value Objects conforme necessário

### Domain Layer - Repositories
- [ ] `IOrderRepository.cs`:
  - [ ] Interface definida
  - [ ] Métodos refletem linguagem ubíqua
  - [ ] Retorna `Order` (Aggregate Root)
  - [ ] Métodos: `GetByIdAsync()`, `AddAsync()`, `UpdateAsync()`, `DeleteAsync()`
  - [ ] Sem implementação (apenas abstração)

### Domain Layer - Services
- [ ] `OrderCalculationService.cs` (se necessário):
  - [ ] Sem estado
  - [ ] Métodos public para lógica compartilhada
  - [ ] Recebe Value Objects como parâmetros
  - [ ] Retorna Value Objects

### Domain Layer - Events
- [ ] `OrderCreatedDomainEvent.cs`:
  - [ ] Herda de `DomainEvent`
  - [ ] Contém informações relevantes
  - [ ] Imutável
  - [ ] Não contém referências completas de Entities

- [ ] Outros eventos conforme necessário

### Domain Layer - Exceptions
- [ ] `OrderNotFoundException.cs`:
  - [ ] Herda de `DomainException`
  - [ ] Mensagem descritiva
  - [ ] Contexto relevante

- [ ] Outras exceções conforme necessário

---

## ?? Checklist: Application Layer

### Estrutura de Pastas
- [ ] Pasta `src/Application/Orders` criada
- [ ] Subpastas criadas:
  - [ ] `Commands/`
  - [ ] `Queries/`
  - [ ] `Handlers/`
  - [ ] `DTOs/`
  - [ ] `Mappings/`
  - [ ] `Validations/`

### Commands & Handlers
- [ ] `CreateOrderCommand.cs`:
  - [ ] Implementa `ICommand`
  - [ ] Propriedades públicas (input)
  - [ ] Sem lógica

- [ ] `CreateOrderCommandHandler.cs`:
  - [ ] Implementa `ICommandHandler<CreateOrderCommand>`
  - [ ] Injeta repositórios via construtor
  - [ ] Valida dados de entrada
  - [ ] Chama métodos do agregado
  - [ ] Persiste via repository
  - [ ] Publica eventos de domínio
  - [ ] Logging apropriado

- [ ] Outros comandos conforme necessário

### Queries & Handlers
- [ ] `GetOrderByIdQuery.cs`:
  - [ ] Implementa `IQuery<OrderDTO>`
  - [ ] Propriedades para filtros

- [ ] `GetOrderByIdQueryHandler.cs`:
  - [ ] Implementa `IQueryHandler<GetOrderByIdQuery, OrderDTO>`
  - [ ] Busca via repository
  - [ ] Mapeia para DTO
  - [ ] Retorna DTO (nunca Entity)

- [ ] Outras queries conforme necessário

### DTOs
- [ ] `OrderDTO.cs`:
  - [ ] Propriedades públicas apenas leitura
  - [ ] Sem métodos de negócio
  - [ ] Mapeia com segurança (não expõe dados sensíveis)

- [ ] `OrderItemDTO.cs`:
  - [ ] Similar ao OrderDTO

### Mappings
- [ ] `OrderMappingProfile.cs`:
  - [ ] Herda de `Profile` (AutoMapper)
  - [ ] `CreateMap<Order, OrderDTO>()`
  - [ ] Mappings de Value Objects para primitivos
  - [ ] Configurações especiais se necessário

### Validations
- [ ] `CreateOrderCommandValidator.cs`:
  - [ ] Implementa `IValidator<CreateOrderCommand>`
  - [ ] Valida cada propriedade
  - [ ] Mensagens de erro em português/ubíqua
  - [ ] Não duplica validação de domínio

### Domain Event Handlers
- [ ] `OrderCreatedDomainEventHandler.cs`:
  - [ ] Implementa `IDomainEventHandler<OrderCreatedDomainEvent>`
  - [ ] Efeitos colaterais apropriados (email, etc)
  - [ ] Logging

---

## ?? Checklist: Infrastructure Layer

### Estrutura
- [ ] Pasta `src/Infrastructure/Persistence/Repositories` existe
- [ ] Pasta `src/Infrastructure/Persistence/Configurations` existe

### DbContext
- [ ] `ApplicationDbContext.cs`:
  - [ ] `DbSet<Order> Orders { get; set; }`
  - [ ] `DbSet<OrderItem> OrderItems { get; set; }`
  - [ ] `OnModelCreating()` aplica configurações

### Repository Implementation
- [ ] `OrderRepository.cs`:
  - [ ] Implementa `IOrderRepository`
  - [ ] Depende de `ApplicationDbContext`
  - [ ] `GetByIdAsync()` com `.Include()` se necessário
  - [ ] `AddAsync()` persiste corretamente
  - [ ] `UpdateAsync()` marca como modificado
  - [ ] `DeleteAsync()` remove
  - [ ] Logging apropriado

### EF Core Configuration
- [ ] `OrderEntityConfiguration.cs`:
  - [ ] Implementa `IEntityTypeConfiguration<Order>`
  - [ ] `ToTable("Orders")`
  - [ ] Propriedades mapeadas corretamente
  - [ ] Value Objects mapeados com `.OwnsOne()` se necessário
  - [ ] Relacionamentos configurados
  - [ ] Índices se necessário

---

## ?? Checklist: API Layer

### Controllers
- [ ] `OrdersController.cs`:
  - [ ] Herda de `ControllerBase`
  - [ ] Rota em `[Route("api/[controller]")]`
  - [ ] `[ApiController]`
  - [ ] POST para criar (injeta CommandHandler)
  - [ ] GET para ler (injeta QueryHandler)
  - [ ] Retorna DTO (não Entity)
  - [ ] Status HTTP apropriados (201, 200, 404, etc)
  - [ ] `[ProducesResponseType(...)]` documentado

---

## ?? Checklist: Testes

### Unit Tests
- [ ] `OrderTests.cs`:
  - [ ] Testa método `Create()`
  - [ ] Testa `AddItem()`
  - [ ] Testa casos de erro
  - [ ] Valida Domain Events
  - [ ] Cobertura ? 80%

- [ ] `OrderIdTests.cs`:
  - [ ] Testa validação
  - [ ] Testa igualdade

### Command Handler Tests
- [ ] `CreateOrderCommandHandlerTests.cs`:
  - [ ] Mock de repositórios
  - [ ] Testa sucesso
  - [ ] Testa casos de erro (cliente não encontrado)
  - [ ] Valida persistência
  - [ ] Valida publicação de eventos

### Query Handler Tests
- [ ] `GetOrderByIdQueryHandlerTests.cs`:
  - [ ] Mock de repositório
  - [ ] Testa retorno do DTO
  - [ ] Testa erro (não encontrado)

### Integration Tests
- [ ] Testa fluxo completo
- [ ] Usa DbContext real (ou em memória)
- [ ] Valida persistência
- [ ] Valida eventos

---

## ?? Checklist: Segurança & DDD

### Isolamento de Camadas
- [ ] Domain não depende de Infrastructure
- [ ] Application não importa Infrastructure (injeção de dependência)
- [ ] Infrastructure implementa interfaces de Domain
- [ ] API não acessa Domain diretamente

### Validação
- [ ] Value Objects validam no construtor
- [ ] Agregates respeitam invariantes
- [ ] Application valida commandos
- [ ] Não há validação duplicada

### Dados Sensíveis
- [ ] Senhas não são logadas
- [ ] Números de cartão não são logados
- [ ] DTOs não expõem dados sensíveis
- [ ] Events não contêm dados sensíveis

### Autorização
- [ ] Controllers validam permissões
- [ ] Repositórios respeitam escopo de usuário
- [ ] Queries filtram por usuário autenticado

---

## ?? Checklist: Documentação

- [ ] README de componentes criado (se complexo)
- [ ] Comentários explicam "por quê", não "quê"
- [ ] DTOs documentados com `[Description]`
- [ ] Events documentados
- [ ] Exceções documentadas
- [ ] Factory methods documentados

---

## ?? Checklist: Git & CI/CD

### Commits
- [ ] Commits seguem Conventional Commits
- [ ] Tipo apropriado (feat, fix, etc)
- [ ] Escopo relacionado a domínio
- [ ] Mensagem descritiva

### Pull Request
- [ ] Código segue padrões DDD
- [ ] Testes inclusos
- [ ] Cobertura ? 80%
- [ ] Sem credenciais
- [ ] Sem warnings do compilador
- [ ] Build passa

### Code Review
- [ ] Padrões DDD respeitados
- [ ] Isolamento de camadas mantido
- [ ] Segurança validada
- [ ] Performance considerada
- [ ] Testes adequados

---

## ?? Checklist Final

Antes de marcar como "done":

- [ ] Estrutura de pastas completa
- [ ] Todas as classes criadas
- [ ] Todos os padrões respeitados
- [ ] Testes com 80%+ cobertura
- [ ] Sem warnings do compilador
- [ ] Código formatado
- [ ] Documentação atualizada
- [ ] Commits bem formados
- [ ] PR aprovado
- [ ] Build passou em CI/CD
- [ ] Deploy pronto

---

**Última atualização**: Janeiro 2025

*Use este checklist regularmente ao desenvolver novos componentes para garantir qualidade e consistência.*
