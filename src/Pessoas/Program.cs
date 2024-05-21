using Microsoft.Extensions.Logging.Abstractions;
using Pessoas.Repositories;
using Pessoas.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddNpgsqlDataSource(
    Environment.GetEnvironmentVariable(
        "DB_CONNECTION_STRING") ??
        "ERRO de connection string!!!", dataSourceBuilderAction: a => { a.UseLoggerFactory(NullLoggerFactory.Instance); });

builder.Services.AddScoped<PessoaRepository>();
//builder.Services.AddSingleton<PessoaRepository>();
builder.Services.AddScoped<PessoaService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
