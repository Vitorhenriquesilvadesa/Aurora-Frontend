namespace AuroraCompiler.Core;

public record Token(int Line, int Column, TokenType Type, string Lexeme, object Literal);