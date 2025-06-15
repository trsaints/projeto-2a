using Agendai.Data.Database.Context;
using Agendai.Data.Database.Repositories.Interfaces;
using Agendai.Data.Models;

namespace Agendai.Data.Database.Repositories;

public class ShiftRepository : Repository<Shift>, IShiftRepository
{
    public ShiftRepository(AppDbContext db) : base(db)
    {
    }
}
