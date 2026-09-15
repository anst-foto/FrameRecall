using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using MongoDB.Driver;

using Testcontainers.MongoDb;

using Xunit;

namespace FrameRecall.DataAccess.Tests;

/// <summary>
/// Фикстура, поднимающая одноразовый контейнер MongoDB для интеграционных тестов.
/// </summary>
public class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container;

    /// <summary>
    /// База данных, к которой подключаются тесты.
    /// </summary>
    public IMongoDatabase Database { get; private set; } = null!;

    /// <summary>
    /// Логгер для репозитория фильмов.
    /// </summary>
    public ILogger<FilmRepository> Logger { get; private set; } = null!;

    /// <summary>
    /// Создаёт фикстуру с образом MongoDB, соответствующим docker-compose.
    /// </summary>
    public MongoDbFixture()
    {
        _container = new MongoDbBuilder()
            .WithImage("mongo:8.3")
            .Build();
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var client = new MongoClient(_container.GetConnectionString());
        Database = client.GetDatabase("frame_recall_test_db");
        Logger = NullLogger<FilmRepository>.Instance;
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}