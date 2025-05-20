# Чат-додаток з аналізом тональності

Реал-тайм чат-додаток з використанням ASP.NET Core, React, Azure SignalR Service та аналізом тональності повідомлень через Azure Cognitive Services.

## Демо

Додаток доступний за посиланнями:
- Frontend: https://chat-app-frontend-eqhyf2b9exh0hqfj.westeurope-01.azurewebsites.net
- Backend: https://testtask-dpdxaghffpb5edb9.westeurope-01.azurewebsites.net

## Технології

### Backend
- ASP.NET Core 8.0
- Azure SignalR Service
- Azure Cognitive Services Text Analytics API
- Entity Framework Core

### Frontend
- React
- @microsoft/signalr
- CSS для стилізації

## Функціональність

- Обмін повідомленнями в реальному часі
- Аналіз тональності повідомлень (позитивна/негативна/нейтральна)
- Візуальне відображення тональності за допомогою емодзі та кольорів
- Збереження історії повідомлень

## Розробка

### Вимоги
- .NET 8 SDK
- Node.js 
- Azure акаунт

### Локальний запуск
1. Клонуйте репозиторій
2. Запустіть бекенд: `dotnet run --project ChatApp/ChatApp.API/ChatApp.API.csproj`
3. Запустіть фронтенд: `cd chat-app-client && npm install && npm start`

## Розгортання

Проект розгорнуто на Azure з використанням:
- Azure App Service для бекенду та фронтенду
- Azure SignalR Service для реал-тайм комунікацій
- Azure Cognitive Services для аналізу тональності
