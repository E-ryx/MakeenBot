using Telegram.Bot;
using MakeenBot.Interfaces;
using MakeenBot.Services;
using MakeenBot.Repositories;
using MakeenBot.Models;
using System.Net.Sockets;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Extact Bot Configs.
builder.Services.Configure<BotConfig>(builder.Configuration.GetSection("BotConfig"));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Set Server Ip and Port.
app.Urls.Add("https://*:" + builder.Configuration["Port"]);

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
