# Consultório Privado

**Aplicação web de demonstração para a gestão operacional de um consultório médico**, desenvolvida em **ASP.NET Core MVC (.NET 10)**, com **Entity Framework Core**, **SQL Server** e **Bootstrap**.

O repositório documenta duas provas de conceito complementares: o **desenvolvimento de uma aplicação de gestão** e a sua **auditoria técnico-funcional independente**, com análise de requisitos, arquitetura, dados, segurança e qualidade da solução.

> **Nota:** os dados utilizados nesta demonstração são fictícios. A aplicação não se destina a utilização clínica em produção sem validações e medidas adicionais de segurança, privacidade e conformidade.

## Objetivos

- Estruturar uma aplicação de gestão de consultório em projetos com responsabilidades distintas.
- Implementar operações de gestão de utentes, médicos, especialidades e consultas.
- Aplicar regras de negócio ao agendamento e à disponibilidade dos profissionais.
- Integrar atos clínicos e pagamentos no fluxo operacional.
- Disponibilizar uma visão de conjunto através de um dashboard e de uma agenda semanal.

## Funcionalidades

| Módulo | Funcionalidades principais |
| --- | --- |
| **Utentes** | Registo e gestão da informação dos utentes. |
| **Médicos** | Registo e gestão dos profissionais e respetivas especialidades. |
| **Especialidades** | Organização das especialidades médicas. |
| **Consultas** | Agendamento e gestão de consultas, incluindo duração e validações de disponibilidade. |
| **Horários e indisponibilidades** | Definição de horários semanais e registo de ausências dos médicos. |
| **Atos clínicos** | Gestão dos atos clínicos associados à atividade do consultório. |
| **Pagamentos** | Registo e acompanhamento de pagamentos. |
| **Agenda semanal** | Visualização organizada das consultas por período. |
| **Dashboard** | Consulta de indicadores e informação operacional resumida. |

### Regras de negócio implementadas

O agendamento de consultas contempla validações relacionadas com a **duração da consulta**, a **sobreposição de marcações**, o **horário semanal do médico** e as **indisponibilidades registadas**.

Estas validações procuram assegurar a coerência do agendamento e reduzir conflitos na utilização dos períodos disponíveis. A auditoria posterior identificou, contudo, um cenário em que é possível registar uma consulta para um médico sem horário configurado; ver a secção de auditoria.

## Tecnologias utilizadas

- **C# e .NET 10** — desenvolvimento da aplicação.
- **ASP.NET Core MVC** — organização da aplicação web segundo o padrão Model–View–Controller.
- **Entity Framework Core** — acesso e persistência de dados.
- **Microsoft SQL Server** — sistema de gestão de base de dados relacional.
- **Razor Views** — construção das interfaces MVC.
- **Bootstrap** — estrutura e apresentação responsiva das interfaces.
- **Visual Studio 2026** — ambiente de desenvolvimento.
- **Git e GitHub** — controlo de versões e alojamento do código-fonte.

## Arquitetura da solução

A solução está organizada em quatro projetos, de modo a separar responsabilidades:

```text
Consultorio/
├── Consultorio.slnx
├── .gitignore
├── Consultorio.Domain/
├── Consultorio.Application/
├── Consultorio.Infrastructure/
└── Consultorio.Web/
```

| Projeto | Responsabilidade |
| --- | --- |
| `Consultorio.Domain` | Entidades e conceitos centrais do domínio. |
| `Consultorio.Application` | Organização dos casos de uso e da lógica aplicacional. |
| `Consultorio.Infrastructure` | Infraestrutura de persistência e integração com a base de dados. |
| `Consultorio.Web` | Interface web ASP.NET Core MVC, controladores e vistas. |

**Visão conceptual:**

```text
Utilizador
    |
    v
Consultorio.Web
    |
    v
Consultorio.Application
    |
    v
Consultorio.Domain

Consultorio.Infrastructure
    |
    v
SQL Server
```

O diagrama representa a separação conceptual de responsabilidades, não necessariamente todas as dependências técnicas entre projetos.

## Modelo de dados

O modelo de dados da demonstração contempla entidades relacionadas com:

- Utentes e médicos;
- Especialidades;
- Consultas;
- Horários e indisponibilidades dos médicos;
- Atos clínicos;
- Pagamentos.

A persistência é efetuada através do Entity Framework Core, sobre uma base de dados SQL Server.

## Requisitos para execução

- **.NET 10 SDK**;
- **Visual Studio 2026**, ou outro ambiente compatível com .NET 10;
- **SQL Server** acessível no ambiente de execução;
- Acesso aos pacotes NuGet necessários para restaurar as dependências.

## Configuração e execução local

1. Clonar o repositório e abrir `Consultorio.slnx` no Visual Studio.
2. Restaurar os pacotes NuGet, caso o Visual Studio não o faça automaticamente.
3. Configurar a ligação SQL Server em `Consultorio.Web/appsettings.json` ou, preferencialmente, num ficheiro local `Consultorio.Web/appsettings.Development.json` (excluído do Git).
4. Preparar a base de dados `ConsultorioDemo` de acordo com a configuração do Entity Framework Core utilizada no projeto.
5. Definir **`Consultorio.Web`** como projeto de arranque.
6. Compilar a solução e executar a aplicação.

### Exemplo de ligação local

