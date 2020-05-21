using System.Threading.Tasks;

namespace Ignitor.Notifier
{
    internal interface IStateSignaling<TId, TEntity>
    {
        Task StateItemAddedAsync(TId id, IImmutable<TEntity> value);
        Task StateItemUpdatedAsync(TId id, IImmutable<TEntity> value);
        Task StateItemRemovedAsync(TId id, IImmutable<TEntity> value);
        Task StateChangedAsync(TId id, IImmutable<TEntity> value);
    }
}
