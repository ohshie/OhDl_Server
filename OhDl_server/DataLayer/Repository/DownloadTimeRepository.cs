using Microsoft.EntityFrameworkCore;
using OhDl_server.DataLayer.DbContext;
using OhDl_server.Models;

namespace OhDl_server.DataLayer.Repository;

public class DownloadTimeRepository : IRepository<DownloadTimeStat>
{
    private readonly OhDlDbContext _dbContext;

    public DownloadTimeRepository(OhDlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DownloadTimeStat?> Get(int id)
    {
        return await _dbContext
            .DownloadTimeStats
            .SingleOrDefaultAsync(dlTS => dlTS.Id == id);
    }

    public async Task<IEnumerable<DownloadTimeStat>> GetAll()
    {
        return _dbContext.DownloadTimeStats.AsNoTracking();
    }

    public async Task Add(DownloadTimeStat entity)
    {
        _dbContext.DownloadTimeStats.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(DownloadTimeStat entity)
    {
        _dbContext.DownloadTimeStats.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(DownloadTimeStat entity)
    {
        _dbContext.DownloadTimeStats.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}