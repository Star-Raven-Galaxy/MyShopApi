# ----------------- Сборка -----------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем весь проект и собираем
COPY . ./
RUN dotnet publish -c Release -o out

# ----------------- РUNTIME -----------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Открываем порт 80 для API
EXPOSE 80

# Точка входа
ENTRYPOINT ["dotnet", "MyShopApi.dll"]