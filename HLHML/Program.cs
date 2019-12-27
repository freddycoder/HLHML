using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var interpreteur = new Interpreteur();

            if (args.Length == 0)
            {
                string input = "";
                while (input != "quitter")
                {
                    try
                    {
                        Console.Write(">>> ");
                        input = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            interpreteur.Interprete(input);
                        }
                    }
                    catch (Exception e)
                    {
                        while (e != null)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine();
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine();

                            e = e.InnerException;
                        }
                    }
                }
            }
            else
            {
                interpreteur.Interprete(ReadAllText(args[0]));
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
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.Default;
        }
    }
}
