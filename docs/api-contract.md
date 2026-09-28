# API-контракты

Базовый URL: `/api`.

Все endpoint'ы, кроме внешнего mock-входа и `/api/health`, требуют JWT Bearer.

## Health

`GET /api/health`

```json
{ "status": "ok", "service": "cipso-registry-api" }
```

## UC-00. Вход через внешнего провайдера — заглушка

### Доступные учебные учётные записи

`GET /api/auth/stub/accounts`

Endpoint публичный только потому, что он заменяет экран внешнего провайдера в учебном контуре.

### Mock-вход MAX / Госуслуги

`POST /api/auth/stub/external`

```json
{
  "provider": "MAX",
  "accountId": "<uuid>"
}
```

Либо:

```json
{
  "provider": "Gosuslugi",
  "accountId": "<uuid>"
}
```

Ответ:

```json
{
  "token": "<jwt>",
  "accountId": "<uuid>",
  "displayName": "Демо-секретарь",
  "role": "Operator",
  "provider": "MAX",
  "expiresAt": "2026-09-28T20:00:00+00:00"
}
```

Это **не реальная интеграция** с MAX или Госуслугами/«Госключом».

## UC-11. Учётные записи

Только `AutomationEngineer`.

- `GET /api/accounts`
- `POST /api/accounts`

```json
{
  "displayName": "Учебный пользователь",
  "role": "Observer",
  "citizenRegistryNumber": "TEST-0001"
}
```

Самостоятельной регистрации нет.

## Призывники

`GET /api/citizens?search=...`

`POST /api/citizens` — Operator/Manager.

```json
{
  "registryNumber": "TEST-0013",
  "lastName": "Примеров",
  "firstName": "Иван",
  "middleName": "Иванович",
  "birthDate": "2001-05-10",
  "email": "example@example.test",
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

## Военкоматы

`GET /api/offices`

Используется при создании повестки и фильтрации реестра.

## Повестки военного учёта

`GET /api/summons?status=Issued&officeId=<uuid>&search=...&page=1&pageSize=20`

Ответ содержит `items`, `page`, `pageSize`, `total`, `totalPages`. `pageSize`: 1–100.

`GET /api/summons/{id}` — карточка: призывник, военкомат, дата формирования, срок явки, статус, история, уведомления, обращения, документы и аудит.

`POST /api/summons` — Operator/Manager.

```json
{
  "number": "CIPSO-2026-0030",
  "citizenId": "<uuid>",
  "authorityOfficeId": "<uuid>",
  "createdByEmployeeId": "<uuid>",
  "issuedAt": "2026-09-28",
  "dueAt": "2026-10-03T09:00:00Z",
  "reason": "Явка в военный комиссариат для уточнения документов воинского учёта",
  "comment": "Синтетические данные"
}
```

`PATCH /api/summons/{id}/status` — Operator/Manager.

`POST /api/summons/{id}/notifications` — Operator/Manager; реальная отправка отсутствует.

`POST /api/summons/{id}/appeals` — Operator/Manager.

## Ошибки

- `400 Bad Request` — некорректные данные;
- `401 Unauthorized` — отсутствует/истёк JWT или неверная учебная учётная запись;
- `403 Forbidden` — роль не имеет права;
- `404 Not Found` — сущность не найдена;
- `409 Conflict` — конфликт бизнес-правил.
