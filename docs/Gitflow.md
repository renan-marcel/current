# ?? Gitflow

Este projeto utiliza o **Gitflow** como estratégia de branching para gerenciamento de versões e desenvolvimento colaborativo.

## Branches Principais

### `main` - Produção
- Contém código estável e pronto para produção
- Todos os commits devem estar tagueados com versões
- Merges apenas de `develop` ou `hotfix/*`
- Representa o estado atual de produção

### `develop` - Desenvolvimento
- Branch de integração para funcionalidades em desenvolvimento
- Base para criação de features
- Sempre à frente da `main` com novas funcionalidades
- Código deve estar sempre em estado funcional

## Branches de Suporte

### Feature Branches
Utilizadas para desenvolver novas funcionalidades.

- **Nomenclatura**: `feature/<nome-da-funcionalidade>`
- **Origem**: `develop`
- **Destino**: `develop`
- **Tempo de vida**: Até a funcionalidade ser concluída

#### Comandos

```bash
# Criar uma nova feature
git checkout develop
git checkout -b feature/nova-funcionalidade

# Durante o desenvolvimento
git add .
git commit -m "feat: implementa nova funcionalidade"

# Finalizar a feature
git checkout develop
git merge feature/nova-funcionalidade
git branch -d feature/nova-funcionalidade
```

### Release Branches
Utilizadas para preparar uma nova versão de produção.

- **Nomenclatura**: `release/<versao>`
- **Origem**: `develop`
- **Destino**: `main` e `develop`
- **Tempo de vida**: Até a release ser finalizada
- **Uso**: Ajustes finais, correções de bugs menores, atualização de versão

#### Comandos

```bash
# Criar uma release
git checkout develop
git checkout -b release/1.0.0

# Fazer ajustes necessários
git add .
git commit -m "chore: bump version to 1.0.0"

# Finalizar a release
git checkout main
git merge release/1.0.0
git tag -a v1.0.0 -m "Versão 1.0.0"

# Merge de volta para develop
git checkout develop
git merge release/1.0.0
git branch -d release/1.0.0

# Push das alterações
git push origin main develop --tags
```

### Hotfix Branches
Utilizadas para correções urgentes em produção.

- **Nomenclatura**: `hotfix/<versao>`
- **Origem**: `main`
- **Destino**: `main` e `develop`
- **Tempo de vida**: Até a correção ser aplicada
- **Uso**: Correções críticas que não podem esperar a próxima release

#### Comandos

```bash
# Criar um hotfix
git checkout main
git checkout -b hotfix/1.0.1

# Fazer a correção
git add .
git commit -m "fix: corrige bug crítico em produção"

# Finalizar o hotfix
git checkout main
git merge hotfix/1.0.1
git tag -a v1.0.1 -m "Versão 1.0.1 - Hotfix"

# Merge de volta para develop
git checkout develop
git merge hotfix/1.0.1
git branch -d hotfix/1.0.1

# Push das alterações
git push origin main develop --tags
```

## Workflow Resumido

```
main (produção)
  ?
  ???? hotfix/1.0.1 (correção urgente)
  ?      ?
  ?      ??? merge ? main + develop
  ?
  ???? release/1.0.0 (preparação de release)
  ?      ?
  ?      ??? merge ? main + develop
  ?
develop (integração)
  ?
  ???? feature/login (nova funcionalidade)
  ?      ?
  ?      ??? merge ? develop
  ?
  ???? feature/dashboard (nova funcionalidade)
         ?
     ??? merge ? develop
```

## Boas Práticas

1. **Sempre sincronize antes de criar uma branch**
   ```bash
   git checkout develop
   git pull origin develop
   ```

2. **Use commits semânticos** (Conventional Commits)
   - `feat:` Nova funcionalidade
   - `fix:` Correção de bug
   - `docs:` Documentação
   - `refactor:` Refatoração
   - `test:` Testes
   - `chore:` Tarefas gerais

3. **Mantenha as features pequenas e focadas**
   - Uma feature = uma funcionalidade específica
   - Facilita code review e integração

4. **Delete branches após merge**
   - Mantém o repositório organizado
   - Evita confusão com branches antigas

5. **Tags para todas as versões**
   - Facilita rollback se necessário
   - Mantém histórico de versões claro

## Versionamento Semântico

Este projeto segue o [Semantic Versioning 2.0.0](https://semver.org/):

- **MAJOR.MINOR.PATCH** (ex: 1.2.3)
  - **MAJOR**: Mudanças incompatíveis na API
  - **MINOR**: Novas funcionalidades compatíveis
  - **PATCH**: Correções de bugs compatíveis

## Referências

- [Git Flow Original (Vincent Driessen)](https://nvie.com/posts/a-successful-git-branching-model/)
- [Atlassian Git Flow Tutorial](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow)
- [Conventional Commits](https://www.conventionalcommits.org/)
