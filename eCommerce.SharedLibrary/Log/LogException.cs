
using Serilog;

namespace eCommerce.SharedLibrary;

public static class LogException
{
    public static void LogExceptions(Exception ex)
    {

        LogToFile(ex.Message);
        LogToConsole(ex.Message);
        LogToDebugger(ex.Message);

    }
        private static void LogToFile(string Message) => Log.Information(Message);
    private static void LogToConsole(string Message) => Log.Warning(Message);
    private static void LogToDebugger(string Message) => Log.Debug(Message);



}

