using System.ComponentModel.DataAnnotations;
using System.Text;
using CurrencyNumberConverter.Currency;

namespace CurrencyNumberConverter;

public class CurrencyNumberConverter
{
    private readonly Dictionary<int, string> _moneyConverterLiteral;
    private readonly ICurrency? _currency;
    private readonly string[] _integerCurrencyLevels;

    public CurrencyNumberConverter(ICurrency? currency)
    {
        _currency = currency;
        _moneyConverterLiteral = new Dictionary<int, string>();
        _integerCurrencyLevels = new string[]
        {
            "",
            "thousand",
            "million"
        };
        
        FillMoneyConverterLiteral();
    }

    /// <summary>
    /// Converts a given string number into a literal text string
    /// </summary>
    /// <param name="input">number to transform to literal</param>
    /// <returns>
    /// Returns a string containing the conversion of the given input number to literal text.
    /// </returns>
    public string Convert(string input)
    {
        StringBuilder numberConverted = new StringBuilder();
        
        var (integerPart, decimalPart) = ParseNumber(input);

        ValidateNumber(integerPart, decimalPart);

        numberConverted.Append(ProcessIntegerPart(integerPart));

        if (!string.IsNullOrEmpty(decimalPart))
        {
            numberConverted.Append(" and ");
            numberConverted.Append(ProcessDecimalPart(decimalPart));
        }

        return numberConverted.ToString();
    }

    /// <summary>
    /// Checks if the received number is in the correct format.
    /// </summary>
    /// <param name="integerPart"></param>
    /// <param name="decimalPart"></param>
    /// <exception cref="ValidationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void ValidateNumber(string integerPart, string decimalPart)
    {
        if (!int.TryParse(integerPart, out var integerNumber))
            throw new ValidationException("Integer part of the number is not a valid int.");

        if (integerNumber is < 0 or > 999_999_999)
            throw new ArgumentOutOfRangeException(integerPart + "," + decimalPart,"Number must be between 0 and 999 999 999.");

        if (string.IsNullOrEmpty(decimalPart))
            return;
        
        if (decimalPart.Length > 2)
            throw new ArgumentOutOfRangeException(decimalPart,"Decimal part must be between 0 and 99.");
        
        if (!int.TryParse(decimalPart, out var decimalNumber))
            throw new ValidationException("Decimal part of the number is not a valid int.");
        
        if (decimalNumber is < 0 or > 99)
            throw new ArgumentOutOfRangeException(decimalPart,"Decimal part must be between 0 and 99.");

    }
    
    /// <summary>
    /// Parses the number, removing white spaces and transforming the comma into dot if they exist.
    /// </summary>
    /// <param name="number">original number</param>
    /// <returns>
    /// Returns a tuple with the integerPart and decimalPart correctly processed.
    /// </returns>
    private (string, string) ParseNumber(string number)
    {
        string decimalPart = string.Empty;
        
        StringBuilder sb = new StringBuilder(number);
        sb.Replace(" ", "");
        sb.Replace(",", ".");

        var splitNumber = sb.ToString().Split('.');
        if (splitNumber.Length > 1)
        {
            decimalPart = splitNumber[1];
        }

        string integerPart = splitNumber[0];

        return (integerPart, decimalPart);
    }

    /// <summary>
    /// The whole process of transforming the integer part of the number into a literal string.
    /// </summary>
    /// <param name="integerPart">complete integer number</param>
    /// <returns>
    /// Returns the transformation of the integer part of the original number to literal.
    /// </returns>
    private string ProcessIntegerPart(string integerPart)
    {
        StringBuilder literal = new StringBuilder();

        int integerNumber = int.Parse(integerPart);

        if (integerNumber == 0)
        {
            literal.Append("zero " + AddCurrencyAmountLiteral(0, 0));
            return literal.ToString();
        }

        int indexLevel = 0;
        while (integerNumber > 0)
        {
            int tmpNumber = integerNumber % 1000;
            
            string intLiteral = TransformIntegerToLiteral(tmpNumber) + AddCurrencyAmountLiteral(tmpNumber, indexLevel);
            
            literal.Insert(0, intLiteral);

            indexLevel++;
            integerNumber /= 1000;
        }

        return literal.ToString();
    }

    /// <summary>
    /// Holds the process of transforming a maximum three-digit number into the correct output.
    /// </summary>
    /// <param name="integerValue">integer number within 1-999 inclusive</param>
    /// <returns>
    /// Returns the literal transformation of the integer within 1 and 999 inclusive.
    /// </returns>
    private string TransformIntegerToLiteral(int integerValue)
    {
        StringBuilder literal = new StringBuilder();
        
        literal.Append(HandleThreeDigitNumber(integerValue));

        int twoDigitInteger = integerValue % 100;
        
        if (twoDigitInteger != 0)
            literal.Append(HandleTwoDigitNumber(twoDigitInteger));
        
        return literal.ToString();
    }

    /// <summary>
    /// Transforms received three-digit number into the left-most number + hundred string.
    /// </summary>
    /// <param name="integerValue">three-digit number</param>
    /// <returns>
    /// Returns the left-most number of the input with the hundred keyword.
    /// </returns>
    private string HandleThreeDigitNumber(int integerValue)
    {
        string literal = string.Empty;
        bool isHundred = System.Convert.ToBoolean(integerValue / 100);

        if (isHundred)
        {
            int leftDigit = integerValue / 100;
            
            if (_moneyConverterLiteral.TryGetValue(leftDigit, out var value))
                literal = value + " hundred ";
        }

        return literal;
    }
    
