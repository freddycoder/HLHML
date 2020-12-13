using HLHML;
using HLHML.Exceptions;
using Serilog;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HMHML.Editor.Evenements
{
    internal class RedesinerArbre
    {
        private readonly RichTextBox richTextBox1;
        private readonly Func<Font> policeSelecteur;
        private readonly PictureBox pictureBox1;

        internal RedesinerArbre(RichTextBox richTextBox, Func<Font> policeSelecteur, PictureBox pictureBox)
        {
            this.richTextBox1 = richTextBox;
            this.policeSelecteur = policeSelecteur;
            this.pictureBox1 = pictureBox;
        }

        internal void MaybeRedrawPicture(object sender, KeyPressEventArgs e)
        {
            if (char.IsWhiteSpace(e.KeyChar) == false)
            {
                NeedToReDrawPicture(sender, e);
            }
        }

        internal void NeedToReDrawPicture(object sender, EventArgs e)
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
                                               policeSelecteur.Invoke(),
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

        private bool done = true;

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
