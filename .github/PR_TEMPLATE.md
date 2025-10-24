# ??? Implementação da Arquitetura DDD com Projetos C#

## ?? Descrição

Esta PR implementa a estrutura completa de **Domain-Driven Design (DDD)** no projeto Current, seguindo as diretrizes documentadas em `.github/DDD-PATTERNS.md` e `.github/copilot-guidelines.md`.

## ? Mudanças Principais

### ?? Estrutura de Projetos Criados

| Projeto | Descrição |
|---------|-----------|
| **Current.Core.SharedKernel** | Código base reutilizável entre domínios |
| **Current.Core.Domain.Orders** | Domínio de Pedidos com Agregados e Value Objects |
| **Current.Application** | Camada de aplicação com Commands/Queries |
| **Current.Infrastructure** | Implementações técnicas (Repositories, DbContext) |
| **Current.API** | ASP.NET Core Web API com endpoints |
| **Current.Tests.Unit** | Testes unitários com xUnit |

### ?? Classes Base Implementadas (SharedKernel)

- **Entity**: Classe base para todas as entidades
- **AggregateRoot**: Base para raízes agregadas com suporte a eventos
- **ValueObject**: Base para objects de valor imutáveis
- **DomainEvent**: Base para eventos de domínio
- **DomainException**: Base para exceções de domínio

### ?? Exemplo de Agregado (Order)

Implementação completa de um agregado de pedidos com:
- ? Value Objects (`OrderId`, `OrderStatus`)
- ? Ciclo de vida do pedido (Pending ? Processing ? Shipped ? Delivered)
- ? Domain Events (OrderCreated, OrderProcessed, OrderShipped, OrderDelivered, OrderCancelled)
- ? Validações de regras de negócio
- ? 9 testes unitários cobrindo todos os cenários

### ?? Referências Entre Projetos

```
API ? Application ? Domain ? Infrastructure
       ?   ?
   SharedKernel
```

- ? Domain.Orders ? SharedKernel
- ? Application ? Domain.Orders + SharedKernel
- ? Infrastructure ? Domain + Application + SharedKernel
- ? API ? Todos os anteriores
- ? Tests ? Domain + Application + SharedKernel

### ?? GitHub Actions

Criado workflow de build automatizado que:
- ? Executa em PRs para `develop` e `main`
- ? Compila a solução
- ? Executa testes unitários
- ? Gera relatório de testes
- ? Falha se testes não passarem

## ?? Testes Implementados

**9 testes unitários** cobrindo:
- ? Criação de pedido com dados válidos
- ? Validação de dados inválidos
- ? Transição de status (Pending ? Processing)
- ? Transição de status (Processing ? Shipped)
- ? Transição de status (Shipped ? Delivered)
- ? Cancelamento de pedido
- ? Cancelamento bloqueado em estado inválido
- ? Eventos de domínio disparados

## ?? Checklist

- [x] Estrutura DDD implementada
- [x] Todos os projetos criados com .NET 8.0
- [x] Referências entre projetos configuradas
- [x] Classes base de DDD implementadas
- [x] Exemplo de Agregado completo
- [x] Testes unitários cobrindo lógica
- [x] GitHub Action de build criado
- [x] Solução compila sem erros
- [x] Testes passam 100%

## ?? Como Validar

### Build Local
```bash
dotnet build Current.sln
dotnet test
```

### GitHub Actions
O workflow `build.yml` será executado automaticamente ao fazer push desta PR

## ?? Referências

- [DDD Patterns]((.github/DDD-PATTERNS.md))
- [Copilot Guidelines](.github/copilot-guidelines.md)
- [Checklist DDD](.github/DDD-CHECKLIST.md)

## ?? Próximos Passos

- [ ] Implementar primeiro Command/Query Handler
- [ ] Configurar DbContext e Repositories
- [ ] Implementar endpoints da API
- [ ] Adicionar integração com banco de dados
- [ ] Expandir com mais domínios

## ?? Reviewers

- Arquiteto de Software - Validar estrutura DDD
- Tech Lead - Validar padrões de código
- QA - Validar testes

---

**Type**: Feature  
**Breaking Changes**: Não  
**Migration Needed**: Não
