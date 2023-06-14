using System.ComponentModel.DataAnnotations;

namespace CurrencyNumberConverter.Exceptions;

public class InvalidDecimalPartLengthException : ArgumentOutOfRangeException
{
    public string Number { get; init; }

    public InvalidDecimalPartLengthException(string number)
    {
        Number = number;
    }
    
    public override string Message => $"Decimal part must be between 0 and 99, value:{Number}";
}

public class DecimalPartNotAnIntException : ValidationException
{
    public override string Message => "Decimal part of the number is not a valid int.";
}

public class InvalidDecimalPartNumberRange : ArgumentOutOfRangeException
{
    public string Number { get; init; }

    public InvalidDecimalPartNumberRange(string number)
    {
        Number = number;
    }
    
    public override string Message => $"Decimal part must be between 0 and 99, value:{Number}";
}