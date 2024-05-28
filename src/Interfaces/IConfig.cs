namespace Interfaces;

using Models;

public interface IConfigService
{
    IEnumerable<Config> GetConfig();

}