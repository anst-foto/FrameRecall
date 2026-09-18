using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FrameRecall.Services.Models;

namespace FrameRecall.Services;

public interface IFilmService
{
    Task<IReadOnlyCollection<Film>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Film?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Film>> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Film>> GetByRatingAsync(FilmRating rating, CancellationToken cancellationToken = default);

    Task<Film?> CreateAsync(Film film, CancellationToken cancellationToken = default);
    Task<Film?> UpdateAsync(Film film, CancellationToken cancellationToken = default);
    Task<Film?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}