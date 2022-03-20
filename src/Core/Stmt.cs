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
            T VisitIfStmt(IfStmt stmt);
            T VisitWhileStmt(WhileStmt stmt);
        }
    }

    

    public record ExpressionStmt(Expr Expression) : Stmt
    {
        public override T Accept<T>(Visitor<T> visitor)
        {
             return visitor.VisitExpressionStmt(this);
        }
    }

    public record PrintStmt(Expr Expression) : Stmt 
    {
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    public record VarStmt(Token Name, Expr Initializer) : Stmt 
    {
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }


    public record BlockStmt(IEnumerable<Stmt> Statements) : Stmt
    {
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }

    public record IfStmt(Expr Condition, Stmt ThenBranch, Stmt? ElseBranch) : Stmt
    {
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitIfStmt(this);
        }
    }
    
    public record WhileStmt(Expr Condition, Stmt Body) : Stmt
    {
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }
    }
}
