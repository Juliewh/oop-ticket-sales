namespace Shared.Storage;

public class InMemoryStore<TEntity>
    where TEntity : class, IEntity
{
    private readonly Dictionary<long, TEntity> _entities;

    private long _lastId;

    public InMemoryStore()
    {
        _entities = new Dictionary<long, TEntity>();
        _lastId = 0;
    }

    public long NextId()
        => ++_lastId;

    public TEntity? Find(long id)
        => _entities.TryGetValue(id, out var entity) ? entity : null;

    public IReadOnlyCollection<TEntity> GetAll()
        => _entities.Values.ToList();

    public TEntity Add(TEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        _entities.Add(entity.Id, entity);

        return entity;
    }

    public bool TryRemove(long id)
        => _entities.Remove(id);
}
