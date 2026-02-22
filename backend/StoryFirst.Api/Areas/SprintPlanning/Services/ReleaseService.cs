using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.SprintPlanning.Services;

public class ReleaseService : IReleaseService
{
    private readonly IRepository<Release> _releaseRepository;

    public ReleaseService(IRepository<Release> releaseRepository)
    {
        _releaseRepository = releaseRepository;
    }

    public async Task<IEnumerable<Release>> GetAllByProjectAsync(int projectId)
    {
        var releases = (await _releaseRepository.FindAsync(r => r.ProjectId == projectId))
            .OrderBy(r => r.ReleaseDate)
            .ToList();

        return releases;
    }

    public async Task<Release?> GetByIdAsync(int projectId, int id)
    {
        return await _releaseRepository.FirstOrDefaultAsync(r => r.ProjectId == projectId && r.Id == id);
    }

    public async Task<Release> CreateAsync(int projectId, Release release)
    {
        release.ProjectId = projectId;
        release.CreatedAt = DateTime.UtcNow;
        release.UpdatedAt = DateTime.UtcNow;

        await _releaseRepository.AddAsync(release);
        await _releaseRepository.SaveChangesAsync();

        return release;
    }

    public async Task UpdateAsync(int projectId, int id, Release release)
    {
        if (id != release.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingRelease = await _releaseRepository.FirstOrDefaultAsync(r => r.Id == id && r.ProjectId == projectId);

        if (existingRelease == null)
        {
            throw new KeyNotFoundException("Release not found");
        }

        existingRelease.Name = release.Name;
        existingRelease.Description = release.Description;
        existingRelease.StartDate = release.StartDate;
        existingRelease.ReleaseDate = release.ReleaseDate;
        existingRelease.Status = release.Status;
        existingRelease.UpdatedAt = DateTime.UtcNow;

        _releaseRepository.Update(existingRelease);
        await _releaseRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int projectId, int id)
    {
        var release = await _releaseRepository.FirstOrDefaultAsync(r => r.Id == id && r.ProjectId == projectId);

        if (release == null)
        {
            throw new KeyNotFoundException("Release not found");
        }

        _releaseRepository.Remove(release);
        await _releaseRepository.SaveChangesAsync();
    }
}
