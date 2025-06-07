
using Agendai.Data.Database;
using Agendai.Data.Models;
using Agendai.Data.Repositories.Interfaces;

namespace Agendai.Data.Repositories;

class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
