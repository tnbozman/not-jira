using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Data;
using StoryFirst.Api.Models;

namespace StoryFirst.Api.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Project?> GetByKeyAsync(string key)
    {
        return await _dbSet
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Key == key);
    }

    public async Task<Project?> GetWithMembersAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
