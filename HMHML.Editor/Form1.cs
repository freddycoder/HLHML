using HLHML;
using HLHML.Exceptions;
using Serilog;
using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
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
        private void NeedToReDrawPicture(object sender, EventArgs e)
        {
            if (done)
            {
                done = false;

                try
                {
                    PaintInBlack();
                    var lexer = new Lexer(richTextBox1.Text);
                    var parser = new Parseur(lexer);
                    var drawer = new ASTDrawer(parser.Parse(),
                                               new Font(fontNameComboBox.Text, float.Parse(fontSizeInput.Text), FontStyle.Bold, GraphicsUnit.Pixel),
                                               Color.White,
                                               Color.Black,
                                               Brushes.Black);
                    pictureBox1.Image = drawer.GetBitmap();
                }
                catch (ParseurException exception)
                {
                    Lexer lexer = exception.Data["Lexer"] as Lexer;
                    int debutSelection = lexer.Position - lexer.DernierTerme.Terme.Mots.Length;

                    int originalSelection = richTextBox1.SelectionStart;
                    richTextBox1.SelectionStart = debutSelection;
                    richTextBox1.SelectionLength = lexer.Position - debutSelection;
                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Underline);
                    richTextBox1.SelectionColor = Color.Red;
                    richTextBox1.SelectionLength = 0;
                    richTextBox1.SelectionStart = originalSelection;
                }
                catch (Exception exception)
                {
                    Log.Logger.Error(exception, nameof(NeedToReDrawPicture));
                }
                finally
                {
                    done = true;
                }
            }
        }

        private void PaintInBlack()
        {
            int originalSelection = richTextBox1.SelectionStart;
            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = richTextBox1.Text.Length;
            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionStart = originalSelection;
        }
    }
}
