namespace CurrencyNumberConverter.Currency;

public class CurrencyEur : ICurrency
{
    public string FullName => "Euro";
    public string Symbol => "€";
    public string IsoCode => "EUR";
    public string MoneyPlural => "euros";
    public string MoneySingular => "euro";
    public string Cents => "cents";
    public string Cent => "cent";
}