using Dapper;
using Microsoft.Data.Sqlite;
using Serilog;

namespace UnicornScriptsExecuter.Updaters;

public class Updater : IUpdater
{
    public async Task ExecuteAsync(string sqlFilesPath, string connectionString)
    {
        if (sqlFilesPath.EndsWith('/'))
            sqlFilesPath = sqlFilesPath[..^1];

        
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
                        var rows = await con.ExecuteAsync(sql, transaction);
                        Log.Information($"affected {rows} rows");
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