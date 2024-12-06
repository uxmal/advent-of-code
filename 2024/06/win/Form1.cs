using Advent2024.Day06.Core;

namespace Advent2024.Day06.Win;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        var vm = (ViewModel?)DataContext;
        if (vm is null)
        {
            return;
        }
        Render(vm);
    }

    private void Render(ViewModel vm)
    {
        IRenderDevice device = new WindowRenderDevice(this, pictureBox);
        vm.State.Render(device);
        throw new NotImplementedException();
    }
}
