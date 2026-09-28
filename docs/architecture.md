# Архитектура

## Контекст

```mermaid
flowchart LR
    User[Секретарь / Комиссар / Призывник] --> UI[React SPA]
    Engineer[Инженер автоматизации] --> UI
    UI -->|mock external auth| MAX[MAX заглушка]
    UI -->|mock external auth| GOS[Госуслуги / Госключ заглушка]
    MAX --> API[ASP.NET Core Web API]
    GOS --> API
    UI -->|JWT Bearer + REST/JSON| API
    API --> EF[Entity Framework Core]
    EF --> DB[(PostgreSQL)]
    API --> Notify[Mock SMS/e-mail provider]
```

В реальной целевой схеме MAX и Госуслуги/«Госключ» являются внешними провайдерами аутентификации. В учебной версии их сетевые интеграции **не реализованы**: UI вызывает локальный mock endpoint, который имитирует успешный результат внешней аутентификации и получает JWT.

## Компоненты

```mermaid
flowchart TB
    subgraph Frontend
      Login[External auth stub page]
      Registry[Military summons pages]
      Accounts[Account administration]
      Client[API client]
      Login --> Client
      Registry --> Client
      Accounts --> Client
    end

    subgraph Backend
      Auth[AuthController]
      Controllers[Military registry controllers]
      AccountCtrl[AccountsController]
      Token[JwtTokenService]
      Services[Application services]
      Data[AppDbContext]
      Domain[Domain entities]
      Auth --> Token
      Controllers --> Services
      AccountCtrl --> Data
      Services --> Data
      Data --> Domain
    end

    Client --> Auth
    Client --> Controllers
    Client --> AccountCtrl
    Data --> PostgreSQL[(PostgreSQL)]
```

## Ролевой доступ

- Operator / Секретарь — рабочие операции реестра;
- Manager / Комиссар — рабочие операции + контроль/аудит;
- Observer / Призывник — чтение;
- AutomationEngineer — создание системных учётных записей.

## Данные

В БД сохраняются призывники, адреса, военкоматы, сотрудники, повестки, история статусов, уведомления, попытки доставки, обращения, документы, audit events и системные учётные записи.

Все seed-данные синтетические.
