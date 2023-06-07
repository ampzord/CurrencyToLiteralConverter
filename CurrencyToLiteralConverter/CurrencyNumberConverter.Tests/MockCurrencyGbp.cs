using CurrencyNumberConverter.Currency;

namespace CurrencyNumberConverter.Tests;

public class MockCurrencyGbp : ICurrency
{
    public string FullName => "Pound sterling";
    public string Symbol => "£";
    public string IsoCode => "GBP";
    public string MoneyPlural => "pounds";
    public string MoneySingular => "pound";
    public string Cents => "pennies";
    public string Cent => "penny";
}