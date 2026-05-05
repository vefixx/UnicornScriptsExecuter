using Dapper;
using Microsoft.Data.Sqlite;
using Serilog;
using UnicornStatsUpdater.Models;

namespace UnicornStatsUpdater.Updaters;

public class HourlyUpdater : IUpdater
{
    public async Task ExecuteAsync(string sqlFilesPath, string connectionString)
    {
        if (sqlFilesPath.EndsWith('/'))
            sqlFilesPath = sqlFilesPath[..^1];
        
        
        // Получаем все .sql файлы
        var files = Directory.GetFiles(sqlFilesPath)
            .Where(f => f.Contains(".sql")).ToArray();
        
        Log.Information($".sql-скриптов в директории: {files.Length}");

        await using (var con = new SqliteConnection(connectionString))
        {
            await con.OpenAsync();
            var lastUpdateString = await con.QuerySingleOrDefaultAsync<string>("SELECT ts FROM m_stats_hourly_by_apartment ORDER BY ts DESC LIMIT 1");

            DateTime lastUpdate = lastUpdateString != null ? DateTime.Parse(lastUpdateString) : new DateTime(1900, 1, 1);
            Log.Information($"Последнее обновление таблицы: {lastUpdateString}");

            var parameters = new { lastHourlyUpdate = lastUpdate };
            
            foreach (var sqlFile in files)
            {
                Log.Information($"Выполнение скрипта \"{sqlFile}\"");
                var sql = await File.ReadAllTextAsync(sqlFile);
                await con.ExecuteAsync(sql, parameters);
            }
        }
        
        Log.Information($"Выполнение скриптов завершено.");
    }
}