using Advent2024.Day06.Core;

namespace Advent2024.Day06.Win;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        var form1 = new Form1();
        form1.DataContext = ArgsParser.EatArgs(args);
        Application.Run(new Form1());
    }    
}