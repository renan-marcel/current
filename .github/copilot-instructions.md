# ?? Instruções para GitHub Copilot - Projeto Current

## Visão Geral

Este arquivo define as diretrizes que o GitHub Copilot deve seguir ao auxiliar no desenvolvimento, manutenção e evolução do projeto Current. Estas instruções garantem consistência, qualidade e alinhamento com os padrões do projeto.

## ?? Princípios Fundamentais

### 1. Qualidade de Código
- ? Priorize legibilidade e manutenibilidade
- ? Siga os padrões de codificação definidos neste projeto
- ? Implemente SOLID principles
- ? Evite code smells e anti-patterns
- ? Mantenha baixa complexidade ciclomática (máximo 10)

### 2. Responsabilidade Comercial
- ? Lembre-se: Este é um software **proprietário e comercial**
- ? Nunca sugira código que viole a licença proprietária
- ? Proteja propriedade intelectual em todas as sugestões
- ? Considere implicações de segurança em todas as mudanças

### 3. Segurança em Primeiro Lugar
- ? Nunca inclua credenciais, chaves ou tokens em código
- ? Valide todas as entradas de usuário
- ? Use autenticação e autorização apropriadas
- ? Criptografe dados sensíveis
- ? Siga OWASP Top 10 guidelines

## ??? Padrões de Arquitetura

### Estrutura de Pastas
```
Current/
??? .github/            # Configurações do GitHub
??? src/  # Código-fonte
?   ??? core/     # Lógica central da aplicação
?   ??? services/        # Serviços de negócio
?   ??? repositories/    # Acesso a dados
?   ??? controllers/     # Controllers (se aplicável)
?   ??? utils/           # Utilitários e helpers
??? tests/     # Testes automatizados
?   ??? unit/           # Testes unitários
?   ??? integration/    # Testes de integração
?   ??? e2e/            # Testes end-to-end
??? docs/        # Documentação
??? README.md           # Este arquivo
```

