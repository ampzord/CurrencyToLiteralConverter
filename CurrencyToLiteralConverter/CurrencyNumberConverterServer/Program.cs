using CurrencyNumberConverter.Currency;
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

app.UseHttpsRedirection();

app.MapGet("/api/numberconverter/",IResult (string value, string currency) =>
    {
        ICurrency moneyCurrency = Currency.Validate(currency);

        try
        {
            CurrencyNumberConverter.CurrencyNumberConverter moneyLiteralConverter = new(moneyCurrency);
            string result = moneyLiteralConverter.Convert(value);
            string resultSerialized = JsonConvert.SerializeObject(new { Message = result});
            
            return TypedResults.Ok(resultSerialized);
        }
        catch (Exception e)
        {
            return TypedResults.UnprocessableEntity(e.Message);
        }

    })
    .WithName("GetNumberConverter")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status422UnprocessableEntity);

app.Run();