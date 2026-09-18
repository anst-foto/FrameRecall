using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FrameRecall.Services.Models;

namespace FrameRecall.Services;

/// <summary>
/// Определяет контракт для бизнес-логики работы с фильмами.
/// </summary>
public interface IFilmService
{
    /// <summary>
    /// Асинхронно получает все фильмы из хранилища.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Коллекция всех фильмов или пустая коллекция если фильмов нет.</returns>
    Task<IReadOnlyCollection<Film>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно получает фильм по его идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор фильма.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Фильм если найден, null если не найден.</returns>
    Task<Film?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно получает фильмы по названию (частичное совпадение, регистронезависимый поиск).
    /// </summary>
    /// <param name="title">Название фильма для поиска.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Коллекция найденных фильмов или пустая коллекция.</returns>
    Task<IReadOnlyCollection<Film>> GetByTitleAsync(string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно получает фильмы по рейтингу.
    /// </summary>
    /// <param name="rating">Рейтинг для фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Коллекция фильмов с указанным рейтингом или пустая коллекция.</returns>
    Task<IReadOnlyCollection<Film>> GetByRatingAsync(FilmRating rating, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно создаёт новый фильм в хранилище.
    /// </summary>
    /// <param name="film">Фильм для создания.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Созданный фильм если успешно, null если произошла ошибка.</returns>
    Task<Film?> CreateAsync(Film film, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно обновляет существующий фильм в хранилище.
    /// </summary>
    /// <param name="film">Фильм с обновлёнными данными.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Обновлённый фильм если успешно, null если фильм не найден или произошла ошибка.</returns>
    Task<Film?> UpdateAsync(Film film, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно удаляет фильм из хранилища по его идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор фильма для удаления.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Удалённый фильм если успешно, null если фильм не найден или произошла ошибка.</returns>
    Task<Film?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}