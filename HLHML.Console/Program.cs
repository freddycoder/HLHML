using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace HLHML.Console
{
    public class Program
    {
        public static IConfigurationRoot? Configuration;

        /// <summary>
        /// Point d'entrée lors du démarrage de l'executable. Execution des configurations de l'injection des dépendances.
        /// </summary>
        /// <param name="args">Les arguments, rien pour entrer en mode console, le nom du fichier pour executer le script et etc.</param>
        /// <remarks>Cette méthode n'est pas testé par les essais unitaire</remarks>
        public static void Main(string[] args)
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            var newLineWhenAfficher = Configuration?.GetBool("newLineWhenAfficher") ?? false;

            MainProgram(args, System.Console.Out, System.Console.In, newLineWhenAfficher);
        }

        /// <summary>
        /// Logique du programme HLHML.Console. 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="sdtOut"></param>
        /// <param name="newLineWhenAfficher"></param>
        /// <remarks>Cette méthode est testé par les essais unitaire</remarks>
        public static void MainProgram(string[] args, TextWriter sdtOut, TextReader stdIn, bool newLineWhenAfficher = false)
        {
            var interpreteur = new Interpreteur(sdtOut, stdIn, newLineWhenAfficher);

            if (args.Length == 0)
            {
                string input = "";
                while (input != "quitter")
                {
                    Try(() =>
                    {
                        System.Console.Write(">>> ");
                        input = System.Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(input) && !input.Equals("quitter", StringComparison.OrdinalIgnoreCase))
                        {
                            interpreteur.Interprete(input);
                        }
                    });
                }
            }
            else
            {
                var fileName = args.First(f => File.Exists(f));

                if (args.Any(a => a == "-t"))
                {
                    Try(() =>
                    {
                        var parseur = new Parseur(new Lexer(ReadAllText(fileName)));

                        var ast = parseur.Parse();

                        var destination = args.Last(f => Path.HasExtension(f) && f != fileName);

                        var drawer = new ASTDrawer(ast);

                        drawer.DrawToFile(destination);

                        if (Configuration?.GetBool("openFileAfterGeneration") == true)
                        {
                            Process.Start(destination);
                        }
                    });
                }
                else
                {
                    Try(() =>
                    {
                        interpreteur.Interprete(ReadAllText(fileName));
                    });
                }
            }
        }

        private static void ConfigureServices(ServiceCollection serviceCollection)
        {
            Configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                                    .AddJsonFile("appsettings.json", false)
                                    .Build();

            serviceCollection.AddSingleton(Configuration);
        }

        private static string ReadAllText(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var encoding = FileEncoding.DetectFileEncoding(path);

            return File.ReadAllText(path, encoding);
        }

        private static void Try(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception? e)
            {
                while (e != null)
                {
                    System.Console.WriteLine(e.Message);
                    System.Console.WriteLine();

                    if (Configuration?.GetBool("printStackTrace") == true)
                    {
                        System.Console.WriteLine(e.StackTrace);
                        System.Console.WriteLine();
                    }

                    if (Configuration?.GetBool("printEveryException") == true)
                    {
                        e = e.InnerException;
                    }
                    else
                    {
                        e = null;
                    }
                }
            }
        }
    }

    public static class GetValueExtension
    {
        public static bool GetBool(this IConfigurationRoot configurationRoot, string key)
        {
            return bool.Parse(configurationRoot[key]);
        }
    }
}
