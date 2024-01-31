using FluentValidation;
using KM_ClientApp.Commons.Connection;
using KM_ClientApp.Commons.Policy;
using KM_ClientApp.Endpoint.Category;
using KM_ClientApp.Endpoint.Config;
using KM_ClientApp.Endpoint.Content;
using KM_ClientApp.Endpoint.Email;
using KM_ClientApp.Endpoint.Feedback;
using KM_ClientApp.Endpoint.Message;
using KM_ClientApp.Endpoint.Session;
using KM_ClientApp.Middleware;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logger
builder.Host.UseSerilog();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = new JsonLowerCaseKeyPolicy();
    });

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddCors(options =>
    options.AddPolicy("local", cfg =>
        cfg.WithOrigins("http://localhost:5173", "http://localhost:2020", "http://localhost:64159")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod()
    )
);

builder.Services.AddRazorPages();

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddScoped<ISQLConnectionFactory, SQLServerConnection>();
builder.Services.AddScoped<IConfigRepository, ConfigRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();


builder.Services.AddTransient<ErrorExceptionMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //Use Middleware For Released App

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseCors("local");
}


app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMiddleware<ErrorExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=App}/{action=Index}");

app.Run();