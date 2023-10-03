using Elsa.Activities.Temporal;
using Elsa.Builders;
using NodaTime;
using OhDl_server.DataLayer;

namespace OhDl_server.YtDlp.ElsaCleaner;

public class FileCleaner : IWorkflow
{
    private readonly IServiceProvider _provider;

    public FileCleaner(IServiceProvider provider)
    {
        _provider = provider;
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
            if (file.CreationTime.AddMinutes(5) >= DateTime.Now) continue;

            var dir = $"./{file.Type}/{file.UuId}/";
            var filePath = $"{dir}{file.FileName}{file.Extension}";
            
            try
            {
                File.Delete(filePath);
                if (!Directory.GetFiles(dir).Any()) Directory.Delete(dir);
                logger.LogInformation(eventId: 0, "{fileName} deleted Successfully", file.FileName);
                await fileOperator.RemoveFile(file);
            }
            catch (Exception e)
            {
                logger.LogError(eventId: 0, 
                    "Something went wrong while deleting {FileFileName}. {E}", 
                    file.FileName, e);
                throw;
            }
        }
    }
    
    public void Build(IWorkflowBuilder builder)
    {
        builder
            .AsTransient()
            .Timer(Duration.FromMinutes(5))
            .Then(Execute);
    }
}