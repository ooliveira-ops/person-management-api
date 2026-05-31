using Api.Data;
using Api.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Api.Models;
using Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// add
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer("Server=localhost;Database=PersonManagementApi;User Id=sa;Password=__REMOVED__;TrustServerCertificate=true;")
);

builder.Services.AddScoped<IPersonRepository, PersonRepository>();          //injeção de dependência: "Sempre que alguém pedir um IPersonRepository, dê uma instância de PersonRepository"
builder.Services.AddScoped<IValidator<Person>, PersonValidator>();

builder.Services.AddControllers();											//registra os controllers
builder.Services.AddEndpointsApiExplorer();	
builder.Services.AddSwaggerGen();

var app = builder.Build();

// http:
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();                                               //mapeia os endpoints dos controllers para que possam ser acessados via HTTP

app.Run(); 