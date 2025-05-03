using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;
using AuthImplementation.Extensions;
using AuthImplementation.Application.Features.HealthCheck;
using AuthImplementation.Infrastructure.Extensions;
using AuthImplementation.Application.Extensions;

using AuthImplementation.Infrastructure.Services.Email;
using AuthImplementation.Infrastructure.Services.Email.Interfaces;
using AuthImplementation.Infrastructure.Services.Identity;
using AuthImplementation.Infrastructure.Services.Identity.Interfaces;
using AuthImplementation.Application.Features.Auth;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddLogging();

builder.Services.AddCorsExtension(builder.Configuration);
builder.Services.AddDatabaseExtension(builder.Configuration);
builder.Services.AddValidationExtension();
builder.Services.AddAppExtension();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


builder.Services.AddAuthorization();
builder.Services.AddAuthExtension(builder.Configuration);
builder.Services.AddScoped<IEmailService, MailtrapEmailService>();  
builder.Services.AddScoped<ITokenService, TokenService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowedOriginsCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthCheckEndpoints();

app.MapAuthEndpoints();


app.Run();