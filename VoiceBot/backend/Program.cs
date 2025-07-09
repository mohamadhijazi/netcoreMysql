using Microsoft.EntityFrameworkCore;
using VoiceBot.Data;
using VoiceBot.Services;
using VoiceBot.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add EF Core with MySQL
builder.Services.AddDbContext<VoiceBotDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Register modular services
builder.Services.AddHttpClient<PythonTtsService>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["PythonService:BaseUrl"] ?? "http://localhost:8000");
});
builder.Services.AddScoped<ITtsService, PythonTtsService>();

builder.Services.AddHttpClient<PythonSttService>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["PythonService:BaseUrl"] ?? "http://localhost:8000");
});
builder.Services.AddScoped<ISttService, PythonSttService>();

builder.Services.AddHttpClient<PythonLlmService>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["PythonService:BaseUrl"] ?? "http://localhost:8000");
});
builder.Services.AddScoped<ILlmService, PythonLlmService>();

builder.Services.AddScoped<ICsvService, CsvService>();
builder.Services.AddScoped<IHardwareConfigService, HardwareConfigService>();

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(builder.Configuration["Frontend:BaseUrl"] ?? "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddSignalR();

// TODO: Add authentication, SignalR, and other services

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");

app.MapControllers();
app.MapHub<VoiceHub>("/voicehub");

// TODO: Map endpoints for TTS, STT, LLM, admin, hardware switching, SignalR, etc.

// EF Core migration: run 'dotnet ef migrations add InitialCreate' and 'dotnet ef database update' to create/update schema.

app.Run();
