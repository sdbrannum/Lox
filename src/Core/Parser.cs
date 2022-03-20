namespace Core
{
    public class Parser
    {
        private readonly IList<Token> _tokens;
        private int _current = 0;

        public Parser(IList<Token> tokens)
        {
            this._tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            try
            {
                var statements = new List<Stmt>();
                while(!IsAtEnd())
                {
                    statements.Add(Declaration());
                }
                return statements;
             }
            catch (ParserException ex)
            {
                return null;
            }
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Or()
        {
            var expr = And();
            while (Match(TokenType.OR))
            {
                var op = Previous();
                var right = And();
                expr = new LogicalExpr(expr, op, right);
            }

            return expr;
        }

        private Expr And()
        {
            var expr = Equality();

            while (Match(TokenType.AND))
            {
                var op = Previous();
                var right = Equality();
                expr = new LogicalExpr(expr, op, right);
            }

            return expr;
        }
        

        private Expr Equality()
        {
            var expr = Comparison();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                var op = Previous();
                var right = Comparison();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr Assignment()
        {
            var expr = Or();

            if (Match(TokenType.EQUAL))
            {
                var equals = Previous();
                var val = Assignment();

                if (expr is VariableExpr exprVar)
                {
                    var name = exprVar.Name;
                    return new AssignExpr(name, val);
                }

                Error(equals, "Invalid assignment target");
            }

            return expr;
        }
        

        private Expr Comparison()
        {
            var expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                var op = Previous();
                var right = Term();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr Term()
        {
            var expr = Factor();

            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                var op = Previous();
                var right = Factor();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr Factor()
        {
            var expr = Unary();
            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                var op = Previous();
                var right = Unary();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                var op = Previous();
                var right = Unary();
                return new UnaryExpr(op, right);
            }
            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new LiteralExpr(false);
            if (Match(TokenType.TRUE)) return new LiteralExpr(true);
            if (Match(TokenType.NIL)) return new LiteralExpr(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new LiteralExpr(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new VariableExpr(Previous());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                var expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new GroupingExpr(expr);
            }

            throw Error(Peek(), "Expect expression");
        }

        private Stmt Statement()
        {
            if (Match((TokenType.FOR))) return ForStmt();
            if (Match(TokenType.IF)) return IfStmt();
            if (Match(TokenType.PRINT)) return PrintStmt();
            if (Match(TokenType.WHILE)) return WhileStmt();
            if (Match(TokenType.LEFT_BRACE)) return new BlockStmt(Block());
            return ExpressionStmt();
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.VAR)) return VarDeclaration();

                return Statement();
            } catch(ParserException e)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt ExpressionStmt()
        {
            var expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression");
            return new ExpressionStmt(expr);
        }

        private Stmt IfStmt()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'");
            var condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition");

            var thenBranch = Statement();
            Stmt? elseBranch = null;
            if (Match(TokenType.ELSE))
            {
                elseBranch = Statement();
            }

            return new IfStmt(condition, thenBranch, elseBranch);
        }

        private Stmt PrintStmt()
        {
            Expr val = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new PrintStmt(val);
        }

        private Stmt WhileStmt()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            var body = Statement();
            return new WhileStmt(condition, body);
        }

        private Stmt ForStmt()
        {
            Consume(TokenType.LEFT_PAREN, "Exepct '(' after 'for'");
            Stmt init;
            if (Match(TokenType.SEMICOLON))
            {
                init = null;
            } else if (Match(TokenType.VAR))
            {
                init = VarDeclaration();
            }
            else
            {
                init = ExpressionStmt();
            }

            Expr condition = null;
            if (!Check(TokenType.SEMICOLON))
            {
                condition = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition");

            Expr incr = null;
            if (!Check(TokenType.RIGHT_PAREN))
            {
                incr = Expression();
            }

            Consume(TokenType.RIGHT_PAREN, "Expect ')' after clauses.");
            var body = Statement();

            if (incr != null)
            {
                var bodyStatments = new List<Stmt>() { body, new ExpressionStmt(incr) };
                body = new BlockStmt(bodyStatments);
            }

            if (condition == null) condition = new LiteralExpr(true);
            body = new WhileStmt(condition, body);

            if (init != null)
            {
                var bodyStatments = new List<Stmt>() { init, body }; 
                body = new BlockStmt(bodyStatments);
            }
            
            return body;
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration");
            return new VarStmt(name, initializer);
        }

        private IEnumerable<Stmt> Block()
        {
            List<Stmt> statements = new();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block");
            return statements;
        }

        private Token Consume(TokenType tokenType, string message)
        {
            if (Check(tokenType)) return Advance();
            throw Error(Peek(), message);
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var tokenType in types)
            {
                if (Check(tokenType))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private bool Check(TokenType tokenType)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == tokenType;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current += 1;
            return Previous();
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private ParserException Error(Token token, string message)
        {
            Repl.Error(token, message);
            return new ParserException();
        }

        /// <summary>
        /// Discards tokens until we're past an error boundary
        /// </summary>
        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON) return;

                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }
                Advance();
            }
        }

    }

    public class ParserException : Exception
    {
    }
}
