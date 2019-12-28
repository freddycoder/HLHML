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
            if (instruction is Actionnable action)
            {
                action.Actionner();
            }
        }

        internal static dynamic Eval(AST ast)
        {
            if (ast.Type == TokenType.Sujet)
            {
                return ast.Scope[ast.Value];
            }

            return ast.Value;
        }
    }
}
