using System.Globalization;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Serilog;
using UnicornScriptsExecuter.Models;
using UnicornScriptsExecuter.Updaters;

namespace UnicornScriptsExecuter;


class Program
{
    private static AppSettings _appSettings = null!;
    private static Options _options = null!;

    static async Task Main(string[] args)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        _options = Parser.Default.ParseArguments<Options>(args).Value;

        if (_options == null)
        {
            return;
        }

        var initSuccess = Init();

        if (!initSuccess)
            return;
        
        
        // Запуск обновления
        
        var updaterManager = new UpdaterManager();
        
        Log.Information($"Запуск менеджера обновления..");
        await updaterManager.ExecuteUpdater(_options.UpdateType, _appSettings);
        
        Log.Information($"Завершение программы");
    }
    
    /// <summary>
    /// Инициализация логгера
    /// </summary>
    private static void InitLogger()
    {
        var loggerBuilder = new LoggerConfiguration()
            .WriteTo.Console();

        if (_options.WithFileLogging)
        {
            var currentDateString = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            loggerBuilder.WriteTo.File($"updater_{_options.UpdateType}_{currentDateString}");
        }

        Log.Logger = loggerBuilder.CreateLogger();
    }
    
    /// <summary>
    /// Инициализация конфигурационного файла appsettings.json
    /// </summary>
    /// <returns></returns>
    private static bool InitAndCheckConfiguration()
    {
        Log.Information($"Загрузка конфигурации \"appsettings.json\"..");

        if (!File.Exists("appsettings.json"))
        {
            Log.Error($"Файл конфигурации \"appsettings.json\" не найден. Пожалуйста, создайте и заполните файл по шаблону.");
            return false;
        }
        

        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");
        var configRoot = builder.Build();

        var appSettingsSection = configRoot.GetSection("AppSettings");
        if (!appSettingsSection.Exists())
        {
            Log.Error($"Не найдена секция \"AppSettings\" в конфигурации. Пожалуйста, заполните файл по шаблону.");
            return false;
        }
        
        _appSettings = configRoot.GetSection("AppSettings").Get<AppSettings>()!;
        Log.Information("Файл конфигурации загружен");

        return true;
    }

    private static bool Init()
    {
        InitLogger();
        var configSuccess = InitAndCheckConfiguration();

        return configSuccess;
    }
}