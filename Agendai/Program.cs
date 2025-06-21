using Avalonia;
using Avalonia.Logging;
using System;
using System.Threading.Tasks;
using Serilog;


namespace Agendai;

sealed class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args)
	{
		// Configuração do Serilog
		Log.Logger = new LoggerConfiguration()
		             .MinimumLevel.Debug()
		             .WriteTo.Console()
		             .CreateLogger();

		// Captura exceções não observadas em tarefas
		TaskScheduler.UnobservedTaskException += (sender, e) =>
		{
			Log.Error(e.Exception, "Unobserved task exception");
			e.SetObserved(); // Marca a exceção como observada
		};

		// Captura exceções globais
		AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
		{
			Log.Fatal(
				e.ExceptionObject as Exception,
				"Unhandled exception"
			);
		};

		try { BuildAvaloniaApp().StartWithClassicDesktopLifetime(args); }
		catch (Exception ex)
		{
			Log.Fatal(ex, "Application terminated unexpectedly");

			throw;
		}
		finally { Log.CloseAndFlush(); }
	}

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
	{
		Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
		                                      .WriteTo.Console()
		                                      .CreateLogger();

		return AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
	}
}
