using Agendai.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;


namespace Agendai.Data.Database;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<Todo> Todos { get; set; }
    public DbSet<Shift> Shifts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!Path.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }

        optionsBuilder.UseSqlite($"Data Source={Path.Combine(DataPath, "agendai.db")}");
    }

    private string DataPath => Path.Combine(DatabasePath, "Agendai", "Dados");

    private string DatabasePath
    {
        get
        {
            var documents
                = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (documents != string.Empty) return documents;

            var desktop
                = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (desktop != string.Empty) return desktop;

            var userProfile
                = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            if (userProfile != string.Empty) return userProfile;

            var myComputer
                = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

            return myComputer != string.Empty
                ? myComputer
                : Environment.GetFolderPath(Environment.SpecialFolder.System);
        }
    }
}
