using Runtime;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Apple2;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    [RequiresAssemblyFiles()]
    static void Main()
    {
    
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Interface());
    }    
}