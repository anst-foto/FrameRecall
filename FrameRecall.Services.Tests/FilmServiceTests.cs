using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FrameRecall.DataAccess;

using MongoDB.Driver;

using Xunit;

using DomainFilm = FrameRecall.Services.Models.Film;
using DomainFilmRating = FrameRecall.Services.Models.FilmRating;
using DataFilm = FrameRecall.DataAccess.Models.Film;

namespace FrameRecall.Services.Tests;

[Collection(MongoDbCollection.Name)]
public class FilmServiceTests
{
    private readonly FilmService _service;
    private readonly IMongoCollection<DataFilm> _collection;

    public FilmServiceTests(MongoDbFixture fixture)
    {
        FilmRepository repository = new(fixture.Database, fixture.RepositoryLogger);
        _service = new FilmService(repository);
        _collection = fixture.Database.GetCollection<DataFilm>("films");
    }

    private async Task ClearCollectionAsync()
    {
        await _collection.DeleteManyAsync(Builders<DataFilm>.Filter.Empty);
    }

    [Fact]
    public async Task CreateAsync_ValidFilm_ReturnsDomainFilm()
    {
        await ClearCollectionAsync();

        DomainFilm film = new()
        {
            Title = "The Matrix", Description = "A hacker learns about reality", Rating = DomainFilmRating.Good
        };

        DomainFilm? result = await _service.CreateAsync(film);

        Assert.NotNull(result);
        Assert.Equal(film.Id, result!.Id);
        Assert.Equal(film.Title, result.Title);
        Assert.Equal(film.Description, result.Description);
        Assert.Equal(film.Rating, result.Rating);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingFilm_ReturnsFilm()
    {
        await ClearCollectionAsync();

        DomainFilm film = new() { Title = "Test", Rating = DomainFilmRating.Good };
        await _service.CreateAsync(film);

        DomainFilm? result = await _service.GetByIdAsync(film.Id);

        Assert.NotNull(result);
        Assert.Equal(film.Id, result!.Id);
        Assert.Equal(film.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_EmptyId_ReturnsNull()
    {
        await ClearCollectionAsync();

        DomainFilm? result = await _service.GetByIdAsync(Guid.Empty);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        await ClearCollectionAsync();

        DomainFilm? result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ExistingFilm_ReturnsUpdatedFilm()
    {
        await ClearCollectionAsync();

        DomainFilm film = new() { Title = "Original", Rating = DomainFilmRating.Good };
        await _service.CreateAsync(film);

        film.Title = "Updated";
        film.Rating = DomainFilmRating.Bad;
        DomainFilm? result = await _service.UpdateAsync(film);

        Assert.NotNull(result);
        Assert.Multiple(() => Assert.Equal(film.Title, result!.Title),
            () => Assert.Equal(film.Rating, result.Rating));
    }

    [Fact]
    public async Task UpdateAsync_NonExistingFilm_ReturnsNull()
    {
        await ClearCollectionAsync();

        DomainFilm film = new() { Id = Guid.NewGuid(), Title = "NonExisting", Rating = DomainFilmRating.Ok };

        DomainFilm? result = await _service.UpdateAsync(film);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ExistingFilm_ReturnsTrue()
    {
        await ClearCollectionAsync();

        DomainFilm film = new() { Title = "ToDelete", Rating = DomainFilmRating.Bad };
        await _service.CreateAsync(film);

        DomainFilm? result = await _service.DeleteAsync(film.Id);

        Assert.NotNull(result);

        DomainFilm? fromDb = await _service.GetByIdAsync(film.Id);
        Assert.Null(fromDb);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingFilm_ReturnsFalse()
    {
        await ClearCollectionAsync();

        DomainFilm? result = await _service.DeleteAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTitleAsync_MatchingQuery_ReturnsFilteredFilms()
    {
        const string TITLE = "matrix";

        await ClearCollectionAsync();

        await _service.CreateAsync(new DomainFilm { Title = "The Matrix", Rating = DomainFilmRating.Good });
        await _service.CreateAsync(new DomainFilm { Title = "Matrix Reloaded", Rating = DomainFilmRating.Ok });
        await _service.CreateAsync(new DomainFilm { Title = "Inception", Rating = DomainFilmRating.Good });

        IReadOnlyCollection<DomainFilm> result = await _service.GetByTitleAsync(TITLE);

        Assert.Equal(2, result.Count);
        Assert.All(result, f => Assert.Contains(TITLE, f.Title, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetByTitleAsync_EmptyQuery_ReturnsEmpty()
    {
        await ClearCollectionAsync();

        IReadOnlyCollection<DomainFilm> result = await _service.GetByTitleAsync("   ");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByRatingAsync_MatchingRating_ReturnsFilteredFilms()
    {
        const DomainFilmRating RATING = DomainFilmRating.Good;

        await ClearCollectionAsync();

        await _service.CreateAsync(new DomainFilm { Title = "Film1", Rating = DomainFilmRating.Good });
        await _service.CreateAsync(new DomainFilm { Title = "Film2", Rating = DomainFilmRating.Bad });
        await _service.CreateAsync(new DomainFilm { Title = "Film3", Rating = DomainFilmRating.Good });

        IReadOnlyCollection<DomainFilm> result = await _service.GetByRatingAsync(RATING);

        Assert.Equal(2, result.Count);
        Assert.All(result, f => Assert.Equal(RATING, f.Rating));
    }

    [Fact]
    public async Task GetByRatingAsync_NoMatches_ReturnsEmpty()
    {
        await ClearCollectionAsync();

        await _service.CreateAsync(new DomainFilm { Title = "Film1", Rating = DomainFilmRating.Good });

        IReadOnlyCollection<DomainFilm> result = await _service.GetByRatingAsync(DomainFilmRating.Bad);

        Assert.Empty(result);
    }
}