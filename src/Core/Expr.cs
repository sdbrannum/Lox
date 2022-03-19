using System;

namespace Core;



public abstract record Expr
{
    public abstract T Accept<T>(Visitor<T> visitor);

    public interface Visitor<T>
    {
        T VisitBinaryExpr(BinaryExpr expr);
        T VisitGroupingExpr(GroupingExpr expr);
        T VisitLiteralExpr(LiteralExpr expr);
        T VisitUnaryExpr(UnaryExpr expr);

        T VisitVariableExpr(VariableExpr expr);
        T VisitAssignExpr(AssignExpr expr);
    }
}

public record BinaryExpr : Expr
{
    public Expr Left { get; init; } = default!;
    public Token Op { get; init; } = default!;
    public Expr Right { get; init; } = default!;

    public BinaryExpr(Expr left, Token op, Expr right)
    {
        this.Left = left;
        this.Op = op;
        this.Right = right;
    }
    
    public override T Accept<T>(Visitor<T> visitor)
    {
        return visitor.VisitBinaryExpr(this);
    }
}

public record GroupingExpr : Expr
{
    public Expr Expression { get; init; } = default!;

    public GroupingExpr(Expr expression)
    {
        this.Expression = expression;
    }
    public override T Accept<T>(Visitor<T> visitor)
    {
        return visitor.VisitGroupingExpr(this);
    }
}

public record LiteralExpr : Expr
{
    public object? Value { get; set; } = default!;
    public LiteralExpr(object? val)
    {
        this.Value = val;
    }
    public override T Accept<T>(Visitor<T> visitor)
    {
        return visitor.VisitLiteralExpr(this);
    }
}

public record UnaryExpr : Expr
{
    public Token Op { get; init; } = default!;
    public Expr Right { get; init; } = default!;

    public UnaryExpr(Token op, Expr right)
    {
        this.Op = op;
        this.Right = right;
    }
    public override T Accept<T>(Visitor<T> visitor)
    {
        return visitor.VisitUnaryExpr(this);
    }
}

public record VariableExpr : Expr 
{
    public Token Name { get; init; } = default!;
    public VariableExpr(Token name)
    {
        this.Name = name;
    }

    public override T Accept<T>(Visitor<T> visitor)
    {
        return visitor.VisitVariableExpr(this);
    }
}


public record AssignExpr : Expr
{
    public Token Name { get; init; } = default!;
    public Expr Value { get; init; } = default!;

    public AssignExpr(Token name, Expr val)
    {
        this.Name = name;
        this.Value = val;
    }

    public override T Accept<T>(Visitor<T> visitor)
    {
        return visitor.VisitAssignExpr(this);
    }
}

