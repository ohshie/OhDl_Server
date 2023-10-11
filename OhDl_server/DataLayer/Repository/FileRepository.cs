using Microsoft.EntityFrameworkCore;
using OhDl_server.DataLayer.DbContext;
using OhDl_server.Models;

namespace OhDl_server.DataLayer.Repository;

public class FileRepository : IFileRepository
{
    private readonly OhDlDbContext _dbContext;

    public FileRepository(OhDlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FileTracker?> Get(int id)
    {
        return await _dbContext
            .FileTrackers
            .SingleOrDefaultAsync(ft => ft.Id == id);
    }

    public async Task<IEnumerable<FileTracker>> GetAll()
    {
        return await _dbContext.FileTrackers.AsNoTracking().ToListAsync();
    }

    public async Task Add(FileTracker entity)
    {
        _dbContext.FileTrackers.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(FileTracker entity)
    {
        _dbContext.FileTrackers.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(FileTracker entity)
    {
        _dbContext.FileTrackers.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<FileTracker?> GetByNameAndFolder(string name, string folder)
    {
       return await _dbContext.FileTrackers
           .FirstOrDefaultAsync(ft => ft.Filename == name && ft.Folder == folder);
    }
}