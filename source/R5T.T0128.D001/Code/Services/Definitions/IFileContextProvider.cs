using System;
using System.Threading.Tasks;

using R5T.T0064;


namespace R5T.T0128.D001
{
    [ServiceDefinitionMarker]
    public interface IFileContextProvider<TFileContext> : IServiceDefinition
        where TFileContext : FileContext
    {
        Task<TFileContext> GetFileContext();
    }
}
