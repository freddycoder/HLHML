using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.Test
{
    [TestClass]
    public class Goal_Programming
    {
        [TestMethod]
        [Timeout(2000)]
        public void Fibonacci()
        {
            var program = "a = 1. b = 1. c = 1." +
                          "tant que c est plus petit que 900," +
                          "c = a + b." +
                          "a = b." +
                          "b = c." +
                          "Ensuite, afficher c.";

            Interprete(program, "987");
        }

        [TestMethod]
        public void AdditionDeTroisNombres()
        {
            Interprete("Afficher 3 + 3 + 3.", "9");
        }

        [TestMethod]
        public void CombinaisonsSoustractionsEtAdditions()
        {
            Interprete("Afficher 5 - 2 + 5 - 1 + 10 + 10 - 50", "-23");
        }

        [TestMethod]
        public void Soustraction()
        {
            Interprete("Afficher 12 - 4.", "8");
        }

        [TestMethod]
        public void TroisSoustractions()
        {
            Interprete("Afficher 5 - 10 - 10", "-15");
        }

        [TestMethod]
        public void Multiplication()
        {
            Interprete("Afficher 4 * 10.", "40");
        }

        [TestMethod]
        public void Division()
        {
            Interprete("Afficher 10 / 5.", "2");
        }

        private void Interprete(string program, string expectedOuput)
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual(expectedOuput, sw.ToString());
            }
        }
    }
}
