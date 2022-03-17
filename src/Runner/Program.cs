using Core;

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



void TestPrinter()
{
    var exp = new Binary(
        new Unary(
            new Token(TokenType.MINUS, "-", null, 1),
            new Literal(123)),
        new Token(TokenType.STAR, "*", null, 1),
        new Grouping(new Literal(45.67)));

    Console.Write(new AstPrinter().Print(exp));
}