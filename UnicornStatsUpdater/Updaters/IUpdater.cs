using UnicornStatsUpdater.Models;

namespace UnicornStatsUpdater.Updaters;

public interface IUpdater
{
    /// <summary>
    /// Запускает все скрипты из соответствующей директории
    /// </summary>
    /// <returns></returns>
    Task ExecuteAsync(string sqlFilesPath, string connectionString);
}