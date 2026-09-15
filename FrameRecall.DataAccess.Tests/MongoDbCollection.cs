using Xunit;

namespace FrameRecall.DataAccess.Tests;

/// <summary>
/// Коллекция xUnit, разделяющая одну фикстуру MongoDB между всеми тестовыми классами.
/// </summary>
[CollectionDefinition(Name)]
public class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
    /// <summary>
    /// Имя коллекции, используемое в атрибуте <see cref="CollectionAttribute"/>.
    /// </summary>
    public const string Name = "MongoDb";
}