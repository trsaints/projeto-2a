using Avalonia;
using System;
using System.Threading.Tasks;


namespace Agendai;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
	    // Captura exceções não tratadas no domínio do aplicativo
	    AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
	    {
		    Console.WriteLine($"Unhandled exception: {e.ExceptionObject}");
	    };

	    // Captura exceções não observadas em tarefas
	    TaskScheduler.UnobservedTaskException += (sender, e) =>
	    {
		    Console.WriteLine($"Unobserved task exception: {e.Exception}");
		    e.SetObserved(); // Marca a exceção como observada
	    };
	    
	    BuildAvaloniaApp()
			    .StartWithClassicDesktopLifetime(args);
    }
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
