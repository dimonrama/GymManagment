# GymManagment

REST API для управления фитнес-залом: учёт клиентов и тренеров, авторизация и разграничение прав доступа по ролям.

## Возможности

- **Авторизация и аутентификация** — регистрация и вход через JWT (access-токен), с ролями `Member`, `Trainer`, `Admin` и разграничением доступа через `[Authorize(Roles = ...)]`
- **Refresh-токены** — access-токен живёт 15 минут, refresh-токен — 30 дней, с ротацией при каждом обновлении и автоматическим отзывом всех токенов пользователя при обнаружении повторного использования уже погашенного токена (защита от кражи токена)
- **Управление клиентами и тренерами** — CRUD-операции, привязка клиента к тренеру
- **Пагинация и фильтрация** — постраничная выдача списков с фильтрацией клиентов по тренеру
- **Валидация данных** — через FluentValidation, отдельно от бизнес-правил в сервисном слое
- **Кэширование** — in-memory кэш для часто запрашиваемых данных, с инвалидацией через версионирование ключей
- **Централизованная обработка ошибок** — единый middleware для перехвата исключений
- **Логирование** — структурированное логирование через `ILogger`
- **Юнит-тесты** — покрытие сервисов, валидаторов и репозиториев через xUnit + Moq

## Технологии

- **C# / ASP.NET Core 8** — веб-API
- **Entity Framework Core 8** — доступ к данным
- **PostgreSQL** — база данных (через провайдер Npgsql)
- **JWT (JSON Web Tokens)** — аутентификация, подпись HMACSHA256
- **BCrypt** — хэширование паролей
- **FluentValidation** — валидация входных данных
- **AutoMapper** — маппинг между сущностями и DTO
- **Swagger / Swashbuckle** — документация и тестирование API
- **xUnit + Moq** — юнит-тестирование
- **Docker** — контейнеризация (multi-stage build)

## Архитектура

Слоистая структура в пределах одного проекта:

```
Controllers → Services → Repositories → DbContext
```

- **Controllers** — обработка HTTP-запросов
- **Services** — бизнес-логика, включая Result-паттерн с типизированными ошибками (`None / NotFound / Conflict / ValidationError / ServerError`)
- **Repositories** — доступ к данным через EF Core
- **Domain** — модели, DTO, общие типы (`PagedResult`, `Result`)

## Запуск проекта

### Требования

- .NET 8 SDK
- PostgreSQL (локально или в контейнере)

### 1. Клонировать репозиторий

```bash
git clone https://github.com/dimonrama/GymManagment.git
cd GymManagment
```

### 2. Создать базу данных

Создайте в PostgreSQL пустую базу данных (например, через `psql`):

```sql
CREATE DATABASE gymmanagment;
```

### 3. Настроить строку подключения

Строка подключения без пароля уже указана в `appsettings.json`. Пароль хранится отдельно через .NET User Secrets и не попадает в репозиторий — добавьте свой локально:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=gymmanagment;Username=postgres;Password=ВАШ_ПАРОЛЬ" --project GymManagment
```

### 4. Применить миграции

```bash
dotnet ef database update --project GymManagment
```

### 5. Запустить проект

```bash
dotnet run --project GymManagment
```

После запуска Swagger UI доступен по адресу `/swagger` (в Development-окружении).

### Запуск в Docker

В корне решения есть готовый multi-stage `Dockerfile`:

```bash
docker build -t gymmanagment .
docker run -d -p 8080:8080 --name gymapp gymmanagment
```

Приложение внутри контейнера слушает порт `8080`.

## Статус проекта

Учебный pet-проект, разрабатывается в рамках самостоятельного изучения C#/ASP.NET Core backend-разработки. В процессе дальнейшего развития.
