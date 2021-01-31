using HLHML;
using HLHML.AnalyseurLexical;
using HLHML.Exceptions;
using Serilog;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HMHML.Editor.Evenements
{
    internal class RedesinerArbre
    {
        private readonly RichTextBox richTextBox;
        private readonly Func<Font> policeSelecteur;
        private readonly PictureBox pictureBox;

        internal RedesinerArbre(RichTextBox richTextBox, Func<Font> policeSelecteur, PictureBox pictureBox)
        {
            this.richTextBox = richTextBox;
            this.policeSelecteur = policeSelecteur;
            this.pictureBox = pictureBox;
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

                    var lexer = new Lexer(richTextBox.Text).PrendreEnComptesEspacement();
                    var parser = new Parseur(lexer);
                    var drawer = new ASTDrawer(policeSelecteur.Invoke(),
                                               Color.White,
                                               Color.Black,
                                               Brushes.Black);

                    pictureBox.Image = drawer.GetBitmap(parser.Parse());
                }
                catch (ParseurException exception)
                {
                    PaintMistakInRed(exception);
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

        private void PaintMistakInRed(ParseurException exception)
        {
            ILexer lexer = exception.Lexer;
            int debutSelection = lexer.Position - lexer.DernierTerme.Terme.Mots.Length;

            int originalSelection = richTextBox.SelectionStart;
            richTextBox.SelectionStart = debutSelection;
            richTextBox.SelectionLength = lexer.Position - debutSelection;
            richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Underline);
            richTextBox.SelectionColor = Color.Red;
            richTextBox.SelectionLength = 0;
            richTextBox.SelectionStart = originalSelection;

            redOnScreen = true;
        }

        private bool done = true;
        private bool redOnScreen = false;

        private void PaintInBlack()
        {
            if (redOnScreen)
            {
                int originalSelection = richTextBox.SelectionStart;
                richTextBox.SelectionStart = 0;
                richTextBox.SelectionLength = richTextBox.Text.Length;
                richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Regular);
                richTextBox.SelectionColor = Color.Black;
                richTextBox.SelectionLength = 0;
                richTextBox.SelectionStart = originalSelection;

                redOnScreen = false;
            }
        }
    }
}
