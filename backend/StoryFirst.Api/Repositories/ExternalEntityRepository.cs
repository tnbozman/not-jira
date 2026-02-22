using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Data;
using StoryFirst.Api.Models;

namespace StoryFirst.Api.Repositories;

public class ExternalEntityRepository : Repository<ExternalEntity>, IExternalEntityRepository
{
    public ExternalEntityRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ExternalEntity>> GetByProjectIdAsync(int projectId)
    {
        return await _dbSet
            .Where(e => e.ProjectId == projectId)
            .Include(e => e.Problems)
            .Include(e => e.EntityTags)
                .ThenInclude(et => et.Tag)
            .ToListAsync();
    }

    public async Task<ExternalEntity?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Problems)
                .ThenInclude(p => p.Outcomes)
            .Include(e => e.Interviews)
            .Include(e => e.EntityTags)
                .ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
