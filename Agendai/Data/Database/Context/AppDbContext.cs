using Agendai.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Agendai.Data.Database.Context;

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<Todo> Todos { get; set; }
    public DbSet<Shift> Shifts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var databasePath = GetDatabasePath();

        optionsBuilder.UseSqlite(Path.Combine(databasePath, "agendai.db"),
            options =>
            {
                options.MigrationsAssembly("Agendai.Data.Database.Migrations");
            })
            .UseLazyLoadingProxies();
    }

    private static string GetDatabasePath()
    {
        var myDocments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        if (Path.Exists(myDocments))
        {
            return myDocments;
        }

        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        if (Path.Exists(appData))
        {
            return appData;
        }

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        if (Path.Exists(localAppData))
        {
            return localAppData;
        }

        var root = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (Path.Exists(root))
        {
            return root;
        }

        throw new DirectoryNotFoundException("Could not find a suitable directory for the database.");
    }
}
