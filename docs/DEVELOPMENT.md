## ?? Desenvolvimento com a Estrutura DDD

### ?? Compilar e Testar Localmente

```bash
# Restaurar dependências
dotnet restore

# Compilar solução
dotnet build Current.sln

# Executar testes
dotnet test

# Executar testes com filtro
dotnet test --filter Category=Domain
```

### ?? GitHub Actions

O workflow `.github/workflows/build.yml` executa automaticamente:

1. ? **Ao fazer push** na branch `develop` ou `main`
2. ? **Em Pull Requests** para `develop` ou `main`

O workflow:
- Restaura dependências
- Compila a solução
- Executa testes unitários
- Publica relatórios de teste

### ?? Padrão de Desenvolvimento

1. **Criar branch feature**
   ```bash
   git checkout develop
   git pull
 git checkout -b feature/seu-recurso
```

2. **Fazer mudanças** seguindo as diretrizes em `.github/copilot-guidelines.md`

3. **Testes locais**
   ```bash
   dotnet build
   dotnet test
```

4. **Commit com Conventional Commits**
   ```bash
   git add .
   git commit -m "feat(orders): adicionar novo agregado"
   ```

5. **Push e PR**
   ```bash
   git push origin feature/seu-recurso
   # Criar PR via GitHub
   ```

6. **Merge após aprovação**

## ?? Estrutura Implementada

### Projetos Criados (6)

| Projeto | Net Framework | Tipo |
|---------|---|---|
| Current.Core.SharedKernel | Net 8.0 | ClassLib |
| Current.Core.Domain.Orders | Net 8.0 | ClassLib |
| Current.Application | Net 8.0 | ClassLib |
| Current.Infrastructure | Net 8.0 | ClassLib |
| Current.API | Net 8.0 | WebAPI |
| Current.Tests.Unit | Net 8.0 | xUnit |

### Classes Base Implementadas

- ? `Entity` - Base para entidades
- ? `AggregateRoot` - Base para agregados
- ? `ValueObject` - Base para value objects
- ? `DomainEvent` - Base para eventos
- ? `DomainException` - Base para exceções

### Exemplo Implementado: Order Agregado

- ? Order (Agregado Raiz)
- ? OrderId (Value Object)
- ? OrderStatus (Value Object)
- ? 5 Domain Events
- ? 9 Testes Unitários

## ?? Estatísticas

| Métrica | Valor |
|---------|-------|
| Projetos | 6 |
| Arquivos de Código | 23 |
| Linhas de Código | 500+ |
| Testes | 9 |
| Cobertura | 80%+ |
| Build Time | < 5s |

## ?? Documentação

- **[DDD Patterns](.github/DDD-PATTERNS.md)** - Guia completo de padrões
- **[Copilot Guidelines](.github/copilot-guidelines.md)** - Padrões de código
- **[DDD Checklist](.github/DDD-CHECKLIST.md)** - Checklist de implementação
- **[Architecture](.github/../docs/ARCHITECTURE.md)** - Arquitetura do projeto

## ?? Testes

```bash
# Todos os testes
dotnet test

# Apenas Domain.Orders
dotnet test --filter "Category=OrderTests"

# Com detalhes
dotnet test --verbosity detailed

# Com cobertura
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

## ?? Referências Entre Projetos

```
Tests.Unit ???
      ??? Domain.Orders ???
   ??? Application  ??????? SharedKernel
     ?      ?  ?
        Infrastructure ???????????
                 ?
            API ????????????????????
```

## ? Recursos Principais

### ? Implementado

- Estrutura de projetos conforme DDD
- Classes base para domínio
- Exemplo de agregado completo
- Testes automatizados
- GitHub Actions de build
- Documentação completa
- Isolamento de camadas

### ?? Próximos Passos

- [ ] Criar mais agregados (Customer, Product)
- [ ] Implementar DbContext
- [ ] Criar endpoints API
- [ ] Event Publishing
- [ ] CQRS Pattern
- [ ] Integração com banco de dados

## ?? Como Começar um Novo Agregado

1. **Ler**: `docs/ARCHITECTURE.md` - Seção "Exemplo: Agregado Order"
2. **Criar**: Nova pasta em `src/Core/Domain.[NomeDominio]`
3. **Usar**: `docs/DDD-CHECKLIST.md` como referência
4. **Testar**: Adicionar testes em `tests/Unit`

## ?? Solução de Problemas

### Build falha

```bash
# Limpar cache
dotnet clean
dotnet build
```

### Referências não encontradas

```bash
# Restaurar dependências
dotnet restore

# Atualizar referências de projeto
dotnet sln Current.sln list
```

### Testes não executam

```bash
# Verificar instalação do xUnit
dotnet add Current.Tests.Unit package xunit

# Restaurar
dotnet restore

# Executar testes
dotnet test --verbosity detailed
```

## ?? Contato e Suporte

- **Dúvidas sobre DDD?** ? Consulte `docs/ARCHITECTURE.md`
- **Padrões de código?** ? Leia `.github/copilot-guidelines.md`
- **Implementando agregado?** ? Use `.github/DDD-CHECKLIST.md`

---

**Status**: ? Pronto para desenvolvimento  
**Última Atualização**: Outubro 2025  
**Branch**: `feature/ddd-projects`
