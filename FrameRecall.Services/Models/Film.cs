using System;

namespace FrameRecall.Services.Models;

/// <summary>
/// Сущность фильма в системе FrameRecall.
/// </summary>
public class Film
{
    /// <summary>
    /// Уникальный идентификатор фильма. По умолчанию генерируется как UUID v7.
    /// </summary>
    public Guid Id { get; set; } = Guid.CreateVersion7();

    /// <summary>
    /// Название фильма.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Описание фильма. Может быть null, если описание не предоставлено.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Рейтинг фильма. По умолчанию — <see cref="FilmRating.Undefined"/>.
    /// Хранится в MongoDB как строка.
    /// </summary>
    public required FilmRating Rating { get; set; } = FilmRating.Undefined;
}