# Cadastro de Disciplinas - UCP (Backend)

Sistema de controle de disciplinas para o curso de Engenharia da Computação da UCP.

## 🏗️ Arquitetura
A solução segue os princípios de **Clean Architecture**, dividida em:
- **Api:** Camada de entrada (Controllers, Middlewares, Filtros).
- **Application:** Lógica de aplicação, DTOs, Interfaces e Casos de Uso.
- **Domain:** Entidades de negócio, Interfaces de repositório e regras core.
- **Infrastructure:** Implementações técnicas (Persistência SQLite, Autenticação JWT, Segurança Argon2).
- **Shared:** Código compartilhado entre camadas.

## 🛠️ Tecnologias
- .NET 9 (C#)
- Entity Framework Core (SQLite)
- JWT Bearer Authentication
- Argon2 Password Hashing
- Health Checks para monitoramento

## 🚀 Como Executar

### Pré-requisitos
- .NET SDK 9.0
- Visual Studio 2022 ou VS Code

### Configuração
O arquivo `src/Backend/Api/appsettings.json` contém as configurações básicas. Em produção, você pode sobrescrever via variáveis de ambiente:
- `ConnectionStrings:DefaultConnection`: String de conexão SQLite.
- `Jwt:Secret`: Chave de segurança para o token (mínimo 32 caracteres).
- `AllowedOrigins`: URLs do frontend permitidas no CORS.

### Comandos
No diretório `src/Backend`:
```bash
# Restaurar dependências
dotnet restore

# Rodar a API
dotnet run --project Api/Api.csproj
```
A API estará disponível em `https://localhost:5001` (ou conforme configurado). O endpoint de saúde pode ser acessado em `/health`.

## 🔒 Segurança
- Autenticação via JWT (Bearer).
- Senhas armazenadas com hash Argon2.
- Middleware global para tratamento de exceções.
- Validações de unicidade e integridade nos Services.

## 📊 Monitoramento
A API expõe o endpoint `/health` que verifica:
1. Status da API (Liveness).
2. Conexão com o banco de dados SQLite.
