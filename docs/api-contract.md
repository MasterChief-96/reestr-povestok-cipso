# API-контракты

Базовый URL: `/api`.

Кроме `/api/health` и `/api/auth/login`, endpoint'ы требуют JWT Bearer token.

## Health

`GET /api/health`

```json
{ "status": "ok", "service": "cipso-registry-api" }
```

## Authentication

`POST /api/auth/login`

```json
{
  "username": "operator",
  "password": "demo123!"
}
```

Ответ:

```json
{
  "token": "<jwt>",
  "username": "operator",
  "displayName": "Демо-оператор",
  "role": "Operator",
  "expiresAt": "2026-09-28T16:00:00+00:00"
}
```

Роли:

- `Operator` — чтение и изменение;
- `Manager` — чтение и изменение;
- `Observer` — только чтение.

## Citizens

`GET /api/citizens?search=...`

`POST /api/citizens` — только Operator/Manager.

```json
{
  "registryNumber": "TEST-0002",
  "lastName": "Петров",
  "firstName": "Петр",
  "middleName": "Петрович",
  "birthDate": "2000-01-01",
  "email": "petrov@example.test",
  "phone": "+70000000000",
  "address": {
    "postalCode": "000000",
    "region": "Тестовый регион",
    "city": "Тестоград",
    "street": "Учебная",
    "building": "1",
    "apartment": "1"
  }
}
```

## Offices

`GET /api/offices`

## Summons

`GET /api/summons?status=Issued&officeId=<uuid>&search=...&page=1&pageSize=20`

Ответ:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "total": 0,
  "totalPages": 0
}
```

Ограничение `pageSize`: 1–100.

`GET /api/summons/{id}` — детальная карточка с историей, уведомлениями, обращениями, документами и аудитом.

`POST /api/summons` — только Operator/Manager.

```json
{
  "number": "CIPSO-2026-0002",
  "citizenId": "<uuid>",
  "authorityOfficeId": "<uuid>",
  "createdByEmployeeId": "<uuid>",
  "issuedAt": "2026-09-28",
  "dueAt": "2026-10-03T09:00:00Z",
  "reason": "Учебное оповещение",
  "comment": "Синтетические данные"
}
```

`PATCH /api/summons/{id}/status` — только Operator/Manager.

```json
{
  "status": "Acknowledged",
  "actor": "demo.operator",
  "comment": "Подтверждено в демонстрации"
}
```

`POST /api/summons/{id}/notifications` — только Operator/Manager.

```json
{
  "channel": "Email",
  "destination": "demo@example.test"
}
```

`POST /api/summons/{id}/appeals` — только Operator/Manager.

```json
{
  "type": "Clarification",
  "text": "Просьба уточнить время",
  "actor": "demo.operator"
}
```

## Ошибки

Основные коды:

- `400 Bad Request` — некорректные данные;
- `401 Unauthorized` — отсутствует или истек JWT;
- `403 Forbidden` — роль не дает права на изменение;
- `404 Not Found` — сущность не найдена;
- `409 Conflict` — конфликт бизнес-правил, например дубликат номера.
