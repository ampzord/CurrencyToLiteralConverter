namespace CurrencyNumberConverter.Currency;

public class CurrencyUsd : ICurrency
{
    public string FullName => "US Dollar";
    public string Symbol => "$";
    public string IsoCode => "USD";
    public string MoneyPlural => "dollars";
    public string MoneySingular => "dollar";
    public string Cents => "cents";
    public string Cent => "cent";
}