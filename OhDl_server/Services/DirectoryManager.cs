namespace OhDl_server.Services;

public class DirectoryManager
{
    public DirectoryInfo DirectoryCreator(string type, Guid uuId)
    {
        int randomDirectory;
        do
        {
            randomDirectory = new Random().Next(0, 10000);
        } while (Directory.Exists(AssemblePath(type, uuId: uuId, randomDirectory)));
        
        var directory = Directory.CreateDirectory(AssemblePath(type, uuId: uuId, randomDirectory));

        return directory;
    }
    
    private string AssemblePath(string type, Guid uuId, int randomDirectory)
    {
        string[] filePaths = 
        { 
            AppDomain.CurrentDomain.BaseDirectory,
            type,
            uuId.ToString(),
            randomDirectory.ToString()
        };

        return Path.Combine(filePaths);
    }
}