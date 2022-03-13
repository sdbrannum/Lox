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
        var parser = new Parser(tokens);
        var expr = parser.Parse();

        if (_hadError) return;
        Console.WriteLine(new AstPrinter().Print(expr));
        //foreach (var token in tokens) Console.WriteLine(token);
    }
  
    public static void Error(int line, string message)
    {
        Report(line, null, message);
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Repl.Report(token.Line, $" at end", message);
        }
        else
        {
            Repl.Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }

    public static void Report(int line, string? where, string message)
    {
        Debug.WriteLine($"[line: {line}] Error {where}: {message}");
        _hadError = true;
    }
}