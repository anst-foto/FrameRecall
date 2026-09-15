using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FrameRecall.DataAccess.Models;

/// <summary>
/// Сущность фильма в системе FrameRecall.
/// </summary>
public class Film
{
    /// <summary>
    /// Уникальный идентификатор фильма. По умолчанию генерируется как UUID v7.
    /// </summary>
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    /// <summary>
    /// Название фильма.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Описание фильма. Может быть null, если описание не предоставлено.
    /// </summary>
    [BsonIgnoreIfNull]
    public string? Description { get; set; }

    /// <summary>
    /// Рейтинг фильма. По умолчанию — <see cref="FilmRating.Undefined"/>.
    /// Хранится в MongoDB как строка.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public required FilmRating Rating { get; set; } = FilmRating.Undefined;
}