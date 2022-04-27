using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace R5T.T0128
{
    /// <summary>
    /// Base file set type without the generic type parameter for the entity.
    /// </summary>
    public abstract class FileSet
    {
        abstract public Task Save();
    }


    /// <summary>
    /// The file set is fundamentally synchronous since it represents the local data store (comparable to the Entity Framework DbSet&lt;T&gt;).
    /// </summary>
    public abstract class FileSet<TEntity> : FileSet, IEnumerable<TEntity>
        // Use a class constraint to ensure modifying the returned object reference will modify the object in the file set.
        where TEntity : class
    {
        abstract public IEnumerator<TEntity> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        abstract public TEntity Add(TEntity entity);
        abstract public void AddRange(IEnumerable<TEntity> entities);
        abstract public TEntity Remove(TEntity entity);
        abstract public void RemoveRange(IEnumerable<TEntity> entities);
        abstract public TEntity Update(TEntity entity);
        abstract public void UpdateRange(IEnumerable<TEntity> entities);
    }
}
