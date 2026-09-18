using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FrameRecall.DataAccess;
using FrameRecall.Services.Mapping;

using DataFilm = FrameRecall.DataAccess.Models.Film;
using DomainFilm = FrameRecall.Services.Models.Film;
using DomainFilmRating = FrameRecall.Services.Models.FilmRating;

namespace FrameRecall.Services;

public class FilmService : IFilmService
{
    private readonly IRepository<DataFilm> _repository;

    public FilmService(IRepository<DataFilm> repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<DomainFilm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var films = await _repository.GetAllAsync(cancellationToken);
        return films.Select(f => f.ToDomain()).ToList();
    }

    public async Task<DomainFilm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var films = await _repository.GetAllAsync(cancellationToken);
        return films.SingleOrDefault(f => f.Id == id)?.ToDomain();
    }

    public async Task<IReadOnlyCollection<DomainFilm>> GetByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        var films = await _repository.GetAllAsync(cancellationToken);
        return films.Where(f => f.Title == title)
            .Select(f => f.ToDomain())
            .ToList();
    }

    public async Task<IReadOnlyCollection<DomainFilm>> GetByRatingAsync(DomainFilmRating rating, CancellationToken cancellationToken = default)
    {
        var films = await _repository.GetAllAsync(cancellationToken);
        return films.Where(f => f.Rating == rating.ToDataAccess())
            .Select(f => f.ToDomain())
            .ToList();
    }

    public async Task<DomainFilm?> CreateAsync(DomainFilm film, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(film);

        var created = await _repository.CreateAsync(film.ToDataAccess(), cancellationToken);
        return created?.ToDomain();
    }

    public async Task<DomainFilm?> UpdateAsync(DomainFilm film, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(film);
        
        var filmToUpdate = await GetByIdAsync(film.Id, cancellationToken);
        if (filmToUpdate is null) return null;

        var updated = await _repository.UpdateAsync(filmToUpdate.ToDataAccess(), cancellationToken);
        return updated?.ToDomain();
    }

    public async Task<DomainFilm?> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return null;
        
        var filmToDelete = await GetByIdAsync(id, cancellationToken);
        if (filmToDelete is null) return null;
        
        var result = await _repository.DeleteAsync(filmToDelete.ToDataAccess(), cancellationToken);
        return result ? null : filmToDelete;
    }
}