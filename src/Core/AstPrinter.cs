using System.Text;

namespace Core;

public class AstPrinter : Expr.Visitor<string>
{
        public string Print(Expr expr)
        {
                return expr.Accept<string>(this);
        }


        public string VisitBinaryExpr(BinaryExpr expr)
        {
                return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(GroupingExpr expr)
        {
                return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(LiteralExpr expr)
        {
                return expr.Value?.ToString() ?? "nil";
        }

        public string VisitUnaryExpr(UnaryExpr expr)
        {
                return Parenthesize(expr.Op.Lexeme, expr.Right);
        }

        public string VisitVariableExpr(VariableExpr expr)
        {
                return null;
        }

        public string VisitAssignExpr(AssignExpr expr)
        {
                return null;
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
                var sb = new StringBuilder($"({name}");

                foreach (var expr in exprs)
                {
                        sb.Append(' ');
                        sb.Append(expr.Accept(this));
                }

                sb.Append(')');
                return sb.ToString();

        }
}