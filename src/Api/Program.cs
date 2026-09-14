using Api.Data;
using Api.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Api.Models;
using Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// A connection string vem da configuração (appsettings.Development.json, user-secrets
// ou variável de ambiente) — NUNCA embutida aqui.
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Injeção de dependência: quem pedir IPersonRepository recebe um PersonRepository.
// Scoped = uma instância por requisição HTTP, compartilhando o mesmo DbContext.
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

// Regras de negócio de Person (data não futura, tamanho do nome). Injetado no
// PersonsController e aplicado no POST e no PUT.
builder.Services.AddScoped<IValidator<Person>, PersonValidator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Origens autorizadas a chamar a API pelo navegador. Vêm da configuração para
// que cada ambiente tenha a sua sem recompilar.
var origensPermitidas = builder.Configuration
	.GetSection("Cors:AllowedOrigins")
	.Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
	options.AddPolicy("CorsPolicy", policy =>
	{
		policy.WithOrigins(origensPermitidas)
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});


var app = builder.Build();

// Swagger só existe em Development. Rodando em Production (o default quando
// ASPNETCORE_ENVIRONMENT não está definido), /swagger e a raiz devolvem 404.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	app.MapGet("/", () => Results.Redirect("/swagger"));
}

// No perfil "http" não há porta HTTPS configurada: este middleware apenas registra um
// aviso e deixa a requisição passar. No perfil "https", redireciona a 5164 para a 7221.
app.UseHttpsRedirection();

// Precisa vir antes do MapControllers: o middleware responde (OPTIONS)
// que o navegador envia e adiciona os headers de CORS na resposta. Sem eles, a API
// ainda responde normalmente — quem recusa a LEITURA da resposta é o navegador.
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
