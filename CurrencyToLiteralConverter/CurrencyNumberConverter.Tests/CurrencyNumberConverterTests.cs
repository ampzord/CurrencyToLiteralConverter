using System.ComponentModel.DataAnnotations;
using CurrencyNumberConverter.Currency;
using CurrencyNumberConverter.Tests.MockCurrency;
using FluentAssertions;
using Xunit;

namespace CurrencyNumberConverter.Tests;

public class CurrencyNumberConverterTests
{
    
    [Theory]
    [InlineData("0", "zero dollars")]
    [InlineData("0,01", "zero dollars and one cent")]
    [InlineData("1", "one dollar")]
    [InlineData("25,1", "twenty-five dollars and ten cents")]
    [InlineData("45 100", "forty-five thousand one hundred dollars")]
    [InlineData("4 841 100,78", "four million eight hundred forty-one thousand one hundred dollars and seventy-eight cents")]
    [InlineData("999 999 999,99", "nine hundred ninety-nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars and ninety-nine cents")]
    public void CurrencyNumberConverter_Convert_ReturnsNumberInLiteralDollars(string number, string expected)
    {
        // Arrange
        CurrencyNumberConverter numberConverter = new CurrencyNumberConverter(new CurrencyUsd());
        
        // Act
        string result = numberConverter.Convert(number);
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("0,01", "zero euros and one cent")]
    [InlineData("1,02", "one euro and two cents")]
    public void CurrencyNumberConverter_Convert_ReturnsNumberInLiteralEuros(string number, string expected)
    {
        // Arrange
        CurrencyNumberConverter numberConverter = new CurrencyNumberConverter(new CurrencyEur());
        
        // Act
        string result = numberConverter.Convert(number);
        
        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("0,01", "zero pounds and one penny")]
    [InlineData("1,02", "one pound and two pennies")]
    public void CurrencyNumberConverter_Convert_ReturnsNumberInLiteralMockedPounds(string number, string expected)
    {
        // Arrange
        CurrencyNumberConverter numberConverter = new CurrencyNumberConverter(new MockCurrencyGbp());
        
        // Act
        string result = numberConverter.Convert(number);
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("45100", "forty-five thousand one hundred dollars")]
    [InlineData("999999999,99", "nine hundred ninety-nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars and ninety-nine cents")]
    public void CurrencyNumberConverter_Convert_ReturnsNumberInLiteralDollarsWithoutSpacing(string number, string expected)
    {
        // Arrange
        CurrencyNumberConverter numberConverter = new CurrencyNumberConverter(new CurrencyUsd());
        
        // Act
        string result = numberConverter.Convert(number);
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("-1")]
    [InlineData("1999999999,99")]
    public void CurrencyNumberConverter_Convert_ReturnsArgumentOutOfRangeException(string number)
    {
        // Arrange
        CurrencyNumberConverter numberConverter = new CurrencyNumberConverter(new CurrencyUsd());
    
        // Act
        Action act = () => numberConverter.Convert(number);
        
        // Assert
        act
            .Should().Throw<ArgumentOutOfRangeException>()
            .Where(e => e.Message.Contains("Number must be between 0 and 999 999 999."));
    }
    
    [Theory]
    [InlineData("12a,4")] 
    [InlineData("123,a")] 
    public void CurrencyNumberConverter_Convert_ReturnsValidationException(string number)
    {
        // Arrange
        CurrencyNumberConverter numberConverter = new CurrencyNumberConverter(new CurrencyUsd());

        // Act
        Action act = () => numberConverter.Convert(number);

        // Assert
        act
            .Should().Throw<ValidationException>()
            .WithMessage("*number is not a valid int.");
    }
    
    [Theory]
    [InlineData("123,001")]
    public void CurrencyNumberConverter_Convert_InvalidLengthDecimalPart(string number)
    {
        // Arrange
        CurrencyNumberConverter numberConverter = new CurrencyNumberConverter(new CurrencyUsd());

        // Act
        Action act = () => numberConverter.Convert(number);

        // Assert
        act
            .Should().Throw<ArgumentOutOfRangeException>()
            .Where(e => e.Message.Contains("Decimal part must be between 0 and 99."));
    }
    
    [Theory]
    [InlineData("999 999 999 1,01")]
    public void CurrencyNumberConverter_Convert_ValueHigherThanMaxInt(string number)
    {
        // Arrange
        CurrencyNumberConverter numberConverter = new CurrencyNumberConverter(new CurrencyUsd());

        // Act
        Action act = () => numberConverter.Convert(number);

        // Assert
        act
            .Should().Throw<ValidationException>()
            .WithMessage("*number is not a valid int.");
    }

}