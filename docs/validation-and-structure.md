# Проверка ввода и структура проекта

## Локализация валидации
- Клиент проверяет все поля **до отправки** и показывает русское объяснение непосредственно под нужным полем.
- Для ошибок установлены `aria-invalid`, `aria-describedby`; фокус переходит к первому неверному полю.
- API возвращает `{ "message": "…", "errors": { "citizen.registryNumber": ["…"] } }` для ошибок валидации и бизнес-конфликтов.
- Ошибки автоматической проверки модели ASP.NET Core приведены к тому же формату.
- Повторные реестровые номера, дата явки раньше формирования и отсутствие секретаря имеют собственные сообщения.

## Папки
- `src/frontend/src/features/<section>/pages` — страницы по предметным разделам.
- `src/frontend/src/shared/api` — HTTP-клиент и отдельные API-модули.
- `src/frontend/src/shared/validation` — валидация, нормализация ошибок.
- `src/frontend/src/shared/layout` и `shared/ui` — макет и общие компоненты.
- `src/frontend/src/styles` — стили по тематике.
- `src/backend/Cipso.Registry.Api/Validation` — единые ответы API.
- `src/backend/Cipso.Registry.Api/Domain/Entities` — отдельные файлы сущностей.

## Проверка
В каталоге `src/frontend` выполните `npm install && npm test && npm run build`.
В корне: `dotnet test tests/Cipso.Registry.Api.Tests/Cipso.Registry.Api.Tests.csproj`.

Проверить вручную: пустую фамилию, некорректные e-mail и индекс, повторный реестровый номер и срок явки раньше даты формирования.
