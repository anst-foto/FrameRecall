using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FrameRecall.DataAccess;

/// <summary>
/// Контракт универсального репозитория для операций доступа к данным.
/// </summary>
/// <typeparam name="T">Тип управляемой сущности.</typeparam>
public interface IRepository<T>
{
    /// <summary>
    /// Создаёт новую сущность в хранилище.
    /// </summary>
    /// <param name="entity">Сущность для создания.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Созданную сущность или null, если сущность не прошла валидацию.</returns>
    /// <exception cref="MongoDB.Driver.MongoException">Пробрасывается при ошибке записи в MongoDB.</exception>
    Task<T?> CreateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую сущность в хранилище.
    /// </summary>
    /// <param name="entity">Сущность с новыми значениями.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Обновлённую сущность или null, если сущность не найдена или невалидна.</returns>
    Task<T?> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет сущность из хранилища.
    /// </summary>
    /// <param name="entity">Сущность для удаления.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>true, если удаление прошло успешно; иначе false.</returns>
    Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает все сущности типа <typeparamref name="T"/> из хранилища.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция всех сущностей. Пустая коллекция, если сущностей нет.</returns>
    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default);
}