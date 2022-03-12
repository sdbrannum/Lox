using Core;

Console.Write(">");

Console.ReadLine();
switch (args.Length)
{
    case > 1:
        Console.WriteLine("Usage: jlox [script]");
        break;
    case 1:
        Repl.RunFile(args[0]);
        break;
    default:
        Repl.RunPrompt();
        break;
}