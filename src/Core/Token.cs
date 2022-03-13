namespace Core;

public record Token(TokenType Type, string Lexeme, object? Literal, int Line);