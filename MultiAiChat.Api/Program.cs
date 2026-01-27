using Microsoft.EntityFrameworkCore;
using MultiAiChat.Api.Data;
using MultiAiChat.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddHttpClient<OpenAiService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()   
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.EnsureCreated();
        Console.WriteLine("--> Veritabaný ve tablolar baþarýyla kontrol edildi/oluþturuldu.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Veritabaný oluþturulurken hata: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();



app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();