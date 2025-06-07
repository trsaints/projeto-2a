using Agendai.Data.Database;
using Agendai.Data.Repositories;
using Agendai.Data.Repositories.Interfaces;
using Agendai.ViewModels;
using Agendai.ViewModels.Agenda;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Agendai;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        ServiceCollection services = new();

        services.AddDbContext<AppDbContext>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<ITodoRepository, TodoRepository>();

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<HomeWindowViewModel>();
        services.AddTransient<AgendaWindowViewModel>();
        services.AddTransient<TodoWindowViewModel>();
        services.AddTransient<PomodoroWindowViewModel>();

        App.ServiceProvider = services.BuildServiceProvider();

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
