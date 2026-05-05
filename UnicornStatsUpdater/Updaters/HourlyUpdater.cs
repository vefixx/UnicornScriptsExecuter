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

            await using (var transaction = await con.BeginTransactionAsync())
            {
                try
                {
                    foreach (var sqlFile in files)
                    {
                        Log.Information($"Выполнение скрипта \"{sqlFile}\"");
                        var sql = await File.ReadAllTextAsync(sqlFile);
                        await con.ExecuteAsync(sql);
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    Log.Error($"Ошибка при выполнении одного из скрипта: {e.Message}");
                    await transaction.RollbackAsync();
                }
            }
        }
        
        Log.Information($"Выполнение скриптов завершено.");
    }
}