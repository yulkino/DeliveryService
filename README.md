# DeliveryService

Это приложение фильтрующее заказы для службы доставки, разработанное на ASP.NET Core и использующее PostgreSQL для хранения данных.

## Содержание

- [Описание проекта](#описание-проекта)
- [Технологии](#технологии)
- [Как запустить проект](#как-запустить-проект)
- [Логи](#логи)
- [Получение данных](#получение-данных)

## Описание проекта

Для фильтрации заказов воспользуйтесь эндпоинтом:
```
/orders/filter/{districtId}?firstDeliveryDateTime={deliveryTime}
```
При попытке передать несуществующий идентификатор района выведется ошибка, при попытке передать время, в периоде получаса от которого нет заказов, вернется пустая коллекция.

## Технологии

Проект использует следующие технологии:

- .NET 8.0
- ASP.NET Core
- PostgreSQL
- Entity Framework Core
- Docker

Unit-тестирование:
- xUnit
- Moq

Логирование:
- Serilog

Генерирование данных в БД:
- Bogus

## Как запустить проект

Выберите вариант, который вам покажется более удобным:

#### 1. Развернуть всё приложение в Docker

В проекте предусмотрен docker-compose.

#### 2. Запустить ASP.NET Core проект локально, PostgreSQL развернуть в Docker

Для простоты можно запустить через тот же docker-compose, выключить контейнер с Web API, и запустить само приложение локально, в appsettings предусмотрена строка подключения.

## Логи

В папке logs в проекте DeliveryService. 

## Получение данных

Заказы и районы можно получить по эндпоинтам:
```
GET
/orders

GET
/districts
```

Данные сгенерированы с помощью библиотеки Bogus, настройка происходит в ApplicationContext в методе SeedRandomData.

Количество районов: 3 

Количество заказов: 1000


Если вам крайне необходимо изменить кол-во данных, то поменяйте его в **Generate**, но вам придется убрать последнюю миграцию и добавить новую с измененными настройками.

```csharp
var districts = districtFaker.Generate(3);
var orders = orderFaker.Generate(1000);
```
