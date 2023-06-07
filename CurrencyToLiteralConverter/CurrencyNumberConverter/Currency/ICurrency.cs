namespace CurrencyNumberConverter.Currency;

public interface ICurrency
{
    public string FullName { get; }
    public string Symbol { get; }
    public string IsoCode { get; }
    public string MoneyPlural { get; }
    public string MoneySingular { get;  }
    public string Cents { get; }
    public string Cent { get; }
}