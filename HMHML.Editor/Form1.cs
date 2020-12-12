using HMHML.Editor.Evenements;
using Serilog;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HMHML.Editor
{
    public partial class Form1 : Form
    {
        private readonly RedesinerArbre redesinerArbre;
        private readonly InformationsTermes informationsTermes;

        public Form1()
        {
            InitializeComponent();

            redesinerArbre = new RedesinerArbre(richTextBox1, SelectionnerPolice, pictureBox1);
            informationsTermes = new InformationsTermes(richTextBox1);

            var log = new LoggerConfiguration().WriteTo.File($"{DateTime.Now.ToString("yyyy-MM-ddThhmmss")}.txt")
                                               .CreateLogger();

            Log.Logger = log;
        }

        private Font SelectionnerPolice() => new Font(fontNameComboBox.Text, float.Parse(fontSizeInput.Text), FontStyle.Bold, GraphicsUnit.Pixel);

        private void NeedToReDrawPicture(object sender, EventArgs e) => redesinerArbre.NeedToReDrawPicture(sender, e);

        private void MontrerInformationsTermes(object sender, MouseEventArgs e) => informationsTermes.MontrerInformationsTermes(sender, e);
    }
}
