# Реестр Повесток ЦИПСО

**ЦИПСО** в рамках этого учебного проекта означает **«Централизованная информационная платформа списков оповещения»**.

> Учебный демонстрационный проект. Он не связан с государственными информационными системами и рассчитан только на синтетические тестовые данные.

Клиент-серверное веб-приложение для учета электронных повесток, статусов оповещения, истории доставки, обращений и сопроводительных документов.

## Стек

- Frontend: React 18 + TypeScript + Vite
- Backend: ASP.NET Core 8 Web API
- ORM: Entity Framework Core 8
- DB: PostgreSQL 16
- API docs: Swagger / OpenAPI
- DevOps: Docker Compose, Nginx
- Диаграммы и проектная документация: Markdown + Mermaid

## Что уже заложено в каркас

- 11 доменных сущностей, из них большинство — объекты предметной области;
- REST API для граждан, подразделений и повесток;
- история изменения статусов;
- mock-отправка уведомлений с фиксацией попыток доставки;
- обращения/обжалования;
- аудит ключевых действий;
- React-интерфейс со сводкой и реестром;
- PostgreSQL + Docker Compose;
- ERD, бизнес-требования, use cases, API-контракты, roadmap и негативные кейсы;
- CI для backend/frontend.

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

## Демо-данные

При первом старте backend создает схему БД и синтетические записи: подразделение, сотрудника, гражданина и тестовую повестку. Для учебной версии используется `EnsureCreated`; переход на полноценные EF Core migrations указан в roadmap.

## Структура

```text
.
├── src/
│   ├── backend/Cipso.Registry.Api/
│   └── frontend/
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

Рекомендуемая схема:

- `main` — стабильная версия;
- `develop` — интеграционная ветка;
- `feature/<short-name>` — функциональность;
- `docs/<short-name>` — документация;
- `fix/<short-name>` — исправления.

Изменения в `main` вносятся через Pull Request после code review.

## Лицензия

Учебный проект. Использование реальных персональных данных не предполагается.
