using Telegram.Bot;
using MakeenBot.Interfaces;
using MakeenBot.Services;
using MakeenBot.Repositories;
using System.Net.Sockets;
using System.Net;
using MakeenBot.Data;
using Microsoft.EntityFrameworkCore;
using MakeenBot.Models.ValueObjects;
using MakeenBot.Interfaces.Services;
using MakeenBot.Interfaces.Repositories;
using MakeenBot.Interfaces.Validators;
using MakeenBot.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Extact Bot Configs.
builder.Services.Configure<BotConfig>(builder.Configuration.GetSection("BotConfig"));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IReportValidator, ReportValidator>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

// Add services to the container
builder.Services.AddDbContext<BotDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Set Server Ip and Port.
//app.Urls.Add("https://*:" + builder.Configuration["Port"]);

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
