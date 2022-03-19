using System.Text;

namespace Core;

public static class Repl
{
    private static bool _hadError;
    private static bool _hadRuntimeError;

    public static void RunFile(string path)
    {
        var bytes = File.ReadAllBytes(path);
        Run(Encoding.UTF8.GetString(bytes));

        if (_hadError)
        {
            Environment.Exit(65);
        }

        if (_hadRuntimeError)
        {
            Environment.Exit(70);
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
        var statements = parser.Parse();

        if (_hadError)
        {
            return;
        }

        var interpreter = new Interpreter();
        interpreter.Interpret(statements);

        //Console.WriteLine(new AstPrinter().Print(statements));
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
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }

    public static void RuntimeError(RuntimeException ex)
    {
        Console.WriteLine(ex.Message);
        Console.WriteLine($"[line ${ex.token.Line}]");
        _hadRuntimeError = true;
    }

    public static void Report(int line, string? where, string message)
    {
        Console.WriteLine($"[line: {line}] Error {where}: {message}");
        _hadError = true;
    }
}