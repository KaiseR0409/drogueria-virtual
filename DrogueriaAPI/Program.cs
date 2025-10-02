using DrogueriaAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
//conecta a la base de datos con esta linea
//addDbContext registra  drogueriaDbContext como un servicio
builder.Services.AddDbContext<DrogueriaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DrogueriaDb")));
builder.Services.AddControllers();
//configura CORS para permitir solicitudes desde el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
