using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML
{
    public class Program
    {
        public static AppSettingsReader Settings { get; } = new AppSettingsReader();

        public static void Main(string[] args)
        {
            var interpreteur = new Interpreteur();

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
            return File.ReadAllText(path, GetEncoding(path));
        }

        private static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if ((bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) || 
                (bom[0] == 13 && bom[1] == 10 && bom[2] == 97 && bom[3] == 32) ||
                (bom[0] == 108 && bom[1] == 97 && bom[2] == 32 && bom[3] == 108)) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.Default;
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
