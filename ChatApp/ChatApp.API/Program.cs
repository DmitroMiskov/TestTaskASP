using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ChatApp.Services;
using ChatApp.Hubs;
using ChatApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Додаємо сервіси до контейнера
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Додаємо CORS для фронтенду
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Перевіряємо, чи використовувати In-Memory базу даних для розробки
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ChatDbContext>(options =>
        options.UseInMemoryDatabase("ChatDb"));
        
    // Використовуємо заглушку для аналізу тональності під час розробки
    builder.Services.AddSingleton<ISentimentAnalysisService, MockSentimentAnalysisService>();
}
else
{
    // Використовуємо SQL Server для виробництва
    builder.Services.AddDbContext<ChatDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        
    // Додаємо сервіс аналізу тональності
    builder.Services.AddSingleton<ISentimentAnalysisService>(sp =>
    {
        var endpoint = builder.Configuration["CognitiveServices:Endpoint"];
        var apiKey = builder.Configuration["CognitiveServices:ApiKey"];
        var logger = sp.GetService<ILogger<AzureSentimentAnalysisService>>();
        
        if (endpoint == null || apiKey == null) 
            throw new InvalidOperationException("CognitiveServices configuration is missing");
            
        return new AzureSentimentAnalysisService(endpoint, apiKey, logger);
    });
}

// Додаємо SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Налаштування конвеєра HTTP-запитів
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting(); 
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();

// Налаштування маршруту для SignalR хаба
app.MapHub<ChatHub>("/chatHub");

// Створення бази даних та міграцій при запуску
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    if (app.Environment.IsDevelopment())
    {
        // Для In-Memory бази просто переконуємося, що вона створена
        dbContext.Database.EnsureCreated();
    }
    else
    {
        // Для реальної бази даних виконуємо міграції
        dbContext.Database.Migrate();
    }
}

app.Run();