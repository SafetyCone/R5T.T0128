using System;
using System.Threading.Tasks;

using R5T.T0128.D001;


namespace R5T.T0128
{
    // Abstract class, so no need for any service implementation markings.
    public abstract class FileContextBasedRepositoryBase<TFileContext>
        where TFileContext : FileContext
    {
        protected IFileContextProvider<TFileContext> FileContextProvider { get; }


        public FileContextBasedRepositoryBase(IFileContextProvider<TFileContext> fileContextProvider)
        {
            this.FileContextProvider = fileContextProvider;
        }

        protected async Task ExecuteInContext(Func<TFileContext, Task> action)
        {
            var fileContext = await this.FileContextProvider.GetFileContext();

            await action(fileContext);
        }

        protected async Task<TOutput> ExecuteInContext<TOutput>(Func<TFileContext, Task<TOutput>> function)
        {
            var fileContext = await this.FileContextProvider.GetFileContext();

            var output = await function(fileContext);
            return output;
        }
    }
}
