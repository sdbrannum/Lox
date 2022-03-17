using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Interpreter : Visitor<object>
    {
        public void Interpret(Expr expression)
        {
            try
            {
                object val = Evaluate(expression);
                Console.WriteLine(Stringify(val));
            }
            catch (RuntimeException ex)
            {
                Repl.RuntimeError(ex);
            }
        }

        public object VisitBinary(Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch(expr.Op.Type)
            {
                case TokenType.MINUS:
                    GuardNumberOperator(expr.Op, right);
                    return (double)left - (double)right;
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
                    return (double)left / (double)right;
                case TokenType.STAR:
                    GuardNumberOperator(expr.Op, left, right);
                    return (double)left * (double)right;
                case TokenType.GREATER:
                    GuardNumberOperator(expr.Op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    GuardNumberOperator(expr.Op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    GuardNumberOperator(expr.Op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    GuardNumberOperator(expr.Op, left, right);
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
            }
            return null;
        }

        public object VisitGrouping(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteral(Literal expr)
        {
            return expr.Value;
        }

        public object VisitUnary(Unary expr)
        {
            object right = Evaluate(expr.Right);
            switch(expr.Op.Type)
            {
                case TokenType.MINUS:
                    return -(double)right;
                case TokenType.BANG:
                    return !IsTruthy(right);
            }
            return null;
        }

        private object Evaluate(Expr expression)
        {
            return expression.Accept(this);
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool booled) return booled;
            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b);
        }

        private void GuardNumberOperator(Token op, object operand)
        {
            if (operand is not double) throw new RuntimeException(op, "Operand must be a number");
        }

        private void GuardNumberOperator(Token op, object left, object right)
        {
            if (left is not double || right is not double) 
                throw new RuntimeException(op, "Operand must be a number");
        }

        private String Stringify(object obj)
        {
            if (obj is null) return "nil";

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
    }

    public class RuntimeException : Exception
    {
        public Token token { get; init; }

        public RuntimeException(Token token, string message) : base(message)
        {
            this.token = token;
        }
    } 
}
