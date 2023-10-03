using OhDl_server.Models;

namespace OhDl_server.DataLayer.Repository;

public interface IFileRepository : IRepository<FileTracker>
{
    public Task<FileTracker?> GetByFileNameAndType(string name, string type);
}