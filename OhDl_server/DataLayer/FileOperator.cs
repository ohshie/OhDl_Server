using OhDl_server.DataLayer.DbContext;
using OhDl_server.DataLayer.Repository;
using OhDl_server.Models;

namespace OhDl_server.DataLayer;

public class FileOperator
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger _logger;

    public FileOperator(IFileRepository fileRepository, ILogger<FileOperator> logger)
    {
        this._fileRepository = fileRepository;
        _logger = logger;
    }

    public async Task RegisterNewFile(string filename, string type, string uuId)
    {
        var file = await _fileRepository.GetByFileNameAndType(filename, type);
        if (file != null) return;
        
        file = new FileTracker()
        {
            FileName = filename,
            Type = type,
            UuId = uuId,
            CreationTime = DateTime.Now
        };

        file.Extension = type == "audio" ? ".mp3" : ".mp4";

        await _fileRepository.Add(file);
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