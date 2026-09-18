using System.Threading.Tasks;

using FrameRecall.DataAccess;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using MongoDB.Driver;

using Testcontainers.MongoDb;

using Xunit;

namespace FrameRecall.Services.Tests;

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

    public ILogger<FilmRepository> RepositoryLogger { get; private set; } = null!;


    /// <summary>
    /// Создаёт фикстуру с образом MongoDB, соответствующим docker-compose.
    /// </summary>
    public MongoDbFixture()
    {
        _container = new MongoDbBuilder("mongo:8.3")
            .Build();
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        MongoClient client = new(_container.GetConnectionString());
        Database = client.GetDatabase("frame_recall_test_db");

        RepositoryLogger = NullLogger<FilmRepository>.Instance;
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}