## Diretrizes de Desenvolvimento com Copilot

> ?? **Arquivo Principal**: `copilot-guidelines.md`
> ?? **Padrões DDD**: `DDD-PATTERNS.md`

Este diretório contém configurações e diretrizes para auxiliar no desenvolvimento consistente da aplicação Current.

### ?? Arquivos

- **`copilot-guidelines.md`** - Diretrizes gerais de desenvolvimento, padrões de código, testes, segurança e performance
- **`DDD-PATTERNS.md`** - Detalhes completos sobre padrões Domain-Driven Design e exemplos práticos
- **`copilot-instructions.md`** - Instruções para GitHub Copilot (arquivo legado, veja copilot-guidelines.md)

### ?? Objetivo

Estabelecer um padrão claro para como o GitHub Copilot (e qualquer desenvolvedor) deve:
- Estruturar e organizar o código seguindo **Domain-Driven Design**
- Nomear variáveis, funções e classes
- Implementar testes
- Garantir segurança
- Otimizar performance
- Documentar funcionalidades
- Trabalhar com Agregados, Value Objects e Eventos de Domínio

### ?? Como Usar

1. **Antes de Começar um Novo Desenvolvente**
   - Leia o arquivo `copilot-guidelines.md` integralmente
   - Revise `DDD-PATTERNS.md` para entender arquitetura

2. **Ao Escrever Código**
   - Consulte as seções relevantes de padrões
   - Siga as convenções de nomenclatura
   - Aplique os padrões de código apresentados
 - Respeite o isolamento de camadas DDD

3. **Ao Criar um Novo Domínio**
   - Consulte a estrutura em `DDD-PATTERNS.md`
   - Crie as pastas apropriadas (Entities, ValueObjects, Events, etc)
   - Use os exemplos como template
   - Siga o checklist de DDD

4. **Ao Revisar Pull Requests**
   - Use o checklist de revisão de código
   - Verifique conformidade com DDD
   - Valide isolamento de camadas
   - Solicite ajustes se necessário

5. **Ao Interagir com Copilot**
   - Use os prompts recomendados na seção "Interação com Copilot"
   - Sempre revise e adapte o código gerado
- Reporte patterns não seguidos

### ?? Seções Principais

#### copilot-guidelines.md

1. **Princípios Fundamentais**
   - Qualidade, Consistência, Segurança, Performance, Manutenibilidade

2. **Domain-Driven Design**
   - Conceitos fundamentais
   - Cinco camadas: SharedKernel, Domain, Application, Infrastructure, API
   - Padrões DDD: Aggregate Roots, Value Objects, Repository Interfaces, Domain Services

3. **Padrões de Código**
 - Estrutura de arquivos
   - Convenções de nomenclatura
   - Isolamento de camadas
   - Padrões de implementação

4. **Testes**
   - Cobertura mínima (80%) e objetivo (90%)
   - Padrão AAA (Arrange, Act, Assert)
   - Nomenclatura de testes

5. **Commits e Versionamento**
   - Conventional Commits
   - Gitflow Workflow

6. **Segurança**
   - Checklist incluindo DDD-specific items
   - Proteção de credenciais

7. **Performance**
   - Otimizações esperadas
   - Exemplos de queries

#### DDD-PATTERNS.md

1. **Arquitetura DDD Visual**
   - Diagrama de camadas e dependências

2. **SharedKernel**
   - Estrutura
   - Classes base: Entity, AggregateRoot, ValueObject
   - Exemplos completos

3. **Domain Layer**
   - Estrutura por domínio
   - Entities e Aggregate Roots
   - Value Objects
   - Repository Interfaces
   - Domain Events
   - Domain Services
   - Exemplos completos

4. **Application Layer**
   - Estrutura por domínio
   - Commands e Command Handlers
   - Queries e Query Handlers
   - DTOs
   - Mappings
   - Domain Event Handlers
   - Exemplos completos

5. **Infrastructure Layer**
   - DbContext e Repositories
   - Estrutura

6. **API Layer**
   - Controllers
   - Exemplo de integração

7. **Fluxo Completo**
   - Exemplo passo a passo: Criar Pedido

8. **Checklist DDD**
   - Verificações para novos domínios

### ??? Estrutura de Projeto

```
Current/
??? src/
?   ??? Core/
?   ?   ??? SharedKernel/
?   ?   ??? [Domain1]/, [Domain2]/, ...
?   ??? Application/
?   ?   ??? [Domain1]/, [Domain2]/, ...
?   ?   ??? Common/
?   ??? Infrastructure/
?   ?   ??? Persistence/
?   ?   ??? ...
?   ??? API/
??? tests/
    ??? UnitTests/
  ??? IntegrationTests/
    ??? E2ETests/
```

### ?? Manutenção

Este documento deve ser atualizado periodicamente:
- Quando novos padrões DDD forem identificados
- Quando tecnologias/frameworks mudarem
- A cada release major do projeto

---

**Última atualização**: Janeiro 2025
