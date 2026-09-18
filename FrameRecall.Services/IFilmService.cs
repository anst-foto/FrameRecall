using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FrameRecall.Services.Models;

namespace FrameRecall.Services;

public interface IFilmService
{
    public Task<IReadOnlyCollection<Film>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<Film?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<IReadOnlyCollection<Film>> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
    public Task<IReadOnlyCollection<Film>> GetByRatingAsync(FilmRating rating, CancellationToken cancellationToken = default);
    
    public Task<Film?> CreateAsync(Film film, CancellationToken cancellationToken = default);
    public Task<Film?> UpdateAsync(Film film, CancellationToken cancellationToken = default);
    public Task<Film?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}