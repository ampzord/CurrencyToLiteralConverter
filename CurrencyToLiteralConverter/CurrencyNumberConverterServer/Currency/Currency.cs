using System.Net;
using System.Web.Http;
using CurrencyNumberConverter.Currency;

namespace CurrencyNumberConverterServer.Currency;

public static class Currency
{
    public static ICurrency? Validate(string currency)
    {
        
        try
        {
            ICurrency moneyCurrency;
            switch (currency)
            {
                case "EUR":
                    moneyCurrency = new CurrencyEur();
                    break;
                case "USD":
                    moneyCurrency = new CurrencyUsd();
                    break;
                default:
                    var message = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity);
                    message.Content = new StringContent($"The currency does not exist: {currency}.");
                    throw new HttpResponseException(message);
            }
            return moneyCurrency;

        }
        catch (Exception e)
        {
            
        }

        return null;
    }
}