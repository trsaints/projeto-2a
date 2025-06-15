using Agendai.Data.Database.Context;
using Agendai.Data.Database.Repositories;
using Agendai.Data.Database.Repositories.Interfaces;
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
