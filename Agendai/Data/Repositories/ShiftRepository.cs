using Agendai.Data.Database;
using Agendai.Data.Models;
using Agendai.Data.Repositories.Interfaces;

namespace Agendai.Data.Repositories;

class ShiftRepository : Repository<Shift>, IShiftRepository
{
    public ShiftRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
