namespace UnicornStatsUpdater.Models;

public class AppSettings
{
    /// <summary>
    /// Содержит пути до файлов SQL-запросов, где ключ - код метрики, а значение - путь до директории с файлами .sql
    /// </summary>
    public required Dictionary<string, string> QueriesPaths { get; set; }
    
    public required string ConnectionString { get; set; }
}