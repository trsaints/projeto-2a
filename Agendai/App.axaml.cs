using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Agendai.Controllers;
using Agendai.Services.Views;
using Avalonia.Markup.Xaml;
using Agendai.ViewModels;
using Agendai.Views;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;


namespace Agendai;

public partial class App : Application
{
	public static IServiceProvider ServiceProvider { get; private set; } = null!;

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);

		if (!Resources.TryGetResource("ViewLocator", ThemeVariant.Default, out _))
		{
			Resources.Add("ViewLocator", new ViewLocator());
		}
	}

	public override void OnFrameworkInitializationCompleted()
	{
		ServiceCollection serviceCollection = new();

		serviceCollection.AddSingleton<MainWindowViewModel>()
		                 .AddTransient<EventListViewModel>()
		                 .AddTransient<TodoListViewModel>()
		                 .AddTransient<AgendaWindowViewModel>()
		                 .AddTransient<TodoWindowViewModel>()
		                 .AddTransient<PomodoroWindowViewModel>();

		serviceCollection.AddTransient<AgendaMonthController>()
		                 .AddTransient<AgendaWeekController>()
		                 .AddTransient<AgendaDayController>();

		ServiceProvider = serviceCollection.BuildServiceProvider();

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			DisableAvaloniaDataAnnotationValidation();

			desktop.MainWindow = new MainWindow
			{
				DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>()
			};
		}

		base.OnFrameworkInitializationCompleted();
	}

	private static void DisableAvaloniaDataAnnotationValidation()
	{
		var dataValidationPluginsToRemove =
				BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>()
				              .ToArray();

		foreach (var plugin in dataValidationPluginsToRemove)
		{
			BindingPlugins.DataValidators.Remove(plugin);
		}
	}
}
