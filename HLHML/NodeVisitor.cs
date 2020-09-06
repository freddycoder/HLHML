using HLHML.LanguageElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML
{
    public static class NodeVisitor
    {
        public static void Visit(AST ast)
        {
            foreach (var instruction in ast.Childs)
            {
                VisitNode(instruction);
            }
        }

        public static void VisitNode(AST instruction)
        {
            if (instruction is IActionnable action)
            {
                action.Actionner();
            }
        }

        internal static dynamic? Eval(AST ast)
        {
            if (ast.Type == TokenType.Sujet)
            {
                return ast.Scope[ast.Value];
            }
            else if (ast.Type == TokenType.Compound)
            {
                Visit(ast);

                return ast.Scope[ast.Parent.Childs[0].Value];
            }
            else if (ast is OperateurMathematique op)
            {
                return op.Eval();
            }

            return ast.Value;
        }
    }
}
