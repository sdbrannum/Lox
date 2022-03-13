using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace CodeGen
{
    [Generator]
    public class SyntaxGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            throw new NotImplementedException();
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Find the main method
            //var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);

            var expressions = new Dictionary<string, string>()
            {
                { "Binary", "Expr Left, Token Op, Expr Right" },
                { "Grouping", "Expr expression" },
                { "Literal", "object val" },
                { "Unary", "Token Op, Expr Right" }
            };
            string source = $@"
            // Auto-generated code
            using System;
            
            namespace Core;
            {{";
            foreach (var exp in expressions)
            {
                // Build up the source code
                source += $@"
                public record {exp.Key}({exp.Value}) : Expr;";
            }

            source += $@"}}";
            // Add the source code to the compilation
            context.AddSource($"Expr.g.cs", source);

        }
    }
}
