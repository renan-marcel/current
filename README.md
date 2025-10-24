# Projeto Current

## Estrutura do Projeto

```
Current/
? src/     # Código-fonte da aplicação
? tests/    # Testes automatizados
? docs/     # Documentação do projeto
? .gitignore  # Arquivos ignorados pelo Git
? README.md   # Este arquivo
```

## ?? Segurança

A segurança é uma prioridade para o projeto Current. Se você descobrir alguma vulnerabilidade de segurança, por favor, consulte nossa [Política de Segurança](SECURITY.md) para saber como reportá-la de forma responsável.

**NÃO reporte vulnerabilidades através de issues públicas.**

## Gitflow

Este projeto utiliza o **Gitflow** como estratégia de branching:

- **`main`**: Branch de produção (código estável)
- **`develop`**: Branch de desenvolvimento (integração de features)
- **`feature/*`**: Branches para novas funcionalidades
- **`release/*`**: Branches para preparação de releases
- **`hotfix/*`**: Branches para correções urgentes em produção

 **Para informações detalhadas sobre o workflow do Gitflow, consulte: [docs/Gitflow.md](docs/Gitflow.md)**

## Como Começar

1. Clone o repositório
2. Trabalhe sempre a partir da branch `develop`
3. Crie uma branch de feature para cada nova funcionalidade
4. Faça commits seguindo o padrão [Conventional Commits](https://www.conventionalcommits.org/)

## Padrão de Commits

Este projeto segue o padrão **Conventional Commits**:

- `feat:` Nova funcionalidade
- `fix:` Correção de bug
- `docs:` Alterações na documentação
- `style:` Formatação, ponto e vírgula faltando, etc
- `refactor:` Refatoração de código
- `test:` Adição ou correção de testes
- `chore:` Atualizações de tarefas de build, configurações, etc

## Documentação

- **?? [Guia Rápido para Iniciantes](.github/QUICK-START.md)** - Comece aqui se é novo no projeto! (10 min)
- **??? [Mapa de Navegação](.github/NAVIGATION-MAP.md)** - Encontre exatamente o que precisa (5 min)
- [Gitflow Workflow](docs/Gitflow.md) - Guia completo do fluxo de trabalho Git
- [Política de Segurança](SECURITY.md) - Como reportar vulnerabilidades
- [Índice de Diretrizes](.github/INDEX.md) - Ponto de entrada para toda documentação
- [Diretrizes de Desenvolvimento](.github/copilot-guidelines.md) - Padrões e boas práticas com DDD
- [Padrões DDD Detalhados](.github/DDD-PATTERNS.md) - Exemplos práticos completos
- [Checklist DDD](.github/DDD-CHECKLIST.md) - Guia de verificação ao implementar componentes
- [Sumário Executivo](.github/SUMMARY.md) - Visão geral das diretrizes criadas

## Licença

Este projeto está sob **Licença Proprietária**. Consulte o arquivo [LICENSE](LICENSE) para mais detalhes.

**Importante**: Este é um software comercial proprietário. Todos os direitos são reservados. O uso, cópia, modificação ou distribuição sem autorização expressa é proibido.

## Contribuidores
