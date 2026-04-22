# 🍔 Good Hamburger - Sistema de Pedidos

Um sistema completo de gerenciamento de pedidos para hamburgueria, feito com .NET 9, Blazor Server e PostgreSQL.

## 🚀 Como Rodar

Se tiver docker instalado localmente, então é só um comando:

```bash
docker-compose up -d
```

A aplicação vai estar rodando em:
- **Web**: http://localhost:5001  
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

Para testar, use o usuário admin já cadastrado:
- **Email**: admin@goodhamburger.com
- **Senha**: Admin@123

## 💡 O Que Esse Sistema Faz

Basicamente, você pode:
- Criar uma conta e fazer login
- Montar pedidos escolhendo sanduíches, acompanhamentos e bebidas
- Ver descontos aplicados automaticamente (combos ganham desconto)
- Listar, editar e excluir seus pedidos
- Buscar pedidos por número

### Regras de Desconto

O sistema dá desconto quando você monta um combo:

| Sanduíche + Batata + Refrigerante | 20% |
| Sanduíche + Refrigerante | 15% |
| Sanduíche + Batata | 10% |
| Só um item | Sem desconto |

## 🏗️ Como está Organizado

Segui uma arquitetura em camadas de Clean Architecture:

```
src/
├── GoodHamburger.Api/         # REST API com JWT
├── GoodHamburger.Domain/      # Regras de negócio e entidades
└── GoodHamburger.Web/         # Interface Blazor Server
```

### Stack Técnica

**Backend:**
- .NET 9 
- ASP.NET Core Identity 
- Entity Framework Core 
- PostgreSQL 

**Frontend:**
- Blazor Server 
- Bootstrap 

**Infra:**
- Docker - tudo roda em containers
- JWT - autenticação stateless

### Entidades com Propriedades Calculadas

`SubTotal`, `DiscountAmount` e `Total` não são guardados no banco. São calculados toda vez. Por quê? Porque se mudar as regras de desconto, os valores ficam sempre corretos. É um pouco menos performático, mas garante consistência.

### Strategy Pattern pros Descontos

O `DiscountCalculator` usa Strategy Pattern. Facilita adicionar novas regras de desconto sem mexer nas entidades. Só criar um novo calculador e trocar no DI.

## 🧪 Testes

Tem testes unitários cobrindo as principais regras:

```bash
cd tests/GoodHamburger.Tests
dotnet test
```

O que tá coberto:
- ✅ Cálculo de descontos (6 cenários diferentes)
- ✅ Totais de pedidos 
- ✅ Validações de DTOs

## 🚧 O Que Ficou de Fora

Algumas coisas não entraram:

**Testes de Integração**  
Tá estruturado mas comentado. O `WebApplicationFactory` não se dá bem com o `ProtectedSessionStorage` do Blazor Server. Precisaria mockar o contexto de autenticação de um jeito diferente.

**Refresh Token**  
Só tem access token. Em produção seria legal ter refresh token pra não precisar fazer login toda hora.

**Paginação**  
A lista de pedidos não pagina. Se tiver mil pedidos, vai carregar tudo. Não é ideal.

**Controle de Concorrência**  
Se dois usuários editarem o mesmo pedido ao mesmo tempo, o último ganha. Deveria ter controle de concorrência otimista.

**Cache**  
O cardápio é sempre buscado do banco. Um Redis resolveria fácil.

**Logs Estruturados**  
Tá usando o log padrão do ASP.NET Core. Seria melhor ter Serilog com sink pro Application Insights ou similar.

**Internacionalização**  
Tá tudo em pt-BR. Adicionar outros idiomas com Resource Files seria tranquilo, mas não era prioridade.

**Observabilidade**  
Talvez adicionar um colector para otel para que seja possível montar gráficos e alertas no grafana por exemplo, além de poder slavar traces e logs também.

## 🔐 Segurança

- Senhas com hash PBKDF2 (ASP.NET Core Identity)
- JWT com expiração de 60 minutos
- FluentValidation em todos os inputs
- Usuário só vê seus próprios pedidos
- CSRF protection nos formulários Blazor

## 📦 Banco de Dados

As migrations rodam automaticamente quando a API sobe. O banco já vem com os itens de exemplo:

**Sanduíches:**
- X Bacon - R$ 7,00
- X Burger - R$ 5,00
- X Egg - R$ 4,50

**Acompanhamento:**
- Batata frita - R$ 2,00

**Bebida:**
- Refrigerante - R$ 2,50

---

## 🛠️ Rodando Localmente (sem Docker)

Se quiser rodar sem Docker pra debugar:

```bash
# 1. Sobe o Postgres
docker run -d -p 5432:5432 \
  -e POSTGRES_USER=goodhamburger \
  -e POSTGRES_PASSWORD=GoodHamburger@123 \
  -e POSTGRES_DB=GoodHamburgerDb \
  postgres:17

# 2. Roda a API
cd src/GoodHamburger.Api
dotnet run

# 3. Roda o Web (em outro terminal)
cd src/GoodHamburger.Web
dotnet run
```

Lembrar de ajustar os `appsettings.json` pra apontar pro localhost.

