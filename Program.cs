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
using MakeenBot.Interfaces.Handlers;
using MakeenBot.Handlers.WelcomeMessage;
using MakeenBot.Handlers.Course;
using MakeenBot.Handlers.Report;
using MakeenBot.Handlers.HelpMessage;

var builder = WebApplication.CreateBuilder(args);

// Extact Bot Config's
builder.Services.Configure<BotConfig>(builder.Configuration.GetSection("BotConfig"));

// Inject Service's
builder.Services.AddControllers();

// Service's
builder.Services.AddScoped<IReportService, ReportService>();

// Repositorie's
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

// Validator's
builder.Services.AddScoped<IReportValidator, ReportValidator>();

// Handler's
builder.Services.AddScoped<IBotCommandHandler, EditWelcomeMessageHandler>();
builder.Services.AddScoped<IBotCommandHandler, AddCourseHandler>();
builder.Services.AddScoped<IBotCommandHandler, EditCourseHandler>();
builder.Services.AddScoped<IBotCommandHandler, ExportReportHandler>();
builder.Services.AddScoped<IBotCommandHandler, AddReportHandler>();
builder.Services.AddScoped<IBotCommandHandler, HelpMessageHandler>();


// DbContext
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
