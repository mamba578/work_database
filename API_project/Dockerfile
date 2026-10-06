# Этап 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["API_project.csproj", "./"]
RUN dotnet restore "./API_project.csproj"

# Копируем остальные файлы проекта
COPY . .
# Собираем релизную версию
RUN dotnet publish "API_project.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Этап 2: Выполнение
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API_project.dll"]