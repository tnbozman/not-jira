using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Data;
using StoryFirst.Api.Models;

namespace StoryFirst.Api.Repositories;

public class ThemeRepository : Repository<Theme>, IThemeRepository
{
    public ThemeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Theme>> GetByProjectIdAsync(int projectId)
    {
        return await _dbSet
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Outcome)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }

    public async Task<Theme?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(t => t.Outcome)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
