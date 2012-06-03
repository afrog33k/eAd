using System.Windows;
using System.Windows.Media;

namespace ClientApp.Controls
{
    public class SigFont
    {
        public FontFamily Family = new FontFamily("Times");
        public Color ForeColor = Colors.Black;
        public Color BackColor=Colors.Red;
        public double Size =30;
        public FontWeight Weight = FontWeights.Bold;
        public FontStyle Style = FontStyles.Italic;
        public bool IsGradient = false;
    }
}