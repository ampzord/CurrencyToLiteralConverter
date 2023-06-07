using CurrencyNumberConverter.Currency;
using Newtonsoft.Json;

namespace CurrencyNumberConverterServer;

static partial class Program
{
    internal static void NumberConverterRoute(this WebApplication app)
    {
        app.MapGet("/api/numberconverter/",IResult (string value, string currency) =>
            {
                ICurrency moneyCurrency = Currency.Currency.Validate(currency);

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
    }
}