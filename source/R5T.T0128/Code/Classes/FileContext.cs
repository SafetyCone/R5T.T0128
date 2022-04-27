using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace R5T.T0128
{
    /// <summary>
    /// The file context is fundamentally asynchronous since it represents the remote data store (comparable to the Entity Framework DbContext).
    /// </summary>
    public abstract class FileContext
    {
        // No Configure() method, as configuration is performed by the IFileContextProvider<TFileContext> service.

        /// <summary>
        /// Gets all file sets so that they can be saved.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<FileSet> GetAllFileSets();

        public async Task Save()
        {
            var fileSets = this.GetAllFileSets();
            foreach (var fileSet in fileSets)
            {
                await fileSet.Save();
            }
        }
    }
}
