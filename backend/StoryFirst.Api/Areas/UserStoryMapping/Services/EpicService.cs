using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.UserStoryMapping.Services;

public class EpicService : IEpicService
{
    private readonly IEpicRepository _epicRepository;

    public EpicService(IEpicRepository epicRepository)
    {
        _epicRepository = epicRepository;
    }

    public async Task<IEnumerable<Epic>> GetByThemeIdAsync(int themeId)
    {
        return await _epicRepository.GetByThemeIdAsync(themeId);
    }

    public async Task<Epic?> GetByIdAsync(int themeId, int id)
    {
        var epic = await _epicRepository.FirstOrDefaultAsync(e => e.ThemeId == themeId && e.Id == id);

        if (epic == null)
        {
            return null;
        }

        return await _epicRepository.GetWithDetailsAsync(id);
    }

    public async Task<Epic> CreateAsync(int themeId, Epic epic)
    {
        epic.ThemeId = themeId;
        epic.CreatedAt = DateTime.UtcNow;
        epic.UpdatedAt = DateTime.UtcNow;

        await _epicRepository.AddAsync(epic);
        await _epicRepository.SaveChangesAsync();

        return epic;
    }

    public async Task UpdateAsync(int themeId, int id, Epic epic)
    {
        if (id != epic.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingEpic = await _epicRepository.FirstOrDefaultAsync(e => e.Id == id && e.ThemeId == themeId);

        if (existingEpic == null)
        {
            throw new KeyNotFoundException("Epic not found");
        }

        existingEpic.Name = epic.Name;
        existingEpic.Description = epic.Description;
        existingEpic.Order = epic.Order;
        existingEpic.OutcomeId = epic.OutcomeId;
        existingEpic.UpdatedAt = DateTime.UtcNow;

        _epicRepository.Update(existingEpic);
        await _epicRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int themeId, int id)
    {
        var epic = await _epicRepository.FirstOrDefaultAsync(e => e.Id == id && e.ThemeId == themeId);

        if (epic == null)
        {
            throw new KeyNotFoundException("Epic not found");
        }

        _epicRepository.Remove(epic);
        await _epicRepository.SaveChangesAsync();
    }
}
