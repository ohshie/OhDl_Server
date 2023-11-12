using OhDl_server.DataLayer.Repository;
using OhDl_server.Models;

namespace OhDl_server.DataLayer;

public class FileOperator
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger _logger;

    public FileOperator(IFileRepository fileRepository, ILogger<FileOperator> logger)
    {
        _fileRepository = fileRepository;
        _logger = logger;
    }

    public async Task RegisterNewFile(string filePath, string uuId)
    {
        var filename = filePath.Split("/").Last();
        var randomFolder = filePath.Split("/")[^2];
        
        var file = await _fileRepository.GetByNameAndFolder(filename, randomFolder);
        if (file != null) return;
        
        file = new FileTracker
        {
            Filename = filename,
            Folder = randomFolder,
            UuId = uuId,
            CreationTime = DateTime.Now,
        };

        await _fileRepository.Add(file);
        
        _logger.Log(LogLevel.Information, "registered new file {Filename}", file.Filename);
    }

    public async Task<IEnumerable<FileTracker>> ServeFiles()
    {
        var files = await _fileRepository.GetAll();
        return files;
    }

    public async Task RemoveFile(FileTracker file)
    {
        await _fileRepository.Delete(file);
    }
}