# Projeto Current

## ?? Estrutura do Projeto

```
Current/
??? src/ # Código-fonte da aplicação
??? tests/        # Testes automatizados
??? docs/         # Documentação do projeto
??? .gitignore    # Arquivos ignorados pelo Git
??? README.md     # Este arquivo
```

## ?? Gitflow

Este projeto utiliza o **Gitflow** como estratégia de branching:

### Branches Principais

- **`main`**: Branch de produção
  - Contém código estável e pronto para produção
  - Todos os commits devem estar tagueados com versões
  - Merges apenas de `develop` ou `hotfix/*`

- **`develop`**: Branch de desenvolvimento
  - Branch de integração para funcionalidades em desenvolvimento
  - Base para criação de features
  - Sempre à frente da `main` com novas funcionalidades

### Branches de Suporte

#### Feature Branches
- **Nomenclatura**: `feature/<nome-da-funcionalidade>`
- **Origem**: `develop`
- **Destino**: `develop`
- **Uso**: Desenvolvimento de novas funcionalidades

```bash
# Criar uma nova feature
git checkout develop
git checkout -b feature/nova-funcionalidade

# Finalizar a feature
git checkout develop
git merge feature/nova-funcionalidade
git branch -d feature/nova-funcionalidade
```

#### Release Branches
- **Nomenclatura**: `release/<versao>`
- **Origem**: `develop`
- **Destino**: `main` e `develop`
- **Uso**: Preparação para uma nova versão de produção

```bash
# Criar uma release
git checkout develop
git checkout -b release/1.0.0

# Finalizar a release
git checkout main
git merge release/1.0.0
git tag -a v1.0.0 -m "Versão 1.0.0"

git checkout develop
git merge release/1.0.0
git branch -d release/1.0.0
```

#### Hotfix Branches
- **Nomenclatura**: `hotfix/<versao>`
- **Origem**: `main`
- **Destino**: `main` e `develop`
- **Uso**: Correções urgentes em produção

```bash
# Criar um hotfix
git checkout main
git checkout -b hotfix/1.0.1

# Finalizar o hotfix
git checkout main
git merge hotfix/1.0.1
git tag -a v1.0.1 -m "Versão 1.0.1"

git checkout develop
git merge hotfix/1.0.1
git branch -d hotfix/1.0.1
```

## ?? Como Começar

1. Clone o repositório
2. Trabalhe sempre a partir da branch `develop`
3. Crie uma branch de feature para cada nova funcionalidade
4. Faça commits seguindo o padrão [Conventional Commits](https://www.conventionalcommits.org/)

## ?? Padrão de Commits

Este projeto segue o padrão **Conventional Commits**:

- `feat:` Nova funcionalidade
- `fix:` Correção de bug
- `docs:` Alterações na documentação
- `style:` Formatação, ponto e vírgula faltando, etc
- `refactor:` Refatoração de código
- `test:` Adição ou correção de testes
- `chore:` Atualizações de tarefas de build, configurações, etc

## ?? Licença

A definir.

## ?? Contribuidores

A definir.
