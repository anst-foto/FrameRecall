using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FrameRecall.DataAccess.Models;

using Microsoft.Extensions.Logging;

using MongoDB.Driver;

namespace FrameRecall.DataAccess;

/// <summary>
/// Репозиторий для управления фильмами в MongoDB.
/// </summary>
public class FilmRepository : IRepository<Film>
{
    private const string CollectionName = "films";

    private readonly IMongoCollection<Film> _collection;
    private readonly ILogger<FilmRepository> _logger;

    /// <summary>
    /// Инициализирует репозиторий фильмов.
    /// </summary>
    /// <param name="database">База данных MongoDB.</param>
    /// <param name="logger">Логгер.</param>
    public FilmRepository(IMongoDatabase database, ILogger<FilmRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(database);
        ArgumentNullException.ThrowIfNull(logger);

        _collection = database.GetCollection<Film>(CollectionName);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Film?> CreateAsync(Film entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
        {
            _logger.LogWarning("CreateAsync вызван с null-сущностью");
            return null;
        }

        if (!TryValidate(entity))
        {
            return null;
        }

        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.CreateVersion7();
        }

        try
        {
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }
        catch (MongoException ex)
        {
            _logger.LogError(ex, "Ошибка MongoDB при создании фильма {FilmId}", entity.Id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Film?> UpdateAsync(Film entity, CancellationToken cancellationToken = default)
    {
        if (entity is null || entity.Id == Guid.Empty)
        {
            _logger.LogWarning("UpdateAsync вызван с null-сущностью или пустым Id");
            return null;
        }

        if (!TryValidate(entity))
        {
            return null;
        }

        FilterDefinition<Film> filter = Builders<Film>.Filter.Eq(f => f.Id, entity.Id);

        FindOneAndReplaceOptions<Film, Film> options = new() { ReturnDocument = ReturnDocument.After };

        try
        {
            return await _collection.FindOneAndReplaceAsync(
                filter,
                entity,
                options,
                cancellationToken);
        }
        catch (MongoException ex)
        {
            _logger.LogError(ex, "Ошибка MongoDB при обновлении фильма {FilmId}", entity.Id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Film entity, CancellationToken cancellationToken = default)
    {
        if (entity is null || entity.Id == Guid.Empty)
        {
            _logger.LogWarning("DeleteAsync вызван с null-сущностью или пустым Id");
            return false;
        }

        FilterDefinition<Film> filter = Builders<Film>.Filter.Eq(f => f.Id, entity.Id);

        try
        {
            Film? deleted = await _collection.FindOneAndDeleteAsync(
                filter,
                cancellationToken: cancellationToken);

            return deleted is not null;
        }
        catch (MongoException ex)
        {
            _logger.LogError(ex, "Ошибка MongoDB при удалении фильма {FilmId}", entity.Id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Film>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IAsyncCursor<Film> cursor = await _collection.FindAsync(
            Builders<Film>.Filter.Empty,
            cancellationToken: cancellationToken);

        return await cursor.ToListAsync(cancellationToken);
    }

    private bool TryValidate(Film entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Title))
        {
            _logger.LogWarning("Фильм {FilmId} не прошёл валидацию: пустое название", entity.Id);
            return false;
        }

        if (!Enum.IsDefined(typeof(FilmRating), entity.Rating))
        {
            _logger.LogWarning(
                "Фильм {FilmId} не прошёл валидацию: неизвестный рейтинг {Rating}",
                entity.Id,
                entity.Rating);
            return false;
        }

        return true;
    }
}