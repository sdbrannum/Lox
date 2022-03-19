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
    var exp = new BinaryExpr(
        new UnaryExpr(
            new Token(TokenType.MINUS, "-", null, 1),
            new LiteralExpr(123)),
        new Token(TokenType.STAR, "*", null, 1),
        new GroupingExpr(new LiteralExpr(45.67)));

    Console.Write(new AstPrinter().Print(exp));
}