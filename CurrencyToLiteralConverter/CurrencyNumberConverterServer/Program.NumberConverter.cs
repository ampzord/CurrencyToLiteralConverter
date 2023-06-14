using CurrencyNumberConverter.Currency;
using Newtonsoft.Json;
using Serilog.Core;

namespace CurrencyNumberConverterServer;

static partial class Program
{
    internal static void NumberConverterRoute(this WebApplication app, Logger logger)
    {
        app.MapGet("/api/numberconverter/", IResult (string value, string currency) =>
            {
                logger.Information("Processing GET numberconverter request with Value={Value} and Currency={Currency}.", value, currency);
                ICurrency? moneyCurrency = Currency.Currency.Validate(currency);

                try
                {
                    CurrencyNumberConverter.CurrencyNumberConverter moneyLiteralConverter = new(moneyCurrency);
                    string resultAnswer = moneyLiteralConverter.Convert(value);

                    var resultObject = new { result = resultAnswer };
                    var resultSerialized = JsonConvert.SerializeObject(resultObject);

                    logger.Information("Returning 200 Response {@Value}.", resultObject);
                    return TypedResults.Ok(resultSerialized);
                }
                catch (Exception e)
                {
                    logger.Warning("Returning BadRequest Unprocessable Entity with Value={Value} and Currency={Currency}.", value, currency);
                    return TypedResults.UnprocessableEntity(e.Message);
                }

            })
            .WithName("GetNumberConverter")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status422UnprocessableEntity);
    }
}