using Serilog;
using UnicornScriptsExecuter.Models;
using UnicornScriptsExecuter.Updaters;

namespace UnicornScriptsExecuter;

public class UpdaterManager
{
    
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

        var updater = new Updater();
        
        await updater.ExecuteAsync(sqlFilesPath, appSettings.ConnectionString);
    }
}