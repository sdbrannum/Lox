using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
 

    public abstract record Stmt 
    {
        public abstract T Accept<T>(Visitor<T> visitor);

        public interface Visitor<T>
        {
            T VisitExpressionStmt(ExpressionStmt stmt);
            T VisitPrintStmt(PrintStmt stmt);
            T VisitVarStmt(VarStmt stmt);

            T VisitBlockStmt(BlockStmt stmt);
        }
    }

    public record ExpressionStmt : Stmt
    {
        public Expr Expression { get; init; } = default!;

        public ExpressionStmt(Expr expression)
        {
            this.Expression = expression;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
             return visitor.VisitExpressionStmt(this);
        }
    }

    public record PrintStmt : Stmt 
    {
        public Expr Expression { get; init; } = default!;

        public PrintStmt(Expr expression)
        {
            this.Expression = expression;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    public record VarStmt : Stmt 
    {
        public Token Name { get; init; } = default!;
        public Expr Initializer { get; init; } = default!;
        
        public VarStmt(Token name, Expr initializer)
        {
            this.Name = name;
            this.Initializer = initializer;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }


    public record BlockStmt : Stmt
    {
        public IEnumerable<Stmt> Statements { get; init; }

        public BlockStmt(IEnumerable<Stmt> statements)
        {
            this.Statements = statements;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }
    
}
