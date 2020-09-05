using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace HLHML
{
    public class AppSettingsReader
    {
        public object GetValue(string key, Type type)
        {
            return null;
        }
    }

    public class Program
    {
        public static AppSettingsReader Settings { get; } = new AppSettingsReader();

        public static void Main(string[] args)
        {
            var interpreteur = new Interpreteur(Console.Out);

            if (args.Length == 0)
            {
                string input = "";
                while (input != "quitter")
                {
                    Try(() =>
                    {
                        Console.Write(">>> ");
                        input = Console.ReadLine();
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

                        if (Settings.GetValue<bool>("openFileAfterGeneration"))
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
            catch (Exception e)
            {
                while (e != null)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();

                    if (Settings.GetValue<bool>("printStackTrace"))
                    {
                        Console.WriteLine(e.StackTrace);
                        Console.WriteLine();
                    }

                    if (Settings.GetValue<bool>("printEveryException"))
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

    public static class AppSettingsExtension
    {
        public static T GetValue<T>(this AppSettingsReader appSettingsReader, string key)
        {
            try
            {
                return (T) appSettingsReader.GetValue(key, typeof(T));
            }
            catch
            {
                return default;
            }
        }
    }
}
