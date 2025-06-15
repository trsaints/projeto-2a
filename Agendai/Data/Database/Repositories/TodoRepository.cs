using Agendai.Data.Database.Context;
using Agendai.Data.Database.Repositories.Interfaces;
using Agendai.Data.Models;

namespace Agendai.Data.Database.Repositories;

public class TodoRepository : Repository<Todo>, ITodoRepository
{
    public TodoRepository(AppDbContext db) : base(db)
    {
    }
}
