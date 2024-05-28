namespace Interfaces;

using Models;

public interface IConfigService
{
    Task<string> Get();
    Task<IEnumerable<Config>> GetConfigAsync();

}