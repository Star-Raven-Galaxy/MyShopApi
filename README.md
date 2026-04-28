# MyShopApi – Интернет-магазин

REST API на ASP.NET Core 8, консольный клиент с делегатами/событиями, контейнеризация (Docker, PostgreSQL, Redis, Nginx, Prometheus, Grafana), CI/CD (GitHub Actions).

## Технологии

- ASP.NET Core 8 Web API
- Entity Framework Core (PostgreSQL / InMemory)
- Redis (кэширование)
- Nginx (обратный прокси)
- Prometheus + Grafana (мониторинг)
- Docker, Docker Compose
- GitHub Actions (CI)
- xUnit, Moq (тесты)

## Эндпоинты API (основные)

### Products

- `GET /api/products` – список (кэшируется)
- `GET /api/products/{id}` – получение по id (кэшируется)
- `POST /api/products` – создание
- `PUT /api/products/{id}` – обновление
- `DELETE /api/products/{id}` – удаление

### Customers

- `GET /api/customers` – список
- `POST /api/customers` – создание
- `GET /api/customers/{id}` – получение по id
- `PUT /api/customers/{id}` – обновление
- `DELETE /api/customers/{id}` – удаление

### Orders (дополнительно)

- `GET /api/orders` – список всех заказов
- `GET /api/orders/{id}` – получение заказа по id
- `GET /api/orders/customer/{customerId}` – все заказы покупателя (кэшируется)
- `POST /api/orders` – создание заказа
- `DELETE /api/orders/{id}` – удаление заказа

### Deliveries (дополнительно)

- `GET /api/deliveries/order/{orderId}` – доставка по заказу
- `POST /api/deliveries` – назначение доставки
- `PUT /api/deliveries/{id}` – обновление доставки
- `DELETE /api/deliveries/{id}` – удаление доставки

## Консольный клиент

- Собственный делегат `OnRequestCompleted`
- Событие `RequestCompleted` в классе `ApiService`
- Многоадресная подписка:
  - вывод в консоль
  - запись в файл
  - цветной вывод
- Динамическое добавление/удаление обработчиков через `+=` и `-=`
- Меню клиента позволяет выполнять CRUD для товаров, покупателей, заказов и доставки

- ## Мониторинг

- API отдаёт метрики по адресу `/metrics` (пакет `prometheus-net.AspNetCore`).
- Prometheus опрашивает `/metrics` каждые 15 секунд (настройка в `prometheus.yml`).
- Grafana подключена к Prometheus, дашборд включает панели:
  - **RPS** (запросов в секунду)
  - **Среднее время ответа**
  - **Частота ошибок 5xx**
- Дашборд экспортирован в `dashboard.json` (в папке `docker`).

## CI/CD

Пайплайн GitHub Actions (`.github/workflows/dotnet.yml`) выполняется при `push` в `main`/`develop` и включает:

- установку .NET 8
- восстановление пакетов
- сборку проекта
- запуск unit‑тестов
