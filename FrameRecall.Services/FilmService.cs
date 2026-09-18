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

/// <summary>
/// Реализация бизнес-логики для работы с фильмами.
/// Преобразует данные между слоями DataAccess и Services, обеспечивает бизнес-правила.
/// </summary>
public class FilmService : IFilmService
{
    /// <summary>
    /// Репозиторий для доступа к данным фильмов.
    /// </summary>
    private readonly IRepository<DataFilm> _repository;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="FilmService"/>.
    /// </summary>
    /// <param name="repository">Репозиторий для доступа к фильмам.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается если repository null.</exception>
    public FilmService(IRepository<DataFilm> repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// Асинхронно получает все фильмы из хранилища и преобразует их в доменные модели.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Коллекция всех фильмов доменной модели.</returns>
    public async Task<IReadOnlyCollection<DomainFilm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<DataFilm> films = await _repository.GetAllAsync(cancellationToken);
        return films.Select(f => f.ToDomain()).ToList();
    }

    /// <summary>
    /// Асинхронно получает фильм по ID из хранилища и преобразует его в доменную модель.
    /// </summary>
    /// <param name="id">Уникальный идентификатор фильма.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Фильм доменной модели если найден, null если не найден.</returns>
    public async Task<DomainFilm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<DataFilm> films = await _repository.GetAllAsync(cancellationToken);
        return films.SingleOrDefault(f => f.Id == id)?.ToDomain();
    }

    /// <summary>
    /// Асинхронно получает фильмы по названию с частичным совпадением (регистронезависимый поиск).
    /// </summary>
    /// <param name="title">Название или его часть для поиска.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Коллекция найденных фильмов доменной модели.</returns>
    public async Task<IReadOnlyCollection<DomainFilm>> GetByTitleAsync(string title,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<DataFilm> films = await _repository.GetAllAsync(cancellationToken);
        return films.Where(f => f.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
            .Select(f => f.ToDomain())
            .ToList();
    }

    /// <summary>
    /// Асинхронно получает фильмы по рейтингу и преобразует их в доменные модели.
    /// </summary>
    /// <param name="rating">Рейтинг для фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Коллекция фильмов с указанным рейтингом доменной модели.</returns>
    public async Task<IReadOnlyCollection<DomainFilm>> GetByRatingAsync(DomainFilmRating rating,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<DataFilm> films = await _repository.GetAllAsync(cancellationToken);
        return films.Where(f => f.Rating == rating.ToDataAccess())
            .Select(f => f.ToDomain())
            .ToList();
    }

    /// <summary>
    /// Асинхронно создаёт новый фильм, преобразуя доменную модель в модель данных.
    /// </summary>
    /// <param name="film">Фильм доменной модели для создания.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Созданный фильм доменной модели если успешно, null если произошла ошибка.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается если film null.</exception>
    public async Task<DomainFilm?> CreateAsync(DomainFilm film, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(film);

        DataFilm? created = await _repository.CreateAsync(film.ToDataAccess(), cancellationToken);
        return created?.ToDomain();
    }

    /// <summary>
    /// Асинхронно обновляет существующий фильм, преобразуя доменную модель в модель данных.
    /// Предварительно проверяет существование фильма.
    /// </summary>
    /// <param name="film">Фильм доменной модели с обновлёнными данными.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Обновлённый фильм доменной модели если успешно, null если не найден или произошла ошибка.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается если film null.</exception>
    public async Task<DomainFilm?> UpdateAsync(DomainFilm film, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(film);

        DomainFilm? result = await GetByIdAsync(film.Id, cancellationToken);
        if (result is null)
        {
            return null;
        }

        DataFilm? updated = await _repository.UpdateAsync(film.ToDataAccess(), cancellationToken);
        return updated?.ToDomain();
    }

    /// <summary>
    /// Асинхронно удаляет фильм по его идентификатору.
    /// Предварительно проверяет существование фильма.
    /// </summary>
    /// <param name="id">Уникальный идентификатор фильма для удаления.</param>
    /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
    /// <returns>Удалённый фильм доменной модели если успешно, null если не найден или произошла ошибка.</returns>
    public async Task<DomainFilm?> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return null;
        }

        DomainFilm? filmToDelete = await GetByIdAsync(id, cancellationToken);
        if (filmToDelete is null)
        {
            return null;
        }

        bool result = await _repository.DeleteAsync(filmToDelete.ToDataAccess(), cancellationToken);
        return !result ? null : filmToDelete;
    }
}