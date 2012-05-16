using System.Windows.Controls;
using System.Windows.Media;

namespace ClientApp.Controls
{
/// <summary>
/// Interaction logic for DigitalClockDots.xaml
/// </summary>
public partial class DigitalClockDots : UserControl
{
    private Brush renderBrush = null;

    public DigitalClockDots()
    {
        InitializeComponent();

        this.RenderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
    }

    public Brush RenderBrush
    {
        get
        {
            return renderBrush;
        }
        set
        {
            renderBrush = value;
            p0.Fill = renderBrush;
            p1.Fill = renderBrush;
        }
    }
}
}
