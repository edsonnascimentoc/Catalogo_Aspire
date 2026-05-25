using Catalogo.ApiProdutos.Context;
using Catalogo.ApiProdutos.Endpoints;
using Keycloak.AuthServices.Authentication;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddProblemDetails();
builder.AddSqlServerDbContext<ProdutoDataContext>("sqldb");

builder.Services.AddKeycloakWebApiAuthentication(
    builder.Configuration, 
    options =>
    { options.Audience = builder.Configuration["Keycloak:ClientId"];
      options.RequireHttpsMetadata = false;

    }
 );

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

// Mapear os endpoints de produtos
app.MapProdutoEndpoints();
app.CreateDbIfNotExists();

app.Run();