using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Data;
using StoryFirst.Api.Models;

namespace StoryFirst.Api.Repositories;

public class StoryRepository : Repository<Story>, IStoryRepository
{
    public StoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Story>> GetByEpicIdAsync(int epicId)
    {
        return await _dbSet
            .Where(s => s.EpicId == epicId)
            .Include(s => s.Outcome)
            .Include(s => s.Sprint)
            .OrderBy(s => s.Order)
            .ToListAsync();
    }

    public async Task<Story?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Outcome)
            .Include(s => s.Sprint)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}