    /// <summary>
    /// Transforms received two-digit number into a two-digit number in literal text.
    /// </summary>
    /// <param name="integerValue">two-digit number</param>
    /// <returns>
    /// Returns correct transformation of the two-digit number in string.
    /// </returns>
    private string HandleTwoDigitNumber(int integerValue)
    {
        StringBuilder literal = new StringBuilder();
        
        if (_moneyConverterLiteral.TryGetValue(integerValue, out var value))
        {
            literal.Append(value + " ");
        }
        else
        {
            int leftDigit = integerValue / 10;
            int rightDigit = integerValue % 10;

            literal.Append(_moneyConverterLiteral[leftDigit * 10] + "-" + _moneyConverterLiteral[rightDigit] + " ");
        }

        return literal.ToString();
    }

    /// <summary>
    /// Processes the decimal part of the number 
    /// </summary>
    /// <param name="decimalPart">decimal part of the original number</param>
    /// <returns>
    /// The current transformation of the decimal part to the literal part,
    /// also adding the cents.
    /// </returns>
    /// <exception cref="FormatException"></exception>
    private string ProcessDecimalPart(string decimalPart)
    {
        StringBuilder literal = new StringBuilder();

        int decimalValue = int.Parse(decimalPart);

        //Checking the length of the string to transfrom 5.1 into 5.10, making ten searchable in the dictionary
        if (decimalPart.Length == 1)
        {
            decimalValue *= 10;
        }
        
        literal.Append(HandleTwoDigitNumber(decimalValue));

        literal.Append(AddCentsAmountLiteral(decimalValue));
        
        return literal.ToString();
    }

    /// <summary>
    /// Adds the specific name for cents or cent.
    /// </summary>
    /// <param name="number"></param>
    /// <returns>
    /// Returns cent if number is equal to 1 and
    /// Returns cents if different
    /// </returns>
    private string AddCentsAmountLiteral(int number)
    {
        if (number == 1)
            return $"{_currency?.Cent}";
        
        return $"{_currency?.Cents}";
    }

    /// <summary>
    /// Adds the currency number depending on the indexLevel
    /// </summary>
    /// <param name="number"></param>
    /// <param name="indexLevel"></param>
    /// <returns>
    /// Returns the specific value of hundred, thousand or dollars/dollar
    /// depending on the value indexLevel
    /// </returns>
    private string AddCurrencyAmountLiteral(int number, int indexLevel)
    {
        if (indexLevel != 0)
            return _integerCurrencyLevels[indexLevel] + " ";
        
        return number == 1 ? $"{_currency?.MoneySingular}" : $"{_currency?.MoneyPlural}";
    }
    
    /// <summary>
    /// Fills dictionary to hold the number to literal conversion for specific cases and basic cases.
    /// </summary>
    private void FillMoneyConverterLiteral()
    {
        // 0 - 9
        _moneyConverterLiteral.Add(0, "zero");
        _moneyConverterLiteral.Add(1, "one");
        _moneyConverterLiteral.Add(2, "two");
        _moneyConverterLiteral.Add(3, "three");
        _moneyConverterLiteral.Add(4, "four");
        _moneyConverterLiteral.Add(5, "five");
        _moneyConverterLiteral.Add(6, "six");
        _moneyConverterLiteral.Add(7, "seven");
        _moneyConverterLiteral.Add(8, "eight");
        _moneyConverterLiteral.Add(9, "nine");
        
        // 10 - 19
        _moneyConverterLiteral.Add(10, "ten");
        _moneyConverterLiteral.Add(11, "eleven");
        _moneyConverterLiteral.Add(12, "twelve");
        _moneyConverterLiteral.Add(13, "thirteen");
        _moneyConverterLiteral.Add(14, "fourteen");
        _moneyConverterLiteral.Add(15, "fifteen");
        _moneyConverterLiteral.Add(16, "sixteen");
        _moneyConverterLiteral.Add(17, "seventeen");
        _moneyConverterLiteral.Add(18, "eighteen");
        _moneyConverterLiteral.Add(19, "nineteen");
        
        // 2x - 9x
        _moneyConverterLiteral.Add(20, "twenty");
        _moneyConverterLiteral.Add(30, "thirty");
        _moneyConverterLiteral.Add(40, "forty");
        _moneyConverterLiteral.Add(50, "fifty");
        _moneyConverterLiteral.Add(60, "sixty");
        _moneyConverterLiteral.Add(70, "seventy");
        _moneyConverterLiteral.Add(80, "eighty");
        _moneyConverterLiteral.Add(90, "ninety");
        
        // 100
        _moneyConverterLiteral.Add(100, "hundred");
        
        // 1_000
        _moneyConverterLiteral.Add(1000, "thousand");
        
        // 100_000
        _moneyConverterLiteral.Add(100000, "hundred thousand");
        
        // 100_000_000 
        _moneyConverterLiteral.Add(1000000, "million");
    }
}

