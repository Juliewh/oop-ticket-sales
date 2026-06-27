# TicketSales — прототип (этапы 3-4)

Сервис продажи билетов. Два реальных сервиса (**Booking**, **Payment**),
внешние зависимости (Events, Bank, Tickets, User) замоканы. Хранилище — in-memory,
без БД. Язык — C# / .NET 8.

## Структура

```
TicketSales.sln
  Shared/            общий код: ошибки, in-memory хранилище, middleware
  BookingService/    реальный сервис  (владелец: Юля, ветка: booking)
  PaymentService/    реальный сервис  (владелец: Саша, ветка: payment)
  ExternalMocks/     моки внешних зависимостей (владелец: Лиля, ветка: mocks)
```

Каждый сервис разбит на папки `Entities / Dto / Mapping / Services / Controllers`.

## Как запустить

```
dotnet build
Bank__Outcome=Success dotnet run --project PaymentService --urls http://localhost:5001
PaymentService__BaseUrl=http://localhost:5001/ dotnet run --project BookingService --urls http://localhost:5000
```

Booking вызывает Payment по HTTP через `HttpPaymentGateway`.
`Bank__Outcome` можно менять на `Success`, `Decline` или `Timeout`.

## Договорённости до раздачи

### 1. Namespace = проект + папки

Файл `BookingService/Entities/Booking.cs` -> namespace `BookingService.Entities`.

### 2. Модель ошибок

- Бизнес-ошибки — кастомные исключения с постфиксом `Exception`,
  наследники `Shared.Errors.DomainException`, у каждого свой `StatusCode`.
- Ошибки аргументов — стандартный `ArgumentException` (его ловит middleware -> 400).
- `Shared.Web.ErrorHandlingMiddleware` превращает исключение в `ErrorResponse` + HTTP-код.

Коды (из части 2 ТЗ): 200, 201, 400, 401, 402, 403, 404, 500.

Чтобы добавить новую бизнес-ошибку — заводим класс в `Shared/Errors/`,
наследуешь `DomainException`, проставляешь `StatusCode`. Всё, middleware подхватит.

### 3. Конвенции DTO

- Простые DTO — `record` с `init`-свойствами (см. `BookingDto`).
- Конвертация сущность -> DTO только через маппер `Expression<Func<TEntity, TDto>>`,
  который напрямую сетит поля (без конструкторов и статических методов). Пример:
  `BookingService/Mapping/BookingMapping.cs`.
- Поля enum-типа в сущности, строка в DTO (контракт из ТЗ) — маппер делает преобразование.

## Что реализовано

- **Booking**: резерв мест через Events, запрет двойной брони, таймаут оплаты
  30 минут -> автоотмена, отмена брони с освобождением мест.
- **Payment**: списание через Bank, одна бронь оплачивается один раз, выдача
  билетов через Tickets после успешной оплаты.
- **ExternalMocks**: банк настраивается на успех/отказ/таймаут; Events, Tickets и Users
  поддерживают пограничные сценарии для тестов.

## Этап 4

Синхронная связка Booking -> Payment сделана через HTTP-клиент. Основной сценарий:
«бронь -> оплата -> билет -> откат при сбое».

SLA основного сценария:

```
BOOKING_URL=http://localhost:5000 REQUESTS=20 MAX_MS=2000 Scripts/measure-sla.sh
```

UML sequence-диаграммы лежат в `Docs/*.puml`.
