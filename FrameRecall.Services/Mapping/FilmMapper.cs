using System;

using DataFilm = FrameRecall.DataAccess.Models.Film;
using DataFilmRating = FrameRecall.DataAccess.Models.FilmRating;
using DomainFilm = FrameRecall.Services.Models.Film;
using DomainFilmRating = FrameRecall.Services.Models.FilmRating;

namespace FrameRecall.Services.Mapping;

internal static class FilmMapper
{
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

    public static DataFilm ToDataAccess(this DomainFilm film)
    {
        ArgumentNullException.ThrowIfNull(film);

        return new DataFilm
        {
            Id = film.Id, Title = film.Title, Description = film.Description, Rating = film.Rating.ToDataAccess()
        };
    }

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

    public static DomainFilm ToDomain(this DataFilm film)
    {
        ArgumentNullException.ThrowIfNull(film);

        return new DomainFilm
        {
            Id = film.Id, Title = film.Title, Description = film.Description, Rating = film.Rating.ToDomain()
        };
    }
}