using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;


namespace R5T.T0128
{
    /// <summary>
    /// All sychronous operations, since the file set *should* (but doesn't) represent the in-memory set of entities.
    /// </summary>
    public class JsonFileSet<TEntity> : FileSet<TEntity>
        // Use a class constraint to ensure modifying the returned object reference will modify the object in the file set.
        where TEntity : class
    {
        private string JsonFilePath { get; }
        private List<TEntity> Entities { get; set; }


        public JsonFileSet(string jsonFilePath)
        {
            this.JsonFilePath = jsonFilePath;
        }

        /// <summary>
        /// Should be some-sort of asynchronous, but to allow synchronous methods (modeling the in-memory nature of the file set) and poor-man's change tracking (if anything is loaded, then something changed), this method needs to be synchronous.
        /// Ok, since this is really just development infrastructure.
        /// </summary>
        private void EnsureEntitiesIsLoaded()
        {
            var isEntitiesLoaded = this.IsEntitiesLoaded();
            if(!isEntitiesLoaded)
            {
                var entities = JsonFileHelper.LoadFromFileOrDefault<TEntity[]>(
                    this.JsonFilePath,
                    () => Array.Empty<TEntity>());

                this.Entities = new List<TEntity>(entities);
            }
        }

        private bool IsEntitiesLoaded()
        {
            var output = this.Entities is object;
            return output;
        }

        public override IEnumerator<TEntity> GetEnumerator()
        {
            this.EnsureEntitiesIsLoaded();

            return this.Entities.GetEnumerator();
        }

        public override TEntity Add(TEntity entity)
        {
            this.EnsureEntitiesIsLoaded();

            this.Entities.Add(entity);

            return entity;
        }

        public override void AddRange(IEnumerable<TEntity> entities)
        {
            this.EnsureEntitiesIsLoaded();

            this.Entities.AddRange(entities);
        }

        public override TEntity Remove(TEntity entity)
        {
            this.EnsureEntitiesIsLoaded();

            // Ignore return value.
            this.Entities.Remove(entity);

            return entity;
        }

        public override void RemoveRange(IEnumerable<TEntity> entities)
        {
            this.EnsureEntitiesIsLoaded();

            foreach (var entity in entities)
            {
                this.Entities.Remove(entity);
            }
        }

        /// <summary>
        /// The only asynchronous operation, since this should be taken care of asynchronously by the file context, but to allow poor-man's change tracking (if entities were loaded, then something changed) this save operation needs to be handled by the file set.
        /// </summary>
        public override Task Save()
        {
            // Poor man's change tracking: if anything has been loaded, then something has been changed.
            var isEntitiesLoaded = this.IsEntitiesLoaded();
            if(isEntitiesLoaded)
            {
                // Now save to file.
                JsonFileHelper.WriteToFile(this.JsonFilePath, this.Entities.ToArray());
            }
            // Else, no need to do anything.

            return Task.CompletedTask;
        }

        public override TEntity Update(TEntity entity)
        {
            // No need to ensure entities are loaded.
            // If the entity object exists, since the entity type has the class constraing, it will have been retrieved from the file set, meaning the entities of the file set will have to have already been loaded.

            // Determine whether the entity is found.
            var isFound = this.Entities.Contains(entity);
            if(isFound)
            {
                // Shockingly, do nothing. Since the entity type has a class contraint, the entity object is the same object as in the file set's entities list. This means it is already updated!
            }
            else
            {
                // If not found, update is just an add.
                this.Add(entity);
            }

            return entity;
        }

        public override void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.Update(entity);
            }
        }
    }
}
