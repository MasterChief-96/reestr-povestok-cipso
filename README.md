# Реестр Повесток ЦИПСО

**ЦИПСО** в рамках этого учебного проекта означает **«Централизованная информационная платформа списков оповещения»**.

> Учебный демонстрационный проект. Он не связан с государственными информационными системами и рассчитан только на синтетические тестовые данные.

Клиент-серверное веб-приложение для учета электронных повесток, статусов оповещения, истории доставки, обращений и сопроводительных документов.

## Стек

- Frontend: React 18 + TypeScript + Vite
- Backend: ASP.NET Core 8 Web API
- ORM: Entity Framework Core 8 + migrations
- DB: PostgreSQL 16
- Auth: JWT + роли Operator / Manager / Observer
- API docs: Swagger / OpenAPI
- DevOps: Docker Compose, Nginx, GitHub Actions
- Тесты: xUnit + EF Core InMemory
- Диаграммы и проектная документация: Markdown + Mermaid

## Реализовано

- 11 доменных сущностей;
- REST API для адресатов, подразделений и повесток;
- серверные поиск, фильтры и пагинация;
- карточка повестки с историей;
- создание повестки из UI;
- история изменения статусов;
- mock-отправка уведомлений с фиксацией попыток доставки;
- обращения/обжалования;
- аудит ключевых действий;
- JWT-аутентификация;
- role-based authorization;
- EF Core initial migration;
- PostgreSQL + Docker Compose;
- Swagger с Bearer authentication;
- ERD, бизнес-требования, use cases, API-контракты, roadmap и негативные кейсы;
- CI для backend-тестов и frontend build.

## Быстрый запуск

Требования: Docker и Docker Compose plugin.

```bash
cp .env.example .env
docker compose up --build
```

После запуска:

- Web UI: http://localhost:8080
- Backend API: http://localhost:8081
- Swagger: http://localhost:8081/swagger
- PostgreSQL: localhost:5432

Если до перехода на migrations уже запускалась старая версия проекта с тем же Docker volume, для чистого учебного старта удалите старый volume:

```bash
docker compose down -v
docker compose up --build
```

## Демо-аккаунты

Все учетные записи существуют только для учебного контура:

| Логин | Пароль | Роль | Права |
|---|---|---|---|
| `operator` | `demo123!` | Operator | чтение и изменение |
| `manager` | `demo123!` | Manager | чтение и изменение |
| `observer` | `demo123!` | Observer | только чтение |

JWT signing key также является демонстрационным и **не должен использоваться в реальной среде**.

## Демо-данные

После применения миграции backend создает синтетические записи: подразделение, сотрудника, адресата и тестовую повестку.

## Структура

```text
.
├── src/
│   ├── backend/Cipso.Registry.Api/
│   │   ├── Auth/
│   │   ├── Controllers/
│   │   ├── Data/
│   │   ├── Domain/
│   │   ├── Migrations/
│   │   └── Services/
│   └── frontend/
├── tests/
│   └── Cipso.Registry.Api.Tests/
├── docs/
├── .github/
├── docker-compose.yml
└── README.md
```

## Документация

- [Бизнес-требования](docs/requirements.md)
- [Use cases](docs/use-cases.md)
- [Архитектура](docs/architecture.md)
- [ERD](docs/erd.md)
- [API-контракты](docs/api-contract.md)
- [Негативные кейсы](docs/negative-cases.md)
- [Roadmap](docs/roadmap.md)
- [Роли команды](docs/team.md)
- [Чекпоинт 2](docs/checkpoint-2.md)

## Ветки и процесс разработки

- `main` — стабильная версия;
- `develop` — интеграционная ветка;
- `feature/<short-name>` — функциональность;
- `docs/<short-name>` — документация;
- `fix/<short-name>` — исправления.

Изменения в `main` и `develop` рекомендуется вносить через Pull Request после CI и code review.

## Ограничения учебной версии

- нет интеграции с реальными государственными системами;
- нет реальной отправки SMS/e-mail;
- нет реальных персональных данных;
- demo users и JWT key предназначены только для локальной демонстрации;
- бинарные файлы пока не загружаются, хранятся только метаданные.

## Лицензия

Учебный проект. Использование реальных персональных данных не предполагается.
