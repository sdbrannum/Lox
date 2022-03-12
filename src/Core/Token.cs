namespace Core;

public record Token(TokenType Type, string Lexeme, object Literal, int Line);
// {
//     public TokenType Type { get; init; }
//     public string Lexeme { get; init; }
//     public object Literal { get; init; }
//     public int Line { get; init;  }
//
//     public Token()
//     {
//         
//     }
// }