using Xunit;
using static HLHML.Test.Outils.OutilsInterpreteur;

namespace HLHML.Test.Goal
{
    public class Goal_Programming
    {
        [Fact]
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

        [Fact]
        public void ScopeIteration()
        {
            var program = "i vaut 0." +
                          "Tant que i est plus petit que 3, " +
                          "i = i + 1." +
                          "Si i est égal à 3, afficher i \" \"." +
                          "Ensuite, afficher i.";

            Interprete(program, "3 3");
        }

        [Fact]
        public void AdditionDeTroisNombres()
        {
            Interprete("Afficher 3 + 3 + 3.", "9");
        }

        [Fact]
        public void CombinaisonsSoustractionsEtAdditions()
        {
            Interprete("Afficher 5 - 2 + 5 - 1 + 10 + 10 - 50", "-23");
        }

        [Fact]
        public void Soustraction()
        {
            Interprete("Afficher 12 - 4.", "8");
        }

        [Fact]
        public void TroisSoustractions()
        {
            Interprete("Afficher 5 - 10 - 10", "-15");
        }

        [Fact]
        public void Multiplication()
        {
            Interprete("Afficher 4 * 10.", "40");
        }

        [Fact]
        public void Division()
        {
            Interprete("Afficher 10 / 5.", "2");
        }

        [Fact]
        public void Division2()
        {
            Interprete("Afficher 1 / 2.", "0,5");
        }

        [Fact]
        public void Division3()
        {
            Interprete("n vaut 1 / 4. Afficher n.", "0,25");
        }

        [Fact]
        public void NombreDecimale1()
        {
            Interprete("Afficher 3,14159.", "3,14159");
        }

        [Fact]
        public void NombreNegatif()
        {
            Interprete("Afficher -123.", "-123");
        }

        [Fact]
        public void NombreNegatif2()
        {
            Interprete("Afficher - 123.", "-123");
        }

        [Fact]
        public void NombreNegatif3()
        {
            Interprete("Afficher - 123 - 123.", "-246");
        }

        [Fact]
        public void NombreNegatif4()
        {
            Interprete("Afficher 10 * - 3.", "-30");
        }

        [Fact]
        public void NombreNegatif5()
        {
            Interprete("Afficher -5--5", "0");
        }

        [Fact]
        public void NombreNegatif6()
        {
            Interprete("Tant que -5--5 n'est pas égal à 0, afficher \"boucle infinit\"", "");
        }

        [Fact]
        public void NombreNegatif6_1()
        {
            Interprete("Tant que 0 n'est pas égal à -5--5, afficher \"boucle infinit\"", "");
        }

        [Theory]
        [InlineData("Si -5--5 n'est pas égal à 0, afficher \"bouble infinit\"", "")]
        [InlineData("Si 0 n'est pas égal à -5--5, afficher \"bouble infinit\"", "")]
        [InlineData("Si -5--5 n'est pas égal à 0, afficher \"bouble infinit\" sinon afficher \"youpi1\"", "youpi1")]
        [InlineData("Si 0 n'est pas égal à -5--5, afficher \"bouble infinit\" sinon afficher \"youpi2\"", "youpi2")]
        public void NombreNegatif7(string programme, string resultatAttendue)
        {
            Interprete(programme, resultatAttendue);
        }

        [Fact]
        public void MultiplicationAndAddition()
        {
            Interprete("Afficher 10 * 10 + 10", "110");
        }

        [Fact]
        public void NombreDecimale3()
        {
            Interprete("Afficher 1,5 + 1,6.", "3,1");
        }

        [Fact]
        public void NombreDecimale4()
        {
            Interprete("Afficher 100 * 1,5", "150");
        }

        [Fact]
        public void NombreDecimale5()
        {
            Interprete("Afficher 150 / 1,5", "100");
        }
    }
}
