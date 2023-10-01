using NYoutubeDL;
using OhDl_server;
using OhDl_server.YtDlp;
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
builder.Services.AddTransient<TestingClass>();
builder.Services.AddTransient<StreamProvider>();
builder.Services.AddHttpClient();
builder.Services.AddSerilog();

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

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("Debug");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("Base");

app.MapControllers();

/*TestingClass testingClass = new(app.Services.GetService<YtDlOperator>());
await testingClass.Test();*/

app.Run();






