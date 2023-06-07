using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CurrencyNumberConverterServer.Tests;

public class NumberConvertedTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public NumberConvertedTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    /// <summary>
    /// Testing for correct result if given a valid input by the user
    /// </summary>
    /// <param name="url"></param>
    [Theory]
    [InlineData("/api/numberconverter?value=55,01&currency=USD")]
    public async Task GetNumberConverter_GivenValidNumber_ReturnsSuccess(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.Should().BeSuccessful();
    }
    
    /// <summary>
    /// Testing for client error if given a invalid input by the user
    /// </summary>
    /// <param name="url"></param>
    [Theory]
    [InlineData("/api/numberconverter?value=-1&currency=USD")]
    [InlineData("/api/numberconverter?value=25,456&currency=USD")]
    [InlineData("/api/numberconverter?value=256a345,07&currency=USD")]
    [InlineData("/api/numberconverter?value=256,34&currency=CAD")]
    public async Task GetNumberConverter_GivenInvalidInput_ReturnsClientError(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.Should().HaveClientError();
    }
    
}