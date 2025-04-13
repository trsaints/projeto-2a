using System;
using Microsoft.EntityFrameworkCore;

namespace Agendai.Data.Database
{
    public static class DatabaseInitializer
    {
        public static void InitializeDatabase()
        {
            try
            {
                using var db = new AppDbContext();
                // Isso criará o banco de dados se ele não existir
                db.Database.Migrate();
                
                // Se preferir usar EnsureCreated em vez de Migrations
                // db.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                // Logue o erro de alguma forma
                Console.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
            }
        }
    }
}