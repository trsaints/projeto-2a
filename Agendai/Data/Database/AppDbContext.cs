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
        optionsBuilder.UseSqlite($"Data Source={Path.Combine(DatabasePath, "agendai.db")}");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Event>().ToTable("Events");
        builder.Entity<Todo>().ToTable("Todos");
        builder.Entity<Shift>().ToTable("Shifts");

        builder.Entity<Event>()
               .HasMany(e => e.Todos)
               .WithOne(t => t.Event)
               .HasForeignKey(t => t.EventId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Todo>()
                .HasMany(t => t.Shifts)
                .WithOne(s => s.Todo)
                .HasForeignKey(s => s.TodoId)
                .OnDelete(DeleteBehavior.Cascade);
    }

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
