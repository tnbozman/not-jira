using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.ProductDiscovery.Services;

public class ProblemService : IProblemService
{
    private readonly IRepository<Problem> _problemRepository;
    private readonly IExternalEntityRepository _entityRepository;

    public ProblemService(
        IRepository<Problem> problemRepository,
        IExternalEntityRepository entityRepository)
    {
        _problemRepository = problemRepository;
        _entityRepository = entityRepository;
    }

    public async Task<IEnumerable<Problem>> GetAllByProjectAsync(int projectId)
    {
        return await _problemRepository.FindAsync(p => p.ExternalEntity!.ProjectId == projectId);
    }

    public async Task<Problem?> GetByIdAsync(int projectId, int id)
    {
        return await _problemRepository.FirstOrDefaultAsync(p => p.Id == id && p.ExternalEntity!.ProjectId == projectId);
    }

    public async Task<Problem> CreateAsync(int projectId, Problem problem)
    {
        var entity = await _entityRepository.FirstOrDefaultAsync(e => e.Id == problem.ExternalEntityId && e.ProjectId == projectId);
        if (entity == null)
        {
            throw new InvalidOperationException("External entity not found or does not belong to this project");
        }

        problem.CreatedAt = DateTime.UtcNow;
        problem.UpdatedAt = DateTime.UtcNow;

        await _problemRepository.AddAsync(problem);
        await _problemRepository.SaveChangesAsync();

        return problem;
    }

    public async Task UpdateAsync(int projectId, int id, Problem problem)
    {
        if (id != problem.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingProblem = await _problemRepository.FirstOrDefaultAsync(p => p.Id == id && p.ExternalEntity!.ProjectId == projectId);
        if (existingProblem == null)
        {
            throw new KeyNotFoundException("Problem not found");
        }

        existingProblem.Description = problem.Description;
        existingProblem.Severity = problem.Severity;
        existingProblem.Context = problem.Context;
        existingProblem.ExternalEntityId = problem.ExternalEntityId;
        existingProblem.UpdatedAt = DateTime.UtcNow;

        _problemRepository.Update(existingProblem);
        await _problemRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int projectId, int id)
    {
        var problem = await _problemRepository.FirstOrDefaultAsync(p => p.Id == id && p.ExternalEntity!.ProjectId == projectId);
        if (problem == null)
        {
            throw new KeyNotFoundException("Problem not found");
        }

        _problemRepository.Remove(problem);
        await _problemRepository.SaveChangesAsync();
    }
}
