namespace Core;

public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
{
    private LoxEnvironment _loxEnvironment = new LoxEnvironment();
    public object VisitBinaryExpr(BinaryExpr expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case TokenType.MINUS:
                GuardNumberOperator(expr.Op, right);
                return (double) left - (double) right;
            case TokenType.PLUS:
                if (left is double lDouble && right is double rDouble)
                {
                    return lDouble + rDouble;
                }

                if (left is string sLeft && right is string sRight)
                {
                    return sLeft + sRight;
                }

                throw new RuntimeException(expr.Op, "Operands must be two numbers or two strings");
                break;
            case TokenType.SLASH:
                GuardNumberOperator(expr.Op, left, right);
                return (double) left / (double) right;
            case TokenType.STAR:
                GuardNumberOperator(expr.Op, left, right);
                return (double) left * (double) right;
            case TokenType.GREATER:
                GuardNumberOperator(expr.Op, left, right);
                return (double) left > (double) right;
            case TokenType.GREATER_EQUAL:
                GuardNumberOperator(expr.Op, left, right);
                return (double) left >= (double) right;
            case TokenType.LESS:
                GuardNumberOperator(expr.Op, left, right);
                return (double) left < (double) right;
            case TokenType.LESS_EQUAL:
                GuardNumberOperator(expr.Op, left, right);
                return (double) left <= (double) right;
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
        }

        return null;
    }

    public object VisitGroupingExpr(GroupingExpr expr)
    {
        return Evaluate(expr.Expression);
    }

    public object VisitLiteralExpr(LiteralExpr expr)
    {
        return expr.Value;
    }

    public object VisitUnaryExpr(UnaryExpr expr)
    {
        var right = Evaluate(expr.Right);
        switch (expr.Op.Type)
        {
            case TokenType.MINUS:
                return -(double) right;
            case TokenType.BANG:
                return !IsTruthy(right);
        }

        return null;
    }

    public object VisitExpressionStmt(ExpressionStmt stmt)
    {
        Evaluate(stmt.Expression);
        return null;
    }

    public object VisitIfStmt(IfStmt stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.ThenBranch);
        } else if (stmt.ElseBranch != null)
        {
          Execute(stmt.ElseBranch);  
        } 
        return null;
    }

    public object VisitPrintStmt(PrintStmt stmt)
    {
        var val = Evaluate(stmt.Expression);
        Console.WriteLine(Stringify(val));
        return null;
    }

    public object VisitVarStmt(VarStmt stmt)
    {
        object val = null;

        if (stmt.Initializer != null)
        {
            val = Evaluate(stmt.Initializer);
        }
        _loxEnvironment.Define(stmt.Name.Lexeme, val);
        return null;
    }

    public object VisitBlockStmt(BlockStmt stmt)
    {
        ExecuteBlock(stmt.Statements, new LoxEnvironment(_loxEnvironment));
        return null;
    }

    public void ExecuteBlock(IEnumerable<Stmt> statements, LoxEnvironment env)
    {
        var previousEnv = this._loxEnvironment;
        try
        {
            // set the env for the current block
            this._loxEnvironment = env;
            foreach (var stmt in statements)
            {
                Execute(stmt);
            }
        }
        finally
        {
            // set the env back
            this._loxEnvironment = previousEnv;
        }
    }
    

    public object VisitVariableExpr(VariableExpr expr)
    {
        return _loxEnvironment.Get(expr.Name);
    }

    public object VisitAssignExpr(AssignExpr expr)
    {
        object val = Evaluate(expr.Value);
        _loxEnvironment.Assign(expr.Name, val);
        return val;
    }

    public object VisitLogicalExpr(LogicalExpr expr)
    {
        object left = Evaluate(expr.Left);
        if (expr.Op.Type == TokenType.OR)
        {
            if (IsTruthy(left)) return left;
        }
        else
        {
            if (!IsTruthy(left)) return left;
        }
        return Evaluate(expr.Right);
    }

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var stmt in statements) Execute(stmt);
        }
        catch (RuntimeException ex)
        {
            Repl.RuntimeError(ex);
        }
    }

    private object Evaluate(Expr expression)
    {
        return expression.Accept(this);
    }

    private bool IsTruthy(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is bool booled)
        {
            return booled;
        }

        return true;
    }

    private bool IsEqual(object a, object b)
    {
        if (a == null && b == null)
        {
            return true;
        }

        if (a == null)
        {
            return false;
        }

        return a.Equals(b);
    }

    private void GuardNumberOperator(Token op, object operand)
    {
        if (operand is not double)
        {
            throw new RuntimeException(op, "Operand must be a number");
        }
    }

    private void GuardNumberOperator(Token op, object left, object right)
    {
        if (left is not double || right is not double)
        {
            throw new RuntimeException(op, "Operand must be a number");
        }
    }

    private string Stringify(object obj)
    {
        if (obj is null)
        {
            return "nil";
        }

        if (obj is double)
        {
            var text = obj.ToString();
            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }

            return text;
        }

        return obj.ToString();
    }


    private void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }
}

public class RuntimeException : Exception
{
    public RuntimeException(Token token, string message) : base(message)
    {
        this.token = token;
    }

    public Token token { get; init; }
}