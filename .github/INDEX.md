# ?? Índice de Documentação do Projeto Current

## ?? Visão Geral

Este documento serve como ponto de entrada para toda a documentação de diretrizes do projeto Current.

---

## ?? Arquivos Disponíveis

### 1. **copilot-guidelines.md** (Principal - 20KB)
Diretrizes gerais e padrões de desenvolvimento do projeto.

**Conteúdo:**
- ? Princípios Fundamentais (5)
- ? Domain-Driven Design (Conceitos e Padrões)
- ? Estrutura de Arquivos (DDD-based)
- ? Padrões de Nomenclatura
- ? Isolamento de Camadas
- ? Padrões de Código (Métodos, Tratamento de Erros, Comentários, Logging)
- ? Testes (AAA Pattern, Nomenclatura)
- ? Commits (Conventional Commits)
- ? Segurança (Checklist + Exemplos)
- ? Performance (Otimizações)
- ? Documentação
- ? Integração Contínua
- ? Checklist de Code Review
- ? Interação com Copilot

**Quando usar:**
- Como referência principal para desenvolvimento
- Ao revisar código
- Ao interagir com Copilot

---

### 2. **DDD-PATTERNS.md** (Detalhado - 26KB)
Padrões avançados de Domain-Driven Design com exemplos completos.

**Conteúdo:**
- ? Arquitetura DDD Visual
- ? Camadas em Detalhes:
  - SharedKernel (Entidades Base, Value Objects, Events)
  - Domain (Entities, Value Objects, Repositories, Services, Events)
  - Application (Commands, Queries, Handlers, DTOs, Mappings)
  - Infrastructure (DbContext, Repositories)
  - API (Controllers)
- ? Exemplos Práticos Completos
- ? Fluxo Completo: Criar Pedido
- ? Checklist DDD

**Quando usar:**
- Ao criar um novo Domínio/Agregado
- Para entender a arquitetura em profundidade
- Como template para novos padrões
- Ao revisar implementações DDD

---

### 3. **COPILOT_GUIDELINES_README.md** (Índice)
Visão geral das diretrizes e como usar.

**Conteúdo:**
- ? Visão Geral
- ? Índice de Arquivos
- ? Como Usar
- ? Estrutura de Projeto
- ? Referências

**Quando usar:**
- Como primeira leitura
- Para entender a organização das diretrizes
- Para orientar novos desenvolvedores

---

### 4. **copilot-instructions.md** (Legado - 11KB)
Versão anterior mais compacta das diretrizes.

**Status:** Mantido por compatibilidade
**Recomendação:** Use `copilot-guidelines.md` em seu lugar

---

### 5. **DDD-CHECKLIST.md** (Prático - 9KB)
Checklist detalhado para implementar novos componentes DDD.

**Conteúdo:**
- ? Checklist: Criar novo Agregado
- ? Checklist: Application Layer
- ? Checklist: Infrastructure Layer
- ? Checklist: API Layer
- ? Checklist: Testes
- ? Checklist: Segurança & DDD
- ? Checklist: Documentação
- ? Checklist: Git & CI/CD
- ? Checklist Final

**Quando usar:**
- Ao implementar novo domínio/agregado
- Como referência rápida
- Para garantir nenhum item foi esquecido

---

### 6. **INDEX.md** (Este arquivo - 7KB)
Ponto de entrada único para toda documentação.

**Conteúdo:**
- ? Visão geral de todos os arquivos
- ? Primeiros passos organizados por caso de uso
- ? Estrutura do projeto
- ? Checklists rápidos
- ? Dicas e suporte

---

## ?? Primeiros Passos

### 1. Para Novos Desenvolvedores
```
1. Leia: COPILOT_GUIDELINES_README.md (visão geral)
2. Leia: copilot-guidelines.md (padrões gerais)
3. Consulte: DDD-PATTERNS.md (conforme necessário)
```

### 2. Para Criar um Novo Domínio
```
1. Consulte estrutura em DDD-PATTERNS.md
2. Use exemplos como template
3. Siga checklist DDD
4. Teste isolamento de camadas
```

### 3. Para Code Review
```
1. Use checklist em copilot-guidelines.md
2. Valide padrões DDD
3. Verifique segurança
4. Confirme cobertura de testes
```

### 4. Ao Usar Copilot
```
1. Leia seção "Interação com Copilot"
2. Use prompts específicos
3. Revise código gerado
4. Adapte aos padrões
```

---

## ?? Estrutura do Projeto (DDD)

