using System;

using DataFilm = FrameRecall.DataAccess.Models.Film;
using DataFilmRating = FrameRecall.DataAccess.Models.FilmRating;
using DomainFilm = FrameRecall.Services.Models.Film;
using DomainFilmRating = FrameRecall.Services.Models.FilmRating;

namespace FrameRecall.Services.Mapping;

/// <summary>
/// Методы расширения для преобразования сущностей между слоями DataAccess и Services.
/// Обеспечивает конвертацию моделей фильмов и их рейтингов.
/// </summary>
internal static class FilmMapper
{
    /// <summary>
    /// Преобразует рейтинг из доменной модели в модель данных.
    /// </summary>
    /// <param name="rating">Рейтинг доменной модели.</param>
    /// <returns>Рейтинг модели данных.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается если рейтинг не поддерживается.</exception>
    public static DataFilmRating ToDataAccess(this DomainFilmRating rating)
    {
        return rating switch
        {
            DomainFilmRating.Undefined => DataFilmRating.Undefined,
            DomainFilmRating.Good => DataFilmRating.Good,
            DomainFilmRating.Ok => DataFilmRating.Ok,
            DomainFilmRating.Bad => DataFilmRating.Bad,
            _ => throw new ArgumentOutOfRangeException(nameof(rating), rating, null)
        };
    }

    /// <summary>
    /// Преобразует фильм из доменной модели в модель данных.
    /// </summary>
    /// <param name="film">Фильм доменной модели.</param>
    /// <returns>Фильм модели данных.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается если film null.</exception>
    public static DataFilm ToDataAccess(this DomainFilm film)
    {
        ArgumentNullException.ThrowIfNull(film);

        return new DataFilm
        {
            Id = film.Id, Title = film.Title, Description = film.Description, Rating = film.Rating.ToDataAccess()
        };
    }

    /// <summary>
    /// Преобразует рейтинг из модели данных в доменную модель.
    /// </summary>
    /// <param name="rating">Рейтинг модели данных.</param>
    /// <returns>Рейтинг доменной модели.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается если рейтинг не поддерживается.</exception>
    public static DomainFilmRating ToDomain(this DataFilmRating rating)
    {
        return rating switch
        {
            DataFilmRating.Undefined => DomainFilmRating.Undefined,
            DataFilmRating.Good => DomainFilmRating.Good,
            DataFilmRating.Ok => DomainFilmRating.Ok,
            DataFilmRating.Bad => DomainFilmRating.Bad,
            _ => throw new ArgumentOutOfRangeException(nameof(rating), rating, null)
        };
    }

    /// <summary>
    /// Преобразует фильм из модели данных в доменную модель.
    /// </summary>
    /// <param name="film">Фильм модели данных.</param>
    /// <returns>Фильм доменной модели.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается если film null.</exception>
    public static DomainFilm ToDomain(this DataFilm film)
    {
        ArgumentNullException.ThrowIfNull(film);

        return new DomainFilm
        {
            Id = film.Id, Title = film.Title, Description = film.Description, Rating = film.Rating.ToDomain()
        };
    }
}