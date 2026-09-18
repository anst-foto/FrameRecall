FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Создание непривилегированного пользователя для безопасности
RUN useradd -m -u 1001 appuser
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копирование файлов проекта
COPY ["FrameRecall.WebApi/FrameRecall.WebApi.csproj", "FrameRecall.WebApi/"]
COPY ["FrameRecall.Services/FrameRecall.Services.csproj", "FrameRecall.Services/"]
COPY ["FrameRecall.DataAccess/FrameRecall.DataAccess.csproj", "FrameRecall.DataAccess/"]
COPY ["Directory.Packages.props", "."]

# Восстановление зависимостей
RUN dotnet restore "FrameRecall.WebApi/FrameRecall.WebApi.csproj"

# Копирование исходного кода
COPY . .

# Сборка приложения
WORKDIR "/src/FrameRecall.WebApi"
RUN dotnet build "./FrameRecall.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публикация приложения
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FrameRecall.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Healthcheck для проверки доступности сервиса
HEALTHCHECK --interval=10s --timeout=5s --retries=5 --start-period=20s \
    CMD dotnet /app/FrameRecall.WebApi.dll --healthcheck || exit 1

ENTRYPOINT ["dotnet", "FrameRecall.WebApi.dll"]
