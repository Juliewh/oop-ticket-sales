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
dotnet run --project BookingService
dotnet run --project PaymentService
```

(Каждый сервис поднимается отдельно — на этапе 3 они не общаются между собой.)

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

## Что осталось по коду (TODO)

Все места, где нужна доменная логика владельца, помечены `// TODO` с номером
бизнес-правила из ТЗ. Ищи по `TODO` в своём проекте.

- **Booking**: атомарный резерв мест (правило 12), запрет двойной брони (9, 14),
  таймаут оплаты 30 мин -> автоотмена (11), оркестрация сценария на этапе 4.
- **Payment**: оплата = транзакция (3), одна бронь оплачивается один раз (13),
  отмена брони при неудаче (7).
- **ExternalMocks**: сделать исход банка настраиваемым (успех/отказ/таймаут) для тестов.

## Этап 4

Синхронная связка Booking -> Payment (HTTP-клиент), основной сценарий
«бронь -> оплата -> билет -> откат при сбое», замер SLA, тесты на corner-кейсы 7-14.
