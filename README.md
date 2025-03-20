# DentalClinicAPI

## 📌 Visão Geral
A **DentalClinicAPI** é uma API REST desenvolvida em **ASP.NET Core Web API** para gerenciar pacientes, dentistas e agendamentos em uma clínica odontológica. O projeto segue boas práticas de arquitetura, incluindo:

- Padrão **Repository** para acesso ao banco de dados.
- Uso de **AutoMapper** para conversão entre entidades e DTOs.
- Integração com banco de dados **Oracle** via Entity Framework Core.
- Documentação via **Swagger/OpenAPI**.
- Middleware para manipulação de erros.
- Suporte a **injeção de dependência** para serviços e repositórios.

## 🚀 Tecnologias Utilizadas

- **Linguagem**: C#
- **Framework**: ASP.NET Core 8.0
- **Banco de Dados**: Oracle (via Entity Framework Core)
- **ORM**: Entity Framework Core
- **Automapper**: Para mapeamento de DTOs e modelos
- **Swagger**: Para documentação dos endpoints
- **Newtonsoft.Json**: Para serialização JSON

## 📂 Estrutura do Projeto

```
DentalClinicAPI/
│── Controllers/       # Contém os endpoints da API (Pacientes, Dentistas, Agendamentos)
│── DTOs/              # Data Transfer Objects (DTOs) usados para comunicação entre API e clientes
│── Models/            # Modelos de dados (Pacientes, Dentistas, Agendamentos)
│── Mappings/          # Configuração do AutoMapper
│── Repositories/      # Padrão Repository para abstração do banco de dados
│── Services/          # Regras de negócio e lógica aplicada
│── Data/              # Configuração do DbContext e Migrations
│── Migrations/        # Arquivos de migração do banco de dados
│── Program.cs         # Configuração principal da API
│── appsettings.json   # Configuração do banco de dados e outras variáveis
```

## 🏗️ Configuração e Execução do Projeto

### 📥 1. Clonar o repositório
```bash
git clone https://github.com/seu-repositorio/DentalClinicAPI.git
cd DentalClinicAPI
```

### 🛠️ 2. Configurar o Banco de Dados (Oracle)
Edite o arquivo **appsettings.json** e substitua os valores da conexão com o Oracle:
```json
"ConnectionStrings": {
  "OracleConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=SEU_HOST)(PORT=1521))(CONNECT_DATA=(SID=SEU_SID)));"
}
```

### 🔨 3. Aplicar as Migrações e Criar o Banco de Dados
Se for a primeira vez executando, aplique as migrações para criar as tabelas no banco:
```bash
dotnet ef database update
```

### ▶️ 4. Executar a API
```bash
dotnet run
```
A API será iniciada e estará disponível em:
```
http://localhost:5000
```

### 📖 5. Acessar a Documentação Swagger
Acesse o Swagger para visualizar os endpoints e testar requisições:
```
http://localhost:5000/swagger
```

## 🔥 Endpoints Disponíveis
### 🏥 Pacientes
- **GET** `/api/patients` → Retorna todos os pacientes.
- **GET** `/api/patients/{id}` → Retorna um paciente pelo ID.
- **POST** `/api/patients` → Cria um novo paciente.
- **PUT** `/api/patients/{id}` → Atualiza um paciente existente.
- **DELETE** `/api/patients/{id}` → Remove um paciente.

### 🦷 Dentistas
- **GET** `/api/dentists` → Retorna todos os dentistas.
- **GET** `/api/dentists/{id}` → Retorna um dentista pelo ID.
- **POST** `/api/dentists` → Cria um novo dentista.
- **PUT** `/api/dentists/{id}` → Atualiza um dentista existente.
- **DELETE** `/api/dentists/{id}` → Remove um dentista.

### 📅 Agendamentos
- **GET** `/api/appointments` → Retorna todos os agendamentos.
- **GET** `/api/appointments/{id}` → Retorna um agendamento pelo ID.
- **GET** `/api/appointments/by-date/{date}` → Retorna agendamentos em uma data específica.
- **POST** `/api/appointments` → Cria um novo agendamento.
- **PUT** `/api/appointments/{id}` → Atualiza um agendamento existente.
- **DELETE** `/api/appointments/{id}` → Cancela um agendamento.

## 🛠️ Tecnologias e Pacotes Utilizados
| Pacote | Versão |
|--------|--------|
| Microsoft.AspNetCore.Mvc.NewtonsoftJson | 8.0.0 |
| Microsoft.EntityFrameworkCore | 9.0.2 |
| Microsoft.EntityFrameworkCore.Design | 9.0.2 |
| Oracle.EntityFrameworkCore | 9.23.60 |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 |
| Swashbuckle.AspNetCore | 7.3.1 |

## 🎯 Melhorias Futuras
- Implementar autenticação e autorização (JWT).
- Criar testes unitários e de integração.
- Implementar cache para otimização de desempenho.

## 👥 Grupo
**Nome:** Gustavo Araújo Maia **RM:** 553270
**Nome:** Rafael Vida Fernandes **RM:** 553721
**Nome:** Kauã Almeida Silveira **RM:** 552618




