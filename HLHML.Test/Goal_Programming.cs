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
        [Timeout(2000)]
        public void ScopeIteration()
        {
            var program = "i vaut 0." +
                          "Tant que i est plus petit que 3, " +
                          "i = i + 1." +
                          "Si i est égal à 3, afficher i \" \"." +
                          "Ensuite, afficher i.";

            Interprete(program, "3 3");
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

        [TestMethod]
        public void Division2()
        {
            Interprete("Afficher 1 / 2.", "0,5");
        }

        [TestMethod]
        public void Division3()
        {
            Interprete("n vaut 1 / 4. Afficher n.", "0,25");
        }

        [TestMethod]
        public void NombreDecimale()
        {
            Interprete("Afficher 3,14159.", "3,14159");
        }

        [TestMethod]
        public void NombreDecimale2()
        {
            Interprete("Afficher 3.14159.", "3.14159");
        }

        [TestMethod]
        public void NombreNegatif()
        {
            Interprete("Afficher -123.", "-123");
        }

        [TestMethod]
        public void NombreNegatif2()
        {
            Interprete("Afficher - 123.", "-123");
        }

        [TestMethod]
        public void NombreNegatif3()
        {
            Interprete("Afficher - 123 - 123.", "-246");
        }

        [TestMethod]
        public void NombreNegatif4()
        {
            Interprete("Afficher 10 * - 3.", "-30");
        }

        [TestMethod]
        public void NombreNegatif5()
        {
            Interprete("Afficher -5--5", "0");
        }

        [TestMethod]
        [Timeout(2000)]
        public void NombreNegatif6()
        {
            Interprete("Tant que -5--5 n'est pas égal à 0, afficher \"bouble infinit\"", "");
        }

        [TestMethod]
        [Timeout(2000)]
        public void NombreNegatif6_1()
        {
            Interprete("Tant que 0 n'est pas égal à -5--5, afficher \"bouble infinit\"", "");
        }

        [TestMethod]
        public void NombreNegatif7()
        {
            Interprete("Si -5--5 n'est pas égal à 0, afficher \"bouble infinit\"", "");
            Interprete("Si 0 n'est pas égal à -5--5, afficher \"bouble infinit\"", "");
            Interprete("Si -5--5 n'est pas égal à 0, afficher \"bouble infinit\" sinon afficher \"youpi\"", "youpi");
            Interprete("Si 0 n'est pas égal à -5--5, afficher \"bouble infinit\" sinon afficher \"youpi\"", "youpi");
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
