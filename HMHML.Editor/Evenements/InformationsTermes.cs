using HLHML.AnalyseurLexical;
using Serilog;
using System;
using System.Windows.Forms;

namespace HMHML.Editor.Evenements
{
    internal class InformationsTermes
    {
        private readonly RichTextBox richTextBox1;
        private readonly ToolTip toolTip;

        public InformationsTermes(RichTextBox richTextBox)
        {
            richTextBox1 = richTextBox;
            toolTip = new ToolTip();
        }

        private bool done = true;

        private int lastCharIndex;

        internal void MontrerInformationsTermes(object sender, MouseEventArgs e)
        {
            if (done)
            {
                try
                {
                    done = false;

                    GenererToolTip(e);
                }
                catch (Exception exception)
                {
                    Log.Error(exception, string.Empty);
                }
                finally 
                {
                    done = true;
                }
            }
        }

        private void GenererToolTip(MouseEventArgs e)
        {
            var mouseLocation = e.Location;

            var charIndex = richTextBox1.GetCharIndexFromPosition(mouseLocation);

            if (lastCharIndex != charIndex)
            {
                lastCharIndex = charIndex;

                var lexer = new Lexer(richTextBox1.Text).PrendreEnComptesEspacement();

                while (lexer.Position < charIndex)
                {
                    lexer.ObtenirProchainTerme();
                }

                toolTip.SetToolTip(richTextBox1, lexer.DernierTerme.ToString());
            }
        }
    }
}