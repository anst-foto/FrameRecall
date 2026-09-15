namespace FrameRecall.DataAccess.Models;

/// <summary>
/// Возможные рейтинги фильма.
/// </summary>
public enum FilmRating
{
    /// <summary>Рейтинг не определён.</summary>
    Undefined,

    /// <summary>Хороший фильм.</summary>
    Good,

    /// <summary>Средний фильм.</summary>
    Ok,

    /// <summary>Плохой фильм.</summary>
    Bad
}