namespace Interfaces;

using Models;

public interface IConfigService
{
    Task<IEnumerable<Config>> GetConfigAsync();

}