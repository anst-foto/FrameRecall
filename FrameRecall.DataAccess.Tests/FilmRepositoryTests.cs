using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FrameRecall.DataAccess.Models;

using MongoDB.Driver;

using Xunit;

namespace FrameRecall.DataAccess.Tests;

/// <summary>
/// Интеграционные тесты репозитория фильмов на реальной MongoDB.
/// </summary>
[Collection(MongoDbCollection.Name)]
public class FilmRepositoryTests
{
    private readonly FilmRepository _repository;
    private readonly IMongoCollection<Film> _collection;

    /// <summary>
    /// Инициализирует тесты, подключаясь к базе данных из фикстуры.
    /// </summary>
    public FilmRepositoryTests(MongoDbFixture fixture)
    {
        IMongoDatabase database = fixture.Database;
        _repository = new FilmRepository(database, fixture.Logger);
        _collection = database.GetCollection<Film>("films");
    }

    private async Task ClearCollectionAsync()
    {
        await _collection.DeleteManyAsync(Builders<Film>.Filter.Empty);
    }

    [Fact]
    public async Task CreateAsync_ValidFilm_ReturnsCreatedFilm()
    {
        await ClearCollectionAsync();

        Film film = new()
        {
            Title = "The Matrix",
            Description = "A hacker learns about the true nature of reality",
            Rating = FilmRating.Good
        };

        Film? result = await _repository.CreateAsync(film);

        Assert.NotNull(result);
        Assert.Equal(film.Id, result!.Id);
        Assert.Equal(film.Title, result.Title);
        Assert.Equal(film.Description, result.Description);
        Assert.Equal(film.Rating, result.Rating);

        Film fromDb = await _collection
            .Find(f => f.Id == film.Id)
            .FirstOrDefaultAsync();

        Assert.NotNull(fromDb);
        Assert.Equal(film.Title, fromDb.Title);
    }

    [Fact]
    public async Task CreateAsync_EmptyTitle_ReturnsNull()
    {
        await ClearCollectionAsync();

        Film film = new() { Title = "   ", Rating = FilmRating.Good };

        Film? result = await _repository.CreateAsync(film);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_InvalidRating_ReturnsNull()
    {
        await ClearCollectionAsync();

        Film film = new() { Title = "Test", Rating = (FilmRating)999 };

        Film? result = await _repository.CreateAsync(film);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_EmptyId_GeneratesNewId()
    {
        await ClearCollectionAsync();

        Film film = new() { Title = "Test", Rating = FilmRating.Ok };
        film.Id = Guid.Empty;

        Film? result = await _repository.CreateAsync(film);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result!.Id);
    }

    [Fact]
    public async Task UpdateAsync_ExistingFilm_ReturnsUpdatedFilm()
    {
        await ClearCollectionAsync();

        Film film = new() { Title = "Original", Rating = FilmRating.Good };
        await _repository.CreateAsync(film);

        film.Title = "Updated";
        film.Rating = FilmRating.Bad;

        Film? result = await _repository.UpdateAsync(film);

        Assert.NotNull(result);
        Assert.Equal("Updated", result!.Title);
        Assert.Equal(FilmRating.Bad, result.Rating);

        Film fromDb = await _collection
            .Find(f => f.Id == film.Id)
            .FirstOrDefaultAsync();

        Assert.NotNull(fromDb);
        Assert.Equal("Updated", fromDb.Title);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingFilm_ReturnsNull()
    {
        await ClearCollectionAsync();

        Film film = new() { Id = Guid.NewGuid(), Title = "NonExisting", Rating = FilmRating.Ok };

        Film? result = await _repository.UpdateAsync(film);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ExistingFilm_ReturnsTrue()
    {
        await ClearCollectionAsync();

        Film film = new() { Title = "ToDelete", Rating = FilmRating.Bad };
        await _repository.CreateAsync(film);

        bool result = await _repository.DeleteAsync(film);

        Assert.True(result);

        Film? fromDb = await _collection
            .Find(f => f.Id == film.Id)
            .FirstOrDefaultAsync();

        Assert.Null(fromDb);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingFilm_ReturnsFalse()
    {
        await ClearCollectionAsync();

        Film film = new() { Id = Guid.NewGuid(), Title = "NonExisting", Rating = FilmRating.Ok };

        bool result = await _repository.DeleteAsync(film);

        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_EmptyCollection_ReturnsEmpty()
    {
        await ClearCollectionAsync();

        IReadOnlyCollection<Film> result = await _repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithFilms_ReturnsAllFilms()
    {
        await ClearCollectionAsync();

        Film film1 = new() { Title = "Film1", Rating = FilmRating.Good };
        Film film2 = new() { Title = "Film2", Rating = FilmRating.Ok };

        await _repository.CreateAsync(film1);
        await _repository.CreateAsync(film2);

        IReadOnlyCollection<Film> result = await _repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f.Title == "Film1");
        Assert.Contains(result, f => f.Title == "Film2");
    }
}