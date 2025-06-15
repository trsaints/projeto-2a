using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agendai.Data.Database.Context;

class DbFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        var databasePath = AppDbContext.GetDatabasePath();
        var dbFile = Path.Combine(databasePath, "agendai.db");

        optionsBuilder
            .UseSqlite($"Data Source={dbFile}",
                options => options.MigrationsAssembly("Agendai"))
            .UseLazyLoadingProxies();

        return new AppDbContext(optionsBuilder.Options);
    }
}