### Princípios Arquiteturais
- ??? **Clean Architecture**: Camadas bem definidas e independentes
- ?? **Injeção de Dependência**: Facilita testes e desacoplamento
- ?? **Single Responsibility**: Cada classe/função tem uma responsabilidade
- ?? **DRY (Don't Repeat Yourself)**: Reutilize código quando apropriado

## ?? Padrões de Codificação

### Convenções de Nomenclatura

#### Variáveis e Funções
```
? BOM:
const userEmail = "user@example.com";
function calculateTotalPrice() { }
let isAuthenticated = true;

? RUIM:
const ue = "user@example.com";
function calc() { }
let auth = true;
```

#### Classes e Interfaces
```
? BOM:
class UserService { }
interface IUserRepository { }
class AuthenticationManager { }

? RUIM:
class userservice { }
class User_Service { }
```

#### Constantes
```
? BOM:
const MAX_RETRIES = 3;
const API_TIMEOUT_MS = 5000;

? RUIM:
const max_retries = 3;
const timeout = 5000;
```

### Formatação de Código
- ?? Use indentação de 2 ou 4 espaços (consistente com o projeto)
- ?? Máximo 100-120 caracteres por linha
- ?? Uma declaração por linha
- ?? Deixe uma linha em branco entre funções/métodos
- ?? Use aspas consistentes (preferencialmente duplas)

### Comentários e Documentação

#### Comentários de Bloco
```javascript
/**
 * Calcula o preço total incluindo impostos
 * @param {number} basePrice - Preço base do produto
 * @param {number} taxRate - Taxa de imposto (0-1)
 * @returns {number} Preço total com impostos
 */
function calculateTotalPrice(basePrice, taxRate) {
  // ...
}
```

#### Comentários Inline
```javascript
// Use comentários apenas para EXPLICAR POR QUE, não O QUE
? BOM:
// Aplicar retry exponencial para falhas transitórias
for (let i = 0; i < MAX_RETRIES; i++) { }

? RUIM:
// Incrementar i
i++;
```

## ?? Testes

### Cobertura de Testes
- ? Mínimo 80% de cobertura de código
- ? 100% de cobertura para funções críticas de segurança
- ? Testes para casos normais, edge cases e erros

### Estrutura de Testes
```javascript
describe('UserService', () => {
  describe('createUser', () => {
 it('should create a user with valid data', () => {
      // Arrange
      const userData = { name: 'John', email: 'john@example.com' };
   
      // Act
const result = userService.createUser(userData);
      
      // Assert
      expect(result.id).toBeDefined();
    });
    
    it('should throw error with invalid email', () => {
      // ...
 });
  });
});
```

### Nomenclatura de Testes
- ? Use padrão descritivo: `it('should [expected behavior] when [condition]', () => {})`
- ? Evite nomes genéricos como `test1`, `test2`

## ?? Padrões de Gitflow

### Criação de Branches
- `feature/NOME-DA-FEATURE`: Novas funcionalidades
- `fix/NOME-DO-BUG`: Correções de bugs
- `refactor/NOME-DO-REFACTOR`: Refatorações
- `docs/NOME-DA-DOCS`: Documentação
- `chore/NOME-DA-TAREFA`: Tarefas de manutenção

### Exemplo de Branch
```
feature/user-authentication
fix/email-validation-bug
refactor/simplify-service-layer
docs/api-documentation
```

## ?? Padrão de Commits

Utilize **Conventional Commits**:

```
feat: adicionar autenticação de dois fatores
fix: corrigir validação de email
docs: atualizar guia de instalação
refactor: simplificar camada de serviços
test: aumentar cobertura de testes para 85%
chore: atualizar dependências
```

### Commit Semântico
- `feat:` Nova funcionalidade (MINOR)
- `fix:` Correção de bug (PATCH)
- `BREAKING CHANGE:` Mudança incompatível (MAJOR)
- `docs:`, `test:`, `style:`, `refactor:`, `chore:` (sem incremento de versão)

## ?? Segurança

### O Que NÃO Fazer
- ? Commitar `.env` ou arquivos com credenciais
- ? Usar senhas hardcoded
- ? Logar informações sensíveis
- ? Validar apenas no frontend
- ? Usar funções de hash fraco (MD5, SHA1)

### O Que FAZER
- ? Usar variáveis de ambiente para configurações sensíveis
- ? Validar SEMPRE no backend
- ? Usar bcrypt, Argon2 para senhas
- ? Implementar rate limiting
- ? Usar HTTPS em produção
- ? Implementar CORS apropriadamente
- ? Sanitizar inputs do usuário
- ? Usar prepared statements para banco de dados

### Exemplo de Validação Segura
```javascript
? BOM - Validação no backend
// Backend
function validateEmail(email) {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(email)) {
    throw new Error('Email inválido');
  }
  return true;
}

? RUIM - Apenas validação frontend
// Frontend
if (email.includes('@')) { }
```

## ?? Performance

### Otimizações Recomendadas
- ? Use lazy loading quando apropriado
- ? Implemente caching intelligente
- ? Minimize operações de I/O
- ? Use índices em banco de dados
- ? Evite N+1 queries
- ? Comprima respostas HTTP

### Exemplo de Query Otimizada
```javascript
? RUIM - N+1 queries
const users = await User.find();
for (let user of users) {
  user.posts = await Post.find({ userId: user.id });
}

? BOM - Uma query otimizada
const users = await User.find()
  .populate('posts');
```

## ?? Dependências

### Ao Adicionar Novas Dependências
- ? Verifique se é realmente necessária
- ? Considere alternativas menores e mais leves
- ? Verifique a reputação e manutenção do pacote
- ? Revise as vulnerabilidades conhecidas
- ? Documente o motivo da adição
- ? Atualize o arquivo de dependências

### Pacotes Recomendados (Exemplo)
- Testing: Jest, Vitest
- Linting: ESLint, Prettier
- Validação: Zod, Joi
- Security: bcryptjs, jsonwebtoken
- HTTP: Axios, Node-fetch

## ?? Documentação

### O Que Documentar
- ? Funções públicas (sempre)
- ? Classes complexas
- ? Lógica não óbvia
- ? Decisões arquiteturais importantes
- ? Setup e instalação

### Formato de Documentação
```javascript
/**
 * Autenticação de usuário com email e senha
 * 
 * @async
 * @param {string} email - Email do usuário
 * @param {string} password - Senha em texto plano
 * @returns {Promise<{token: string, user: Object}>} Token JWT e dados do usuário
 * @throws {Error} Se credenciais forem inválidas
 * 
 * @example
 * const { token } = await authenticate('user@example.com', 'password123');
 */
async function authenticate(email, password) {
  // ...
}
```

## ?? Debugging

### Práticas Recomendadas
- ? Use console.log() apenas em desenvolvimento
- ? Prefira um logger estruturado em produção
- ? Use níveis de log apropriados (debug, info, warn, error)
- ? Inclua contexto suficiente nos logs

### Exemplo de Logger
```javascript
? BOM
logger.error('Falha na autenticação', {
  email: userEmail,
  attempt: retryCount,
  reason: error.message,
  timestamp: new Date().toISOString()
});

? RUIM
console.log('erro');
```

## ?? Integração Contínua

### Verificações Automáticas
- ? Linting (ESLint, Prettier)
- ? Testes automatizados
- ? Cobertura de código
- ? Análise de segurança (SonarQube, Snyk)
- ? Build successful

### Before Creating a Pull Request
```bash
npm run lint      # Verificar formatação
npm run test    # Executar testes
npm run build     # Compilar (se aplicável)
```

## ? Checklist para Pull Requests

Antes de sugerir mudanças ou criar PRs, verificar:

- [ ] Código segue os padrões de nomenclatura
- [ ] Comentários explicam o POR QUE, não o QUE
- [ ] Testes foram adicionados/atualizados
- [ ] Cobertura de testes acima de 80%
- [ ] Sem console.log() em código de produção
- [ ] Sem credenciais ou dados sensíveis
- [ ] Performance considerada
- [ ] Documentação atualizada
- [ ] Commit segue Conventional Commits
- [ ] Sem conflitos com develop branch
- [ ] Código foi formatado com Prettier
- [ ] ESLint passa sem warnings

## ?? Anti-Patterns a Evitar

```javascript
? Callback Hell
function processUser(id, callback) {
  getUser(id, function(user) {
    getDetails(user.id, function(details) {
 getPreferences(user.id, function(prefs) {
        // Muito aninhado e difícil de ler
      });
    });
  });
}

? Use Async/Await ou Promises
async function processUser(id) {
  const user = await getUser(id);
  const details = await getDetails(user.id);
  const prefs = await getPreferences(user.id);
  return { user, details, prefs };
}

? God Objects
class User {
  // Centenas de métodos fazendo tudo
  saveToDatabase() { }
  sendEmail() { }
  generateReport() { }
  processPayment() { }
}

? Separação de Responsabilidades
class User { /* apenas dados e métodos relacionados */ }
class UserService { /* lógica de negócio */ }
class UserRepository { /* persistência */ }
class EmailService { /* envio de emails */ }
```

## ?? Contato e Suporte

Para dúvidas sobre estas instruções:

- ?? **Repositório**: https://github.com/renan-marcel/current
- ?? **Issues**: Use labels para categorizar
- ?? **Segurança**: Consulte [SECURITY.md](../SECURITY.md)

---

**Última atualização**: Janeiro 2025

**Versão**: 1.0

**Status**: Ativo e em evolução