```json
{
  "ConnectionStrings": {
    "Consultorio": "Server=localhost;Database=ConsultorioDemo;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

> **Atenção:** a instância SQL Server e o procedimento de criação ou atualização do esquema da base de dados dependem da configuração do ambiente. Antes de executar migrações, confirme a estratégia de inicialização da base de dados adotada no projeto.

## Interfaces da aplicação

As capturas seguintes ilustram as principais funcionalidades da prova de conceito. Todas as imagens estão organizadas na pasta [`docs/screenshots`](docs/screenshots/).

### Visão geral e agenda

**Dashboard operacional**

![Dashboard do Consultório Privado](docs/screenshots/01-dashboard.png)

**Agenda semanal**

![Agenda semanal de consultas](docs/screenshots/05-agenda-semanal.png)

### Gestão de utentes, médicos e consultas

**Utentes**

![Gestão de utentes](docs/screenshots/02-utentes.png)

**Médicos — detalhes**

![Detalhes do médico](docs/screenshots/03-medicos-detalhes.png)

**Consultas**

![Gestão de consultas](docs/screenshots/04-consultas.png)

**Detalhes de uma consulta**

![Detalhes da consulta](docs/screenshots/06-consulta-detalhes.png)

### Atos clínicos e pagamentos

**Atos clínicos**

![Gestão de atos clínicos](docs/screenshots/07-atos-clinicos.png)

**Tipos de atos clínicos**

![Tipos de atos clínicos](docs/screenshots/10-tipos-de-atos.png)

**Pagamentos**

![Gestão de pagamentos](docs/screenshots/08-pagamentos.png)

**Detalhes de pagamento**

![Detalhes de pagamento](docs/screenshots/09-pagamento-detalhes.png)

### Disponibilidade médica

**Horários dos médicos**

![Horários dos médicos](docs/screenshots/11-horarios-medicos.png)

**Indisponibilidades dos médicos**

![Indisponibilidades dos médicos](docs/screenshots/12-indisponibilidades.png)

> **Nota:** as imagens destinam-se a ilustrar a aplicação demonstrativa. Confirme que não incluem informação real ou confidencial antes de alterar a visibilidade do repositório.

## Auditoria técnico-funcional — segunda prova de conceito

Após o desenvolvimento da aplicação, foi realizada uma **avaliação crítica da própria solução**, assumindo a perspetiva de consultoria técnico-funcional. O objetivo foi identificar e fundamentar riscos, avaliar regras de negócio, reconhecer boas práticas e formular recomendações — **sem modificar ou corrigir o código durante a auditoria**.

A análise combinou:

- **Ferramentas automáticas:** compilação e analisadores .NET, avisos de dependências NuGet e verificação da configuração;
- **Inspeção manual:** controladores, acesso a dados, validações, tratamento de exceções, autenticação e proteção antifalsificação;
- **Teste funcional:** reprodução de um cenário de agendamento de consulta para um médico sem horário configurado;
- **Avaliação técnico-funcional:** classificação de constatações, impacto, criticidade e recomendações.

### Principais resultados

| Área | Resultado da auditoria |
| --- | --- |
| **Regras de negócio** | Reproduzido um agendamento sem horário médico configurado (TF-001). |
| **Segurança aplicacional** | Não foram identificados mecanismos de autenticação e autorização na aplicação analisada. |
| **Dependências** | Identificados dois avisos de segurança relativos à dependência transitiva `System.Security.Cryptography.Pkcs` 9.0.11; a explorabilidade não foi demonstrada. |
| **Qualidade e manutenção** | Identificados avisos dos analisadores .NET e tratamento genérico de exceções de persistência em quatro controladores. |
| **Tratamento de erros** | Configurado `/Home/Error` sem implementação correspondente identificada na análise estática. |
| **Proteção CSRF** | Verificados atributos antifalsificação nas 21 ações POST explicitamente identificadas. |

O relatório distingue **falhas reproduzidas**, **constatações fundamentadas por análise estática**, **riscos condicionados pelas regras de negócio** e **boas práticas identificadas**. Os comandos e resultados relevantes encontram-se documentados como evidências técnicas.

**Documentação:** [Relatório e evidências da auditoria](docs/Auditoria/).

> **Âmbito:** trata-se de uma prova de conceito com dados fictícios, não de uma certificação de segurança, auditoria de conformidade ou aprovação para utilização clínica em produção. As recomendações não representam alterações já implementadas.

## Estado do projeto

**POC de desenvolvimento concluída para demonstração e POC de auditoria técnico-funcional documentada.** A aplicação inclui os módulos operacionais principais, mas a auditoria identificou limitações que devem ser avaliadas antes de qualquer utilização em produção. A implementação das recomendações e os testes de regressão ficam fora do âmbito desta auditoria.

### Possíveis evoluções

- Automatização de testes e ampliação da cobertura dos cenários de negócio;
- Autenticação, autorização e perfis de acesso;
- Auditoria de operações e reforço de segurança;
- Melhorias de usabilidade e acessibilidade;
- Relatórios operacionais adicionais;
- Preparação para ambientes de implantação controlados.

## Autoria

**Flórida da Graça Botelho Araújo**  
Sistemas de Informação | Engenharia de Dados | Consultoria Técnico-Funcional

[Perfil LinkedIn](https://www.linkedin.com/in/fgaraujo)

---

*Projeto demonstrativo desenvolvido para fins de aprendizagem, consolidação de competências técnicas e apresentação de portefólio.*
