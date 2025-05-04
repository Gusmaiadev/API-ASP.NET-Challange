Aqui está um README.md atualizado para seu projeto, incorporando as novas funcionalidades implementadas:


# DentalClinicAPI

## 📌 Visão Geral
A **DentalClinicAPI** é uma API REST desenvolvida em **ASP.NET Core Web API** para gerenciar pacientes, dentistas e agendamentos em uma clínica odontológica. O projeto segue os princípios SOLID e boas práticas de Clean Code, implementando:

- Padrão **Repository** para acesso ao banco de dados
- Arquitetura em camadas com separação clara de responsabilidades
- **AutoMapper** para conversão entre entidades e DTOs
- Integração com banco de dados **Oracle** via Entity Framework Core
- Documentação via **Swagger/OpenAPI**
- Autenticação JWT para segurança
- Integração com **ML.NET** para recomendações inteligentes de agendamentos
- Middleware para manipulação de erros
- Injeção de dependência para serviços e repositórios

## 🚀 Tecnologias Utilizadas

- **Linguagem**: C#
- **Framework**: ASP.NET Core 8.0
- **Banco de Dados**: Oracle (via Entity Framework Core)
- **ORM**: Entity Framework Core
- **Autenticação**: JWT (JSON Web Tokens)
- **Machine Learning**: ML.NET para análise preditiva
- **Automapper**: Para mapeamento de DTOs e modelos
- **Swagger**: Para documentação dos endpoints
- **Newtonsoft.Json**: Para serialização JSON

## 📂 Estrutura do Projeto

```
DentalClinicAPI/
│── Controllers/       # Contém os endpoints da API 
│── DTOs/              # Data Transfer Objects (DTOs) para comunicação
│── Models/            # Modelos de dados e modelos para ML
│── Mappings/          # Configuração do AutoMapper
│── Repositories/      # Padrão Repository para abstração do banco de dados
│── Services/          # Regras de negócio, lógica e serviços ML
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

### 🛠️ 2. Instalar pacotes necessários
```bash
dotnet add package Microsoft.ML
dotnet add package Microsoft.ML.FastTree
```

### 🛠️ 3. Configurar o Banco de Dados (Oracle)
Edite o arquivo **appsettings.json** e substitua os valores da conexão com o Oracle:
```json
"ConnectionStrings": {
  "OracleConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=SEU_HOST)(PORT=1521))(CONNECT_DATA=(SID=SEU_SID)));"
}
```

### 🔨 4. Aplicar as Migrações e Criar o Banco de Dados
Se for a primeira vez executando, aplique as migrações para criar as tabelas no banco:
```bash
dotnet ef database update
```

### ▶️ 5. Executar a API
```bash
dotnet run
```
A API será iniciada e estará disponível em:
```
https://localhost:7012
http://localhost:5236
```

### 📖 6. Acessar a Documentação Swagger
Acesse o Swagger para visualizar os endpoints e testar requisições:
```
https://localhost:7012/swagger
```

## 🔥 Endpoints Disponíveis

### 🔐 Autenticação
- **POST** `/api/auth/register` → Registra um novo usuário.
- **POST** `/api/auth/login` → Autentica um usuário e retorna um token JWT.
- **GET** `/api/auth/me` → Retorna informações do usuário autenticado.

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

### 🧠 Recomendações com IA
- **POST** `/api/MLRecommendation/train` → Treina o modelo de ML com dados históricos.
- **GET** `/api/MLRecommendation/recommend` → Recomenda os melhores horários para consulta.
- **POST** `/api/MLRecommendation/predict-attendance` → Prevê a probabilidade de comparecimento.

## 🔍 Implementações Principais

### 💎 Princípios SOLID Aplicados
- **S (Single Responsibility)**: Cada classe tem uma única responsabilidade
- **O (Open/Closed)**: Entidades abertas para extensão, fechadas para modificação
- **L (Liskov Substitution)**: Interfaces podem ser substituídas por suas implementações
- **I (Interface Segregation)**: Interfaces pequenas e específicas para cada necessidade
- **D (Dependency Inversion)**: Dependências injetadas via interfaces abstratas

### 📝 Práticas de Clean Code
- Nomes significativos para variáveis, métodos e classes
- Funções pequenas e específicas
- Tratamento adequado de erros
- Modularização do código para facilitar manutenção
- Comentários relevantes apenas quando necessário
- DRY (Don't Repeat Yourself) - Evitando duplicação de código

### 🧠 IA Generativa com ML.NET
Implementamos inteligência artificial para otimizar o agendamento de consultas:

- **Recomendações Inteligentes**: O sistema analisa dados históricos para sugerir os melhores horários para cada paciente.
- **Previsão de Comparecimento**: Prediz a probabilidade de um paciente comparecer à consulta.
- **Treinamento Adaptativo**: O modelo utiliza dados reais ou sintéticos para melhorar ao longo do tempo.
- **Análise de Padrões**: Identifica fatores que influenciam o comparecimento (dia da semana, horário, especialidade).

#### Benefícios da IA no Sistema
- Redução de faltas em consultas
- Otimização da agenda dos dentistas
- Melhor experiência para pacientes
- Abordagem proativa para pacientes com risco de ausência

## 🛠️ Tecnologias e Pacotes Utilizados
| Pacote | Versão | Finalidade |
|--------|--------|------------|
| Microsoft.AspNetCore.Mvc.NewtonsoftJson | 8.0.0 | Serialização JSON |
| Microsoft.EntityFrameworkCore | 9.0.2 | ORM para acesso ao banco |
| Oracle.EntityFrameworkCore | 9.23.60 | Provedor Oracle |
| AutoMapper | 12.0.1 | Mapeamento de objetos |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.0 | Autenticação JWT |
| Microsoft.ML | 3.0.0 | Machine Learning |
| Microsoft.ML.FastTree | 3.0.0 | Algoritmos de árvore de decisão |
| Swashbuckle.AspNetCore | 7.3.1 | Documentação Swagger |

## 🎯 Melhorias Futuras
- Expandir o modelo de ML com mais fatores preditivos
- Implementar um dashboard para visualização de métricas
- Adicionar notificações automáticas baseadas em previsões
- Criar testes unitários e de integração
- Implementar cache para otimização de desempenho

## 👥 Grupo
- **Nome:** Gustavo Araújo Maia **RM:** 553270
- **Nome:** Rafael Vida Fernandes **RM:** 553721
- **Nome:** Kauã Almeida Silveira **RM:** 552618
- **Turma:** 2TDSPS
