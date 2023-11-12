using Microsoft.EntityFrameworkCore;
using OhDl_server.Models;

namespace OhDl_server.DataLayer.DbContext;

public class OhDlDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public required DbSet<FileTracker> FileTrackers { get; set; }
    public required DbSet<DownloadTimeStat> DownloadTimeStats { get; set; }
    
    public OhDlDbContext(DbContextOptions<OhDlDbContext> options) : base(options) {}
}