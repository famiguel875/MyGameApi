using MyGameApi.Model;
using MyGameApi.Service;

var builder = WebApplication.CreateBuilder(args);

// Configurar la inyección de dependencias para GamePlayerDatabaseSettings
builder.Services.Configure<GamePlayerDatabaseSettings>(
    builder.Configuration.GetSection("GamePlayerDatabaseSettings"));

// Registrar el servicio de jugadores
builder.Services.AddSingleton<GamePlayerService>();

// Agregar controladores
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();