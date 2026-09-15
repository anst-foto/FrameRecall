using System;
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

        var film = new Film
        {
            Title = "The Matrix",
            Description = "A hacker learns about the true nature of reality",
            Rating = FilmRating.Good
        };

        var result = await _repository.CreateAsync(film);

        Assert.NotNull(result);
        Assert.Equal(film.Id, result!.Id);
        Assert.Equal(film.Title, result.Title);
        Assert.Equal(film.Description, result.Description);
        Assert.Equal(film.Rating, result.Rating);

        var fromDb = await _collection
            .Find(f => f.Id == film.Id)
            .FirstOrDefaultAsync();

        Assert.NotNull(fromDb);
        Assert.Equal(film.Title, fromDb.Title);
    }

    [Fact]
    public async Task CreateAsync_EmptyTitle_ReturnsNull()
    {
        await ClearCollectionAsync();

        var film = new Film
        {
            Title = "   ",
            Rating = FilmRating.Good
        };

        var result = await _repository.CreateAsync(film);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_InvalidRating_ReturnsNull()
    {
        await ClearCollectionAsync();

        var film = new Film
        {
            Title = "Test",
            Rating = (FilmRating)999
        };

        var result = await _repository.CreateAsync(film);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_EmptyId_GeneratesNewId()
    {
        await ClearCollectionAsync();

        var film = new Film
        {
            Title = "Test",
            Rating = FilmRating.Ok
        };
        film.Id = Guid.Empty;

        var result = await _repository.CreateAsync(film);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result!.Id);
    }

    [Fact]
    public async Task UpdateAsync_ExistingFilm_ReturnsUpdatedFilm()
    {
        await ClearCollectionAsync();

        var film = new Film
        {
            Title = "Original",
            Rating = FilmRating.Good
        };
        await _repository.CreateAsync(film);

        film.Title = "Updated";
        film.Rating = FilmRating.Bad;

        var result = await _repository.UpdateAsync(film);

        Assert.NotNull(result);
        Assert.Equal("Updated", result!.Title);
        Assert.Equal(FilmRating.Bad, result.Rating);

        var fromDb = await _collection
            .Find(f => f.Id == film.Id)
            .FirstOrDefaultAsync();

        Assert.NotNull(fromDb);
        Assert.Equal("Updated", fromDb.Title);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingFilm_ReturnsNull()
    {
        await ClearCollectionAsync();

        var film = new Film
        {
            Id = Guid.NewGuid(),
            Title = "NonExisting",
            Rating = FilmRating.Ok
        };

        var result = await _repository.UpdateAsync(film);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ExistingFilm_ReturnsTrue()
    {
        await ClearCollectionAsync();

        var film = new Film
        {
            Title = "ToDelete",
            Rating = FilmRating.Bad
        };
        await _repository.CreateAsync(film);

        var result = await _repository.DeleteAsync(film);

        Assert.True(result);

        var fromDb = await _collection
            .Find(f => f.Id == film.Id)
            .FirstOrDefaultAsync();

        Assert.Null(fromDb);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingFilm_ReturnsFalse()
    {
        await ClearCollectionAsync();

        var film = new Film
        {
            Id = Guid.NewGuid(),
            Title = "NonExisting",
            Rating = FilmRating.Ok
        };

        var result = await _repository.DeleteAsync(film);

        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_EmptyCollection_ReturnsEmpty()
    {
        await ClearCollectionAsync();

        var result = await _repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithFilms_ReturnsAllFilms()
    {
        await ClearCollectionAsync();

        var film1 = new Film { Title = "Film1", Rating = FilmRating.Good };
        var film2 = new Film { Title = "Film2", Rating = FilmRating.Ok };

        await _repository.CreateAsync(film1);
        await _repository.CreateAsync(film2);

        var result = await _repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f.Title == "Film1");
        Assert.Contains(result, f => f.Title == "Film2");
    }
}