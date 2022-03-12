using System.Diagnostics;
using System.Text;

namespace Core;

public static class Repl
{
    static bool _hadError = false;

    public static void RunFile(string path)
    {
        var bytes = File.ReadAllBytes(path);
        Run(Encoding.UTF8.GetString(bytes));

        if (_hadError)
        {
            Environment.Exit(65);
        }
    }
  
    public static void RunPrompt()
    {
        for (;;)
        {
            Console.Write(">");
            var line = Console.ReadLine();
            if (line == null)
            {
                break;
            }
  
            Run(line);
            _hadError = false;
        }
    }
  
    private static void Run(string source)
    {
        // scanner is the lexer
        var lexer = new Lexer(source);
        var tokens = lexer.ScanTokens();
        foreach (var token in tokens) Console.WriteLine(token);
    }
  
    public static void Error(int line, string message)
    {
        Report(line, null, message);
    }
  
    private static void Report(int line, string where, string message)
    {
        Debug.WriteLine($"[line: {line}] Error {where}: {message}");
        _hadError = true;
    }
}