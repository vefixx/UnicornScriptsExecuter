using CommandLine;

namespace UnicornScriptsExecuter.Models;

public class Options
{
    [Option("type", Required = true, HelpText = "Тип обновления (hourly - часовая (обновится часовая витрина), daily - дневная (обновится дневная витрина), weekly - недельная (обновится недельная витрина))")]
    public string UpdateType { get; set; }
    
    [Option("with_file_logging", Required = false, HelpText = "Логировать ли работу программы в текстовый файл")]
    public bool WithFileLogging { get; set; }
} 