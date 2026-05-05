using Serilog;
using UnicornStatsUpdater.Models;
using UnicornStatsUpdater.Updaters;

namespace UnicornStatsUpdater;

public class UpdaterManager
{
    private static readonly Dictionary<string, IUpdater> UpdatersMap = new()
    {
        { "hourly", new HourlyUpdater() }
    };

    public async Task ExecuteUpdater(string periodType, AppSettings appSettings)
    {
        // Проверяем, указан ли путь в конфигурации
        string sqlFilesPath;
        if (!appSettings.QueriesPaths.TryGetValue(periodType, out sqlFilesPath))
        {
            Log.Error($"В конфигурации не указан путь до SQL-запросов для period_type=\"{periodType}\"");
            return;
        }
         
        // Проверяем, существует ли директория и есть ли в ней файлы .sql
        if (!Directory.Exists(sqlFilesPath) || Directory.GetFiles(sqlFilesPath, "*.sql").Length == 0)
        {
            Log.Error($"Директория \"{sqlFilesPath}\" пустая или не существует");
            return;
        }
        
        await UpdatersMap[periodType].ExecuteAsync(sqlFilesPath, appSettings.ConnectionString);
    }
    
    /// <summary>
    /// Существует ли в программе обработчик для указанного периода
    /// </summary>
    /// <param name="periodType"></param>
    /// <returns></returns>
    public static bool UpdaterExists(string periodType) => UpdatersMap.ContainsKey(periodType);
}