using Elsa;
using Elsa.Persistence.EntityFramework.Sqlite;
using NYoutubeDL;
using OhDl_server.DataLayer;
using OhDl_server.DataLayer.DbContext;
using OhDl_server.DataLayer.Repository;
using OhDl_server.YtDlp;
using OhDl_server.YtDlp.ElsaCleaner;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<YoutubeDLP>();
builder.Services.AddTransient<YtDlOperator>();
builder.Services.AddTransient<FormatSorter>();
builder.Services.AddTransient<StreamProvider>();

builder.Services.AddTransient<IFileRepository, FileRepository>();
builder.Services.AddTransient<FileOperator>();

builder.Services.AddHttpClient();
builder.Services.AddSerilog();

builder.Services.AddDbContext<OhDlDbContext>(s =>
{
    s.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Base",
        builder =>
        {
            builder.WithOrigins("https://127.0.0.1:5500")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("Content-Disposition");
        });
    options.AddPolicy("Debug", builder =>
    {
        builder.WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyMethod()
            .WithExposedHeaders("Content-Disposition");
    });
});

// builder.Services.AddTransient<IClock>(s => SystemClock.Instance);
builder.Services.AddElsa(elsa =>
        {
            elsa.AddQuartzTemporalActivities().AddWorkflow<FileCleaner>();
        });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OhDlDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("Debug");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("Base");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();






