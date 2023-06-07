using CurrencyNumberConverter.Currency;
using CurrencyNumberConverterServer;
using CurrencyNumberConverterServer.Currency;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new() { Title = "Money Currency Converter API", Version="v1"});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapFallback(() => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.NumberConverterRoute();

app.Run();

// This is needed to access Program in Server.Tests
public partial class Program { }

