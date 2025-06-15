using Agendai.Data.Database.Context;
using Agendai.Data.Database.Repositories.Interfaces;
using Agendai.Data.Models;

namespace Agendai.Data.Database.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(AppDbContext db) : base(db)
    {
    }
}
