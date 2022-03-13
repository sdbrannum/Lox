using System.Text;

namespace Core;

public class AstPrinter : Visitor<string>
{
        public string Print(Expr expr)
        {
                return expr.Accept<string>(this);
        }


        public string VisitBinary(Binary expr)
        {
                return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGrouping(Grouping expr)
        {
                return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteral(Literal expr)
        {
                return expr.Value?.ToString() ?? "nil";
        }

        public string VisitUnary(Unary expr)
        {
                return Parenthesize(expr.Op.Lexeme, expr.Right);
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