```
Current/
??? src/
?   ??? Core/
?   ?   ??? SharedKernel/      # Código reutilizável
?   ?   ?   ??? Entities/
?   ?   ?   ??? ValueObjects/
? ?   ?   ??? Events/
?   ?   ?   ??? Specifications/
?   ?   ?   ??? Exceptions/
?   ?   ??? Orders/  # Exemplo: Domínio de Pedidos
?   ?   ?   ??? Entities/
?   ?   ?   ??? ValueObjects/
?   ?   ?   ??? Repositories/
?   ?   ?   ??? Services/
?   ?   ?   ??? Events/
?   ?   ?   ??? Exceptions/
?   ? ??? Customers/          # Exemplo: Domínio de Clientes
?   ?   ??? ...
?   ??? Application/
?   ?   ??? Orders/
?   ?   ?   ??? Commands/
?   ?   ?   ??? Queries/
?   ?   ?   ??? Handlers/
?   ?   ?   ??? DTOs/
?   ?   ?   ??? Mappings/
?   ?   ?   ??? Validations/
? ?   ??? Common/
?   ??? Infrastructure/
?   ?   ??? Persistence/
?   ?   ?   ??? Data/
?   ?   ?   ??? Repositories/
?   ?   ?   ??? Configurations/
?   ?   ??? ExternalServices/
?   ? ??? Caching/
?   ?   ??? EventBus/
?   ??? API/
?       ??? Controllers/
?       ??? Middleware/
?       ??? Filters/
?       ??? Extensions/
??? tests/
?   ??? UnitTests/
?   ??? IntegrationTests/
?   ??? E2ETests/
??? docs/
?   ??? ...
??? .github/
    ??? copilot-guidelines.md          ? Você está aqui
    ??? DDD-PATTERNS.md
    ??? COPILOT_GUIDELINES_README.md
    ??? ...
```

---

## ? Checklist Rápido

**Antes de começar o desenvolvimento:**

- [ ] Li `copilot-guidelines.md`
- [ ] Entendi a estrutura DDD
- [ ] Revisei `DDD-PATTERNS.md` (se criar novo domínio)
- [ ] Conheço as convenções de nomenclatura
- [ ] Sei o que é um Aggregate Root
- [ ] Conheço a diferença entre Commands e Queries
- [ ] Entendi isolamento de camadas
- [ ] Sei como usar repositórios
- [ ] Entendo eventos de domínio

**Ao desenvolver:**

- [ ] Código segue convenções de nomenclatura
- [ ] Respeito isolamento de camadas
- [ ] Testes inclusos (mínimo 80% cobertura)
- [ ] Sem credenciais em código
- [ ] Commits seguem Conventional Commits
- [ ] Documentação atualizada
- [ ] Code review passou

---

## ?? Referências Principais

### Documentação do Projeto
- [SECURITY.md](../SECURITY.md) - Política de Segurança
- [LICENSE](../LICENSE) - Licença Proprietária
- [README.md](../README.md) - Visão Geral do Projeto

### Padrões Externos
- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Implementing Domain-Driven Design - Vaughn Vernon](https://vaughnvernon.com/)
- [CQRS Pattern - Microsoft](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Clean Code - Robert Martin](https://cleancode.com.br/)

---

## ?? Dicas Importantes

### 1. Domain-Driven Design é Fundamental
Toda lógica de negócio deve estar no **Domain Layer**. Infrastructure apenas implementa, API apenas expõe.

### 2. Value Objects Devem Validar
Validações de negócio ocorrem no domínio (Value Objects e Aggregates), não no banco de dados.

### 3. Eventos são Poderosos
Use Domain Events para comunicação entre agregados e para auditoria.

### 4. Repositories Não são DAOs
Repositórios retornam Aggregate Roots completos, não fragments de dados.

### 5. DTOs são Importantes
Nunca retorne Entities diretamente da API. Use DTOs.

### 6. Isolamento de Camadas é Crítico
Não misture Domain com Infrastructure. Use injeção de dependência.

### 7. Testes São Obrigatórios
Mínimo 80% de cobertura. Priorize lógica de negócio.

### 8. Código Legível é Melhor que Clever
Use nomes descritivos. Code review é melhor que comentários.

---

## ?? Suporte

### Dúvidas sobre Diretrizes
- Consulte o arquivo relevant (.md)
- Verifique exemplos em DDD-PATTERNS.md
- Revise checklist apropriado

### Dúvidas sobre DDD
- Leia [DDD-PATTERNS.md](./DDD-PATTERNS.md) completamente
- Consulte referências em "Padrões Externos"
- Pergunte à equipe no PR

### Segurança
- Consulte [SECURITY.md](../SECURITY.md)
- Siga checklist de segurança
- Reporte vulnerabilidades adequadamente

---

**Última atualização**: Janeiro 2025

**Versão**: 2.0 (Com DDD)

**Status**: Ativo e em evolução

---

*Este índice foi criado para facilitar navegação e descoberta. Consulte os arquivos relevantes conforme sua necessidade.*
