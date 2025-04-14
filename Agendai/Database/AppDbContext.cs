using Agendai.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace Agendai.Data.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Shift> Shifts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Define o caminho do banco de dados na pasta AppData do usuário
                string dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Agendai", "agendai.db");
                
                // Garante que a pasta exista
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
                
                // Configura o SQLite
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuração para herança (TPC = Table Per Concrete type)
            modelBuilder.Entity<Entity>().UseTpcMappingStrategy();
            
            // Configura o tipo ulong como chave
            modelBuilder.Entity<Entity>()
                .Property(e => e.Id)
                .ValueGeneratedNever(); // Não gera automaticamente os IDs
            
            // Configura o relacionamento Todo -> Shift (1-N)
            modelBuilder.Entity<Shift>()
                .HasOne<Todo>() // Todo associado ao Shift
                .WithMany(t => t.Shifts) // Referência à coleção de Shifts no Todo
                .HasForeignKey("TodoId") // Nome da chave estrangeira
                .IsRequired(false); // Torna o relacionamento opcional inicialmente
                
            // Configuração para o armazenamento de IEnumerable<DateTime> em Reminders
            modelBuilder.Entity<Recurrence>()
                .Property(r => r.Reminders)
                .HasConversion(
                    v => string.Join(',', v ?? Array.Empty<DateTime>()),
                    v => string.IsNullOrEmpty(v) 
                        ? null 
                        : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(DateTime.Parse)
                            .ToList());
        }
    }
}