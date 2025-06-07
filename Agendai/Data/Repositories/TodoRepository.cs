
using Agendai.Data.Database;
using Agendai.Data.Models;
using Agendai.Data.Repositories.Interfaces;

namespace Agendai.Data.Repositories;

class TodoRepository : Repository<Todo>, ITodoRepository
{
    public TodoRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
