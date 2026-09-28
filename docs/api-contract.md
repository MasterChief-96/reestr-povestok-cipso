# API-контракты

Базовый URL: `/api`.

## Health

`GET /api/health`

```json
{ "status": "ok" }
```

## Citizens

`GET /api/citizens?search=...`

`POST /api/citizens`

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

`GET /api/summons?status=Issued&search=...`

`GET /api/summons/{id}`

`POST /api/summons`

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

`PATCH /api/summons/{id}/status`

```json
{
  "status": "Acknowledged",
  "actor": "demo.operator",
  "comment": "Подтверждено в демонстрации"
}
```

`POST /api/summons/{id}/notifications`

```json
{
  "channel": "Email",
  "destination": "demo@example.test"
}
```

`POST /api/summons/{id}/appeals`

```json
{
  "type": "Clarification",
  "text": "Просьба уточнить время",
  "actor": "demo.operator"
}
```

## Ошибки

Валидационные и бизнес-ошибки возвращаются как `application/problem+json` с кодами 400/404/409.
