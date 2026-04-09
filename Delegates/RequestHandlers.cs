using System.Diagnostics;

namespace MyShopApi.Delegates;

public static class RequestHandlers
{
    public static void LogToConsole(string endpoint, int status, long time)
    {
        Console.WriteLine($"{endpoint} -> {status} ({time} ms)");
    }

    public static void LogToFile(string endpoint, int status, long time)
    {
        File.AppendAllText("log.txt",
            $"{DateTime.Now}: {endpoint} -> {status} ({time} ms)\n");
    }
}