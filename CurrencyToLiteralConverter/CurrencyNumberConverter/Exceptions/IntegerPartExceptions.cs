using System.ComponentModel.DataAnnotations;

namespace CurrencyNumberConverter.Exceptions;

public class IntegerPartNotAnIntException : ValidationException
{
    public override string Message => "Integer part of the number is not a valid int.";
}

public class InvalidNumberRangeException : ArgumentOutOfRangeException
{
    public string Number { get; init; }

    public InvalidNumberRangeException(string number)
    {
        Number = number;
    }
    
    public override string Message => $"Number must be between 0 and 999 999 999, value:{Number}";
}