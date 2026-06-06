using linkedin_api.Data;
using linkedin_api.Services;
using Microsoft.EntityFrameworkCore;

EnvironmentService.LoadDotEnv(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IMediaStorageService, LocalMediaStorageService>();
builder.Services.AddScoped<IEmailService, BrevoEmailService>();
builder.Services.AddScoped<IApiEntityService, ApiEntityService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING")
        ?? "Server=localhost;Port=3306;Database=linkedin_db;User=root;Password=root;";
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "SimpleStorage")),
    RequestPath = "/media"
});

app.Use(async (context, next) =>
{
    var tokenService = context.RequestServices.GetRequiredService<IJwtTokenService>();
    var header = context.Request.Headers.Authorization.ToString();
    if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
        var principal = tokenService.Validate(header["Bearer ".Length..].Trim());
        if (principal is not null) context.User = principal;
    }
    await next();
});

app.MapControllers();
app.MapHub<linkedin_api.Hubs.ConversationHub>("/ws/conversations/{conversationId:int}");
app.MapHub<linkedin_api.Hubs.NotificationHub>("/ws/notifications");
app.MapGet("/", () => Results.Ok(new
{
    name = "LinkedIn Clone API",
    version = "v1",
    swagger = "/swagger",
    endpoints = 220
}));

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Database initialization skipped. Start MySQL with linkedin_db/root/root to enable persistence.");
    }
}

app.Run();
