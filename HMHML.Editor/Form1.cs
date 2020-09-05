using HLHML;
using Serilog;
using System;
using System.Windows.Forms;

namespace HMHML.Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var log = new LoggerConfiguration().WriteTo.File($"{DateTime.Now.ToString("yyyy-MM-ddThhmmss")}.txt")
                                               .CreateLogger();

            Log.Logger = log;
        }

        private bool done = true;
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (done)
            {
                done = false;

                try
                {
                    var lexer = new Lexer(richTextBox1.Text.Trim());
                    var parser = new Parseur(lexer);
                    var drawer = new ASTDrawer(parser.Parse());
                    pictureBox1.Image = drawer.GetBitmap();
                }
                catch (Exception exception)
                {
                    Log.Logger.Error(exception, nameof(richTextBox1_TextChanged));
                }
                finally
                {
                    done = true;
                }
            }
        }
    }
}
