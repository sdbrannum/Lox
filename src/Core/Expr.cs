using System;

namespace Core;

public interface Visitor<T>
{
    T VisitBinary(Binary expr);
    T VisitGrouping(Grouping expr);
    T VisitLiteral(Literal expr);
    T VisitUnary(Unary expr);
}

public abstract record Expr()
{
    public abstract T Accept<T>(Visitor<T> visitor);
}

public record Binary : Expr
{
    public Expr Left { get; init; } = default!;
    public Token Op { get; init; } = default!;
    public Expr Right { get; init; } = default!;

    public Binary(Expr left, Token op, Expr right)
    {
        this.Left = left;
        this.Op = op;
        this.Right = right;
    }
    
    public override T Accept<T>(Visitor<T> visitor)
    {
        return visitor.VisitBinary(this);
    }
}

public record Grouping : Expr
{
    public Expr Expression { get; init; } = default!;

    public Grouping(Expr expression)
    {
        this.Expression = expression;
    }
    public override T Accept<T>(Visitor<T> visitor)
    {
        return visitor.VisitGrouping(this);
    }
}

public record Literal : Expr
{
    public object? Value { get; set; } = default!;
    public Literal(object? val)
    {
        this.Value = val;
    }
    public override T Accept<T>(Visitor<T> visitor)
    {
        return visitor.VisitLiteral(this);
    }
}

public record Unary : Expr
{
    public Token Op { get; init; } = default!;
    public Expr Right { get; init; } = default!;

    public Unary(Token op, Expr right)
    {
        this.Op = op;
        this.Right = right;
    }
    public override T Accept<T>(Visitor<T> visitor)
    {
        return visitor.VisitUnary(this);
    }
}

