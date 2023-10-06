using Elsa.Activities.Temporal;
using Elsa.Builders;
using NodaTime;
using OhDl_server.DataLayer;
using OhDl_server.Models;

namespace OhDl_server.YtDlp.ElsaCleaner;

public class FileCleaner : IWorkflow
{
    private readonly IServiceProvider _provider;
    private readonly int _folderCleanTimer;
    private readonly int _fileCleanTimer;


    public FileCleaner(IServiceProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _fileCleanTimer = configuration.GetSection("CleaningFrequency").GetValue<int>("fileCleaning");
        _folderCleanTimer = configuration.GetSection("CleaningFrequency").GetValue<int>("folderCleaning");
    }

    private async Task Execute()
    {
        using var scope = _provider.CreateScope();

        var fileOperator = scope.ServiceProvider.GetRequiredService<FileOperator>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<FileCleaner>>();
        
        var files = await fileOperator.ServeFiles();
        if (!files.Any()) return;

        foreach (var file in files)
        {
            await RemoveFiles(fileOperator, logger, file);
        }
        
        FolderCleanup();
    }

    private async Task RemoveFiles(FileOperator fileOperator, ILogger<FileCleaner> logger, FileTracker file)
    {
        if (file.CreationTime.AddMinutes(_fileCleanTimer) >= DateTime.Now) return;
        var dir = PathBuilder(file);
        if (!Directory.Exists(dir))
        {
            logger.Log(LogLevel.Warning, "There is no folder for {File}", file.Filename);
            return;
        }
            
        try
        {
            Directory.Delete(dir, true);
            logger.Log(LogLevel.Information, "{Filename} deleted Successfully", file.Filename);
            await fileOperator.RemoveFile(file);
        }
        catch (Exception e)
        {
            logger.LogError(eventId: 0, 
                "Something went wrong while deleting {FileFileName}. {Exception}", 
                file.Filename, e);
            throw;
        }
    }

    private void FolderCleanup()
    {
        var types = new[] { "mp3", "mp4" };
        foreach (var type in types)
        {
            var baseDirectory = Path.Combine(new[] {AppDomain.CurrentDomain.BaseDirectory, type});
            if(!Directory.Exists(baseDirectory)) continue;
            
            var userFolders = Directory.GetDirectories(baseDirectory);
            if (!userFolders.Any()) continue;

            foreach (var userFolder in userFolders)
            {
                var randomDirectories = Directory.GetDirectories(userFolder);
                if (!randomDirectories.Any())
                {
                    Directory.Delete(userFolder, true);
                    continue;
                }

                foreach (var randomDirectory in randomDirectories)
                {
                    if (!Directory.GetFiles(randomDirectory).Any() && Directory.GetCreationTime(randomDirectory)+TimeSpan.FromMinutes(_folderCleanTimer) <= DateTime.Now) 
                        Directory.Delete(randomDirectory);
                }
            }
        }
        
    }

    private string PathBuilder(FileTracker file)
    {
        var filePath = new[]
        {
            AppDomain.CurrentDomain.BaseDirectory,
            file.Filename.Split(".").Last(),
            file.UuId,
            file.Folder,
        };

        return Path.Combine(filePath);
    }
    
    public void Build(IWorkflowBuilder builder)
    {
        builder
            .AsTransient()
            .Timer(Duration.FromMinutes(_fileCleanTimer))
            .Then(Execute);
    }
}