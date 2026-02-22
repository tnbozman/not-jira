using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Data;
using StoryFirst.Api.Models;

namespace StoryFirst.Api.Repositories;

public class EpicRepository : Repository<Epic>, IEpicRepository
{
    public EpicRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Epic>> GetByThemeIdAsync(int themeId)
    {
        return await _dbSet
            .Where(e => e.ThemeId == themeId)
            .Include(e => e.Outcome)
            .OrderBy(e => e.Order)
            .ToListAsync();
    }

    public async Task<Epic?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Outcome)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